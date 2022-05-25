using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IDAstar
{
    public int[,] map;
    private int Size;
    //DFS需要用到的数据结构----栈
    Stack<PointNormal> IDA_Stack = new Stack<PointNormal>();
    List<PointNormal> closeList = new List<PointNormal>();
    List<PointNormal> Surround = new List<PointNormal>();//用于存储已经检查过的点
    public Stack<PointNormal> GizmosIDAstack;
    //初始化地图
    public void initMap(int[,] arr)
    {
        this.map = arr;
    }
    //开始寻路，重点知识
    void IDA_dfs(PointNormal start, PointNormal end)
    {
        int maxDepth = Mathf.Abs(end.x - start.x) + Mathf.Abs(end.y - start.y);//根据终点计算最大深度
        int curDepth;
        IDA_Stack.Push(start);//将起点放入栈中
        while (IDA_Stack.Count > 0)
        {
            PointNormal curPoint = IDA_Stack.Pop();//将出栈的点作为curPoint
            curDepth = Mathf.Abs(curPoint.x - start.x) + Mathf.Abs(curPoint.y - start.y);
            if (curDepth > maxDepth)//如果到达最大深度了
            {
                continue;
            }
            closeList.Add(curPoint);//加入关闭列表，表示已经搜索过该点
            Surround = GetSurroundPoint(curPoint);//获取当前点的周围点
            for (int i = 0; i < Surround.Count; i++)//为什么要用for而不用foreach,因为foreach在遍历时无法修改和删除集合数据
            {
                curDepth = Mathf.Abs(Surround[i].x - start.x) + Mathf.Abs(Surround[i].y - start.y);
                if (curDepth > maxDepth) continue;
                if (CalculateH(Surround[i], end) + curDepth > maxDepth) continue;
                if (Surround[i].x == end.x && Surround[i].y == end.y)//如果周围点的集合中有终点
                {
                    Surround[i] = end;//当检查到end时，修改end的parent,引用类型的=相当于传递地址，修改了Surround[i]才能作用到end本身
                }
                IDA_Stack.Push(Surround[i]);//否则加入栈中
                Surround[i].parent = curPoint;//将周围的点的父节点设为curPoint
            }
            if (isInStack(IDA_Stack, end)!=null)
            {
                GizmosIDAstack = new Stack<PointNormal>(IDA_Stack);
                foreach(var po in closeList)
                {
                    GizmosIDAstack.Push(po);
                }
                break;
            }
        }
    }

    public List<PointNormal> GetPath(PointNormal start, PointNormal end)
    {
        IDA_dfs(start, end);
        List<PointNormal> Path = new List<PointNormal>();
        PointNormal temp = end;
        //Debug.Log(temp.parent);
        while (temp != null)
        {
            Path.Add(temp);
            temp = temp.parent;
        }
        Path.Reverse();
        IDA_Stack.Clear();
        closeList.Clear();
        return Path;

    }

    PointNormal isInList(List<PointNormal> list, PointNormal point)
    {
        foreach (var p in list)
        {
            if (p.x == point.x && p.y == point.y)
                return p;
        }
        return null;
    }
    //判断点是否在栈中，为什么不用Contain,因为我们使用的new去创建的Pointdfs,两个x,y一样的Pointdfs的地址不同
    PointNormal isInStack(Stack<PointNormal> stack, PointNormal point)
    {
        foreach (var p in stack)
        {
            if (p.Equal(point))
            {
                return p;
            }
        }
        return null;
    }
    //获取当前点的周围点
    List<PointNormal> GetSurroundPoint(PointNormal curPoint)
    {
        List<PointNormal> list = new List<PointNormal>();
        Size = MapController.instance.mapSize;
        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                if (Mathf.Abs(i) == Mathf.Abs(j)) continue;
                if (isInStack(IDA_Stack, new PointNormal(curPoint.x + i, curPoint.y + j)) != null
                    || isInList(closeList, new PointNormal(curPoint.x + i, curPoint.y + j)) != null) continue;//已经检测过或已经在栈中就跳过
                if (curPoint.x + i < 0 || curPoint.x + i > Size - 1 || curPoint.y + j < 0 || curPoint.y + j > Size - 1 || map[curPoint.x + i, curPoint.y + j] == 1)
                {
                    //边界检查及是否为墙
                    continue;
                }
                list.Add(new PointNormal(curPoint.x + i, curPoint.y + j));
            }
        }
        return list;
    }

    int CalculateH(PointNormal cur,PointNormal end)
    {
        return Mathf.Abs(cur.x - end.x) + Mathf.Abs(cur.y - end.y);
    }

}
