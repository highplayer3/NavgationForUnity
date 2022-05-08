using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CPPSTL;

public class PointforDij
{
    public int x, y;
    public PointforDij parent;
    public float G;
    public PointforDij(int _x,int _y)
    {
        this.x = _x;
        this.y = _y;
        parent = null;
        G = 0;
    }
}
/// <summary>
/// 迪杰斯特算法就是根据G值（从起点走到当前点的消费）来进行判断，
/// </summary>
public class Dijkstra
{
    private int[,] map;
    private int Size;

    List<PointforDij> openList = new List<PointforDij>();
    List<PointforDij> closeList = new List<PointforDij>();
    public List<PointforDij> GizmosList;

    public void InitMap(int[,] arr)
    {
        this.map = arr;

    }
    PointforDij FindLeastG()
    {
        if (openList.Count > 0)
        {
            PointforDij temp = openList[0];
            foreach(var p in openList)
            {
                if (p.G < temp.G)
                {
                    temp = p;
                }
            }
            return temp;

        }
        return null;
    }//找出openList里最小的G值
    float CalculateG(PointforDij start, PointforDij tarpoint)
    {
        float tempG = Vector2.Distance(new Vector2(start.x, start.y), new Vector2(tarpoint.x, tarpoint.y));
        float parentG = tarpoint.parent == null ? 0 : tarpoint.parent.G;
        float g = tempG + parentG;
        return g;
    }//计算G值

    PointforDij isInList(List<PointforDij> list, PointforDij point)
    {
        foreach (var p in list)
        {
            if (p.x == point.x && p.y == point.y)
                return p;
        }
        return null;
    }

    List<PointforDij> GetSurrounded(PointforDij curPoint)
    {
        List<PointforDij> surround = new List<PointforDij>();
        Size = MapController.instance.mapSize;
        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                if (i == 0 && j == 0) continue;//跳过本身
                if (curPoint.x + i < 0 || curPoint.x + i > Size - 1 || curPoint.y + j < 0 || curPoint.y + j > Size - 1 || map[curPoint.x + i, curPoint.y + j] == 1 || isInList(closeList, new PointforDij(curPoint.x + i, curPoint.y + j)) != null)
                {
                    continue;
                }
                if (i == -1 && j == -1)
                {
                    if (map[curPoint.x + i + 1, curPoint.y + j] == 1 && map[curPoint.x + i, curPoint.y + j + 1] == 1) continue;
                }
                if (i == 1 && j == -1)
                {
                    if (map[curPoint.x + i - 1, curPoint.y + j] == 1 && map[curPoint.x + i, curPoint.y + j + 1] == 1) continue;
                }
                if (i == 1 && j == 1)
                {
                    if (map[curPoint.x + i - 1, curPoint.y + j] == 1 && map[curPoint.x + i, curPoint.y + j - 1] == 1) continue;
                }
                if (i == -1 && j == 1)
                {
                    if (map[curPoint.x + i, curPoint.y + j - 1] == 1 && map[curPoint.x + i + 1, curPoint.y + j] == 1) continue;
                }
                surround.Add(new PointforDij(curPoint.x + i, curPoint.y + j));
            }
        }
        return surround;

    }

    PointforDij dijkstra(PointforDij start,PointforDij end)
    {
        openList.Add(start);
        while (openList.Count > 0)
        {
            PointforDij curPoint = FindLeastG();
            //PointforDij curPoint = FindLeastGPro();
            openList.Remove(curPoint);
            closeList.Add(curPoint);
            List<PointforDij> Surrounds = GetSurrounded(curPoint);
            foreach(var p in Surrounds)
            {
                if (isInList(openList, p) == null)//p没在openList中
                {
                    p.parent = curPoint;
                    p.G = CalculateG(curPoint, p);
                    openList.Add(p);
                }
                else//已经在openList中则更新G值
                {
                    PointforDij temp = isInList(openList, p);
                    float tempG = CalculateG(p,curPoint);
                    if (tempG < temp.G)
                    {
                        p.parent = curPoint;
                        p.G = tempG;
                    }
                }
            }
            if (isInList(openList, end) != null)
            {
                GizmosList = new List<PointforDij>(openList);
                foreach(var p in closeList)
                {
                    GizmosList.Add(p);
                }
                return isInList(openList, end);
            }
        }
        return null;

    }

    public List<PointforDij> Getpath(PointforDij start,PointforDij end)
    {
        List<PointforDij> Path = new List<PointforDij>();
        PointforDij res = dijkstra(start, end);
        while (res != null)
        {
            Path.Add(res);
            res = res.parent;
        }
        Path.Reverse();
        openList.Clear();
        closeList.Clear();
        return Path;
    }

}