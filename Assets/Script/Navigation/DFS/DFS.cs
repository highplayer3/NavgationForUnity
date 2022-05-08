using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointNormal
{
    public int x, y;//代表坐标
    public PointNormal parent;
    public PointNormal(int _x, int _y)
    {
        this.x = _x;
        this.y = _y;
    }
    public virtual bool Equal(PointNormal point)//比较两个Point是否相等只需比较x和y即可
    {
        if (this.x == point.x && this.y == point.y)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
public class DFS 
{
    public int[,] map;
    private int Size;
    //DFS需要用到的数据结构----栈
    Stack<PointNormal> DFS_container = new Stack<PointNormal>();
    List<PointNormal> closeList = new List<PointNormal>();
    List<PointNormal> Surround = new List<PointNormal>();//用于存储已经检查过的点
    public Stack<PointNormal> Gizmosstack;
    //初始化地图
    public void initMap(int[,] arr)
    {
        this.map = arr;
    }
    //开始寻路，重点知识
    void dfs(PointNormal start,PointNormal end)
    {
        DFS_container.Push(start);//将起点放入栈中
        while (DFS_container.Count > 0)
        {
            PointNormal curPoint = DFS_container.Pop();//将出栈的点作为curPoint
            closeList.Add(curPoint);//加入关闭列表，表示已经搜索过该点
            Surround=GetSurroundPoint(curPoint);//获取当前点的周围点
            for(int i=0;i<Surround.Count;i++)//为什么要用for而不用foreach,因为foreach在遍历时无法修改和删除集合数据
            {
    
                if (Surround[i].x == end.x && Surround[i].y == end.y)//如果周围点的集合中有终点
                {
                    Surround[i] = end;//当检查到end时，修改end的parent,引用类型的=相当于传递地址，修改了Surround[i]才能作用到end本身
                }
                DFS_container.Push(Surround[i]);//否则加入栈中
                Surround[i].parent = curPoint;//将周围的点的父节点设为curPoint
                
            }
            if (isInStack(DFS_container,end)!=null)
            {
                Gizmosstack = new Stack<PointNormal>(DFS_container);
                foreach (var po in closeList)
                {
                    Gizmosstack.Push(po);
                }//上面这几步是为了画出地图
                break;
            }
        }
    }

    public List<PointNormal> GetPath(PointNormal start, PointNormal end)
    {
        dfs(start, end);
        List<PointNormal> Path = new List<PointNormal>();
        PointNormal temp = end;
        //Debug.Log(temp.parent);
        while (temp != null)
        {
            Path.Add(temp);
            temp = temp.parent;
        }
        Path.Reverse();
        DFS_container.Clear();
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
        for(int i = -1; i <= 1; i++)
        {
            for(int j = -1; j <= 1; j++)
            {
                if (Mathf.Abs(i) == Mathf.Abs(j)) continue;
                if (i == 0 && j == 0) continue;//自身则跳过
                if (isInStack(DFS_container,new PointNormal(curPoint.x + i, curPoint.y + j))!=null
                    ||isInList(closeList, new PointNormal(curPoint.x + i, curPoint.y + j))!=null) continue;//已经检测过或已经在栈中就跳过
                if (curPoint.x + i < 0 || curPoint.x + i > Size-1 || curPoint.y + j < 0 || curPoint.y + j > Size-1 || map[curPoint.x + i, curPoint.y + j] == 1)
                {
                    //边界检查及是否为墙
                    continue;
                }
                //if (i == -1 && j == -1)
                //{
                //    if (map[curPoint.x + i + 1, curPoint.y + j] == 1 && map[curPoint.x + i, curPoint.y + j + 1] == 1) continue;
                //}
                //if (i == 1 && j == -1)
                //{
                //    if (map[curPoint.x + i - 1, curPoint.y + j] == 1 && map[curPoint.x + i, curPoint.y + j + 1] == 1) continue;
                //}
                //if (i == 1 && j == 1)
                //{
                //    if (map[curPoint.x + i - 1, curPoint.y + j] == 1 && map[curPoint.x + i, curPoint.y + j - 1] == 1) continue;
                //}
                //if (i == -1 && j == 1)
                //{
                //    if (map[curPoint.x + i, curPoint.y + j - 1] == 1 && map[curPoint.x + i + 1, curPoint.y + j] == 1) continue;
                //}
                list.Add(new PointNormal(curPoint.x + i, curPoint.y + j));
            }
        }
        return list;
    }
}
