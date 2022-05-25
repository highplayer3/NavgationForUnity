using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CPPSTL;



public class Point
{
    public int x, y;
    public float F, G, H;
    public Point parent;
    public Point(int _x,int _y)
    {
        this.x = _x;
        this.y = _y;
        F = 0;
        G = 0;
        H = 0;
    }
}

public class Astar
{

    private int Size;
    public int[,] map;
    List<Point> openList = new List<Point>();
    PriorityQueue<Point> PQ = new PriorityQueue<Point>(new MyCom(), new MyComPosX(), new MyComPosY());
    List<Point> closeList = new List<Point>();
    public List<Point> GizmosList;

    public void initMap(int[,] arr)
    {
        this.map = arr;
    }

    float CalculateG(Point start,Point tarpoint)
    {
        float tempG = Vector2.Distance(new Vector2(start.x, start.y), new Vector2(tarpoint.x, tarpoint.y));
        float parentG = tarpoint.parent == null ? 0 : tarpoint.parent.G;
        float g = tempG + parentG;
        return g;
    }//计算G值

    float CalculateH(Point point,Point end)
    {
        return (Mathf.Abs(end.x - point.x) + Mathf.Abs(end.y - point.y));//曼哈顿距离

    }//计算H值

    float CalculateF(Point point)
    {
        return point.G + point.H;
    }//计算F值

    Point FindleastF()
    {
        if (openList.Count > 0)
        {
            Point curPoint = openList[0];
            foreach(Point point in openList)
            {
                if (point.F < curPoint.F)
                {
                    curPoint = point;
                }
            }
            return curPoint;
        }
        return null;
    }//通过遍历去找到最小的F

    Point FindleastFPro()
    {
        return PQ.Top();
    }

    Point findPath(Point startPoint,Point endPoint)
    {
        openList.Add(startPoint);
        while (openList.Count > 0)
        {
            Point curPoint = FindleastF();//获取当前openList中F值最小的点
            openList.Remove(curPoint);//将当前点从OpenList中移除
            closeList.Add(curPoint);//添加到闭表中
            List<Point> surroundPoints = GetSurroundPoint(curPoint);//获取周围的点

            foreach(var target in surroundPoints)
            {
                if (isInList(openList, target) == null)//若周围点不在openList中，则加入openList,并计算点的G,F,H值
                {
                    target.parent = curPoint;
                    target.G = CalculateG(target, curPoint);
                    target.H = CalculateH(target, endPoint);
                    target.F = CalculateF(target);

                    openList.Add(target);
                }
                else//已经在openList中，则更新其G值和F值，因为H值与parent无关，不用管
                {
                    Point temp = isInList(openList, target);
                    float tempG = CalculateG(target, curPoint);
                    if (tempG < temp.G)
                    {
                        target.parent = curPoint;
                        target.G = tempG;
                        target.F = CalculateF(target);
                    }
                } 
            }
            Point resPoint = isInList(openList, endPoint);//如果目标点在openList中，则说明找到了路径
            if (resPoint != null)
            {
                GizmosList = new List<Point>(openList);
                foreach(var p in closeList)
                {
                    GizmosList.Add(p);
                }
                //在地图上画出搜索路径
                return resPoint;
            }
        }
        return null;//返回null就说明没有路径（目标点不可达）
    }

    public List<Point> GetPath(Point start,Point end)
    {
        Point result = findPath(start, end);
        List<Point> Path = new List<Point>();
        while (result != null)
        {
            Path.Add(result);
            result = result.parent;
        }
        Path.Reverse();
        openList.Clear();
        //openlist一旦清除，GizmosList也将被清除，因为前面赋值操作仅仅让GizmosList指向了openList,也就是深拷贝，但我们希望其浅拷贝。
        closeList.Clear();
        return Path;
    }

    Point isInList(List<Point> list, Point point)
    {
        foreach (var p in list)
        {
            if (p.x == point.x && p.y == point.y)
                return p;
        }
        return null;
    }

    List<Point> GetSurroundPoint(Point curPoint)
    {
        Size = MapController.instance.mapSize;
        List<Point> surPoint = new List<Point>();
        for(int i = -1; i <= 1; i++)
        {
            for(int j = -1; j <= 1; j++)//
            {
                if (i == 0 && j == 0) continue;//跳过本身
                if (curPoint.x + i < 0 || curPoint.x + i > Size - 1 || curPoint.y + j < 0 || curPoint.y + j > Size - 1 || map[curPoint.x+i,curPoint.y+j]==1 || isInList(closeList, new Point(curPoint.x + i, curPoint.y + j)) !=null )                 //|| isInList(closeList,new Point(curPoint.x+i,curPoint.y+j))!=null)
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
                surPoint.Add(new Point(curPoint.x + i, curPoint.y + j));
            }
        }
        return surPoint;
    }

}
 
