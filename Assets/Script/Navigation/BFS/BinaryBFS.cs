using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BinaryBFS 
{
    private int[,] map;
    private int Size;
    [Header("双向BFS")]
    int flag;//一个标志，判断对哪一个队列进行操作
    PointNormal result;
    PointNormal Po;//用来保存结果
    Queue<PointNormal> q1 = new Queue<PointNormal>();
    Queue<PointNormal> q2 = new Queue<PointNormal>();
    List<PointNormal> closeList1 = new List<PointNormal>();
    List<PointNormal> closeList2 = new List<PointNormal>();//对搜索过的点进行保存，也就是说里面的点的集合就是搜索范围

    public List<PointNormal> GizmosList1;
    public List<PointNormal> GizmosList2;//画出搜索范围


    public void initMap(int[,] arr)
    {
        this.map = arr;
    }
    PointNormal isInQueue(Queue<PointNormal> queue, PointNormal point)
    {
        foreach (var p in queue)
        {
            if (p.Equal(point))
            {
                return p;
            }
        }
        return null;
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

    PointNormal DBFS(PointNormal start,PointNormal end)
    {
        PointNormal tempPoint;//用来保存要出队的节点
        Size = MapController.instance.mapSize;
        q1.Enqueue(start);//起点入队
        q2.Enqueue(end);//终点入队
        while (q1.Count > 0 && q2.Count > 0)//只要有一个队列不为空就继续
        {
            if (q1.Count > q2.Count)//对小的队列进行操作
            {
                flag = 0;
                tempPoint = q2.Dequeue();
                closeList2.Add(tempPoint);
            }
            else
            {
                flag = 1;
                tempPoint = q1.Dequeue();
                closeList1.Add(tempPoint);
            }
            
            //tempPoint等于较小的对列中的队头
            int[] dx = { -1, 0, 1, 0 };
            int[] dy = { 0, -1, 0, 1 };
            for(int i = 0; i < 4; i++)
            {
                int x0 = tempPoint.x + dx[i];
                int y0 = tempPoint.y + dy[i];
                Po = new PointNormal(x0, y0);//即将搜索的节点
                if (x0 < 0 || x0 > Size - 1 || y0 < 0 || y0 > Size - 1 || map[x0,y0] == 1) continue;//边界条件
                if (flag == 1)
                {
                    if (isInQueue(q1, Po) != null || isInList(closeList1, Po) != null) continue;
                    if (isInQueue(q2, Po) != null)//当前点在q2中，说明两个BFS相交了，结束
                    {
                        result = tempPoint;//保存相交的点的父节点
                        return isInQueue(q2, Po);//返回相交的点
                    }
                    q1.Enqueue(Po);
                    Po.parent = tempPoint;
                }
                else
                {
                    if (isInQueue(q2, Po) != null || isInList(closeList2, Po) != null) continue;
                    if (isInQueue(q1, Po)!=null)
                    {
                        result = tempPoint;
                        return isInQueue(q1, Po);
                    }
                    q2.Enqueue(Po);
                    Po.parent = tempPoint;
                }
            }
        }
        return null;

    }

    public List<PointNormal> GetPath(PointNormal start, PointNormal end)
    {
        PointNormal ans = DBFS(start, end);
        //Debug.Log(ans.x + " " + ans.y);
        //Debug.Log(result.x + " " + result.y);
        //Debug.Log("---------");
        List<PointNormal> Path = new List<PointNormal>();
        while (ans!=null&&result!= null)
        {
            if (ans != null) Path.Add(ans);
            if (result != null) Path.Add(result);
            ans = ans.parent;
            result = result.parent;
        }
        while (ans != null)
        {
            Path.Add(ans);
            ans = ans.parent;
        }
        while (result != null)
        {
            Path.Add(result);
            result = result.parent;
        }
        GizmosList1 = new List<PointNormal>(closeList1);
        GizmosList2 = new List<PointNormal>(closeList2);//深拷贝防止接下来被Clear
        foreach(var p in q1)
        {
            GizmosList1.Add(p);
        }
        foreach(var p in q2)
        {
            GizmosList2.Add(p);
        }//目前还在队列里的点也是被搜索的，将他们放入对应的GizmosList
        //每次寻路都清空上一次寻路的结果
        q1.Clear();
        q2.Clear();
        closeList1.Clear();
        closeList2.Clear();
        return Path;
    }
}
