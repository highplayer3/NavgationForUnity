using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BFS 
{
    private int[,] map;
    private int Size;

    Queue<PointNormal> BFS_container = new Queue<PointNormal>();
    List<PointNormal> closeList = new List<PointNormal>();
    List<PointNormal> Surround = new List<PointNormal>();
    public Queue<PointNormal> Gizmosqueue;
    //初始化地图
    public void initMap(int[,] arr)
    {
        this.map = arr;
    }
    //BFS逻辑和DFS完全一样
    void bfs(PointNormal start,PointNormal end)
    {
        BFS_container.Enqueue(start);//开始节点入队
        while (BFS_container.Count > 0)
        {
            PointNormal curPoint = BFS_container.Dequeue();//取出当前节点
            closeList.Add(curPoint);//将当前节点加入CloseList
            Surround = GetSurroundPoint(curPoint);//获取周围点的集合
            for (int i = 0; i < Surround.Count; i++)
            {
                if (Surround[i].Equal(end))
                {
                    Surround[i] = end;//理由见DFS
                }
                BFS_container.Enqueue(Surround[i]);//将周围点依次入队
                Surround[i].parent = curPoint;//建立索引
            }
            if (isInQueue(BFS_container, end) != null)
            {
                Gizmosqueue = new Queue<PointNormal>(BFS_container);//浅拷贝，BFS_container被Clear时不会影响这儿
                foreach(var po in closeList)
                {
                    Gizmosqueue.Enqueue(po);
                }
                //以上部分是为了画出搜索路径
                break;
            }
            //Debug.Log(BFS_container.Count);
            //uDebug.Log("-------");
        }
    }

    public List<PointNormal> GetPath(PointNormal start,PointNormal end)
    {
        bfs(start, end);
        List<PointNormal> Path = new List<PointNormal>();
        PointNormal temp = end;
        while (temp != null)
        {
            Path.Add(temp);
            temp = temp.parent;
        }
        Path.Reverse();
        closeList.Clear();
        BFS_container.Clear();
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
    //判断点是否在队列中，为什么不用Contain,因为我们使用的new去创建的Pointdfs,两个x,y一样的Pointdfs的地址不同
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
                //if (i == 0 && j == 0) continue;//自身则跳过
                if (isInQueue(BFS_container, new PointNormal(curPoint.x + i, curPoint.y + j)) != null
                    || isInList(closeList, new PointNormal(curPoint.x + i, curPoint.y + j)) != null) continue;//已经检测过或已经在队列中就跳过
                if (curPoint.x + i < 0 || curPoint.x + i > Size - 1 || curPoint.y + j < 0 || curPoint.y + j > Size - 1 || map[curPoint.x + i, curPoint.y + j] == 1)
                {
                    //边界检查及是否为墙
                    continue;
                }
                //if (map[curPoint.x + i + 1, curPoint.y + j] == 1 && map[curPoint.x + i, curPoint.y + j + 1] == 1) continue;
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

