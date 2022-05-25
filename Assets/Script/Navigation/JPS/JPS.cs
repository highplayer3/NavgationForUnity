using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JPS 
{
    private int Size;
    public int[,] map;
    private List<Point> openList = new List<Point>();
    private List<Point> closeList = new List<Point>();
    public List<Point> GizmosListForline = new List<Point>();
    private Point destination;
    public Point Destination
    {
        get
        {
            return destination;
        }
        set
        {
            Size = MapController.instance.mapSize;
            if (value.x < 0 || value.x > Size - 1 || value.y < 0 || value.y > Size - 1 || map[value.x, value.y] == 1)
            {
                Debug.Log("设置错误");
            }
            else
            {
                destination = value;
            }
        }
    }


    public void initMap(int[,] arr)
    {
        this.map = arr;
    }

    float CalculateG(Point start, Point tarpoint)
    {
        float tempG = Vector2.Distance(new Vector2(start.x, start.y), new Vector2(tarpoint.x, tarpoint.y));
        float parentG = tarpoint.parent == null ? 0 : tarpoint.parent.G;
        float g = tempG + parentG;
        return g;
    }//计算G值

    float CalculateH(Point point, Point end)
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
            foreach (Point point in openList)
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

    private bool isWalkable(float x,float y)
    {
        Size = MapController.instance.mapSize;
        if (x < 0 || x > Size - 1 || y < 0 || y > Size - 1)
        {
            return false;
        }
        if (map[(int)x, (int)y] == 1)
        {
            return false;
        }
        return true;
    }

    //private Point CheckJumpPoint(Point currentPt,Point prePoint)
    //{
    //    Vector3 dir = new Vector2((currentPt.x - prePoint.x), (currentPt.y - prePoint.y));
    //    Point res = new Point(-1,-1);
    //    if (!isWalkable(currentPt.x, currentPt.y))
    //    {
    //        return res;
    //    }
    //    if (currentPt.x == desination.x && currentPt.y == desination.y)
    //    {
    //        return currentPt;
    //    }
    //    //斜向判断是否为跳点
    //    if (dir.x != 0 && dir.y != 0)
    //    {
    //        if((!isWalkable(currentPt.x-dir.x,currentPt.y+dir.y)
    //            &&isWalkable(currentPt.x-dir.x,currentPt.y))
    //            ||(!isWalkable(currentPt.x+dir.x,currentPt.y-dir.y)
    //            && isWalkable(currentPt.x, currentPt.y - dir.y)))
    //        {
    //            return currentPt;
    //        }
    //    }
    //    else
    //    {
    //        if (dir.x != 0)
    //        {
    //            if ((!isWalkable(currentPt.x + dir.x, currentPt.y + 1)
    //                &&isWalkable(currentPt.x,currentPt.y+1))
    //                ||(!isWalkable(currentPt.x+dir.x,currentPt.y-dir.y)
    //                && isWalkable(currentPt.x, currentPt.y - dir.y)))
    //            {
    //                return currentPt;
    //            }
    //        }
    //        else
    //        {
    //            if ((!isWalkable(currentPt.x-1, currentPt.y + dir.y)
    //                && isWalkable(currentPt.x-1, currentPt.y))
    //                || (!isWalkable(currentPt.x+1, currentPt.y+dir.y)
    //                && isWalkable(currentPt.x+1, currentPt.y)))
    //            {
    //                return currentPt;
    //            }
    //        }
    //    }
    //    if (dir.x != 0 && dir.y != 0)
    //    {
    //        res = CheckJumpPoint(new Point(currentPt.x +(int)dir.x, currentPt.y), currentPt);
    //        Point temp = CheckJumpPoint(new Point(currentPt.x, currentPt.y + (int)dir.y), currentPt);
    //        if (res.x != -1 && temp.x != -1)
    //        {
    //            return currentPt;
    //        }
    //    }
    //    if(!isWalkable(currentPt.x+dir.x,currentPt.y)
    //        || !isWalkable(currentPt.x, currentPt.y + dir.y))
    //    {
    //        res = CheckJumpPoint(new Point(currentPt.x + (int)dir.x, currentPt.y + (int)dir.y), currentPt);
    //        if (res.x != -1)
    //        {
    //            return res;
    //        }
    //    }
    //    return res;
    //}//返回跳点

    private Point LineSearch(Point current,Vector2 dir)//直线搜索
    {
        if (dir.magnitude == 0)
        {
            Debug.Log("Error!");
            return null;
        }
        Point temp = new Point(current.x +(int)dir.x, current.y +(int)dir.y);
        while (true)
        {
            //跳点定义①：终点是跳点
            if (temp.x == destination.x && temp.y == destination.y)
            {
                return temp;
            }
            if (!isWalkable(temp.x, temp.y))//一旦遇到障碍物或超出地图范围就退出循环
            {
                break;
            }
            if (dir.x != 0&&dir.y==0)//沿X方向前进时的跳点判断
            {
                //跳点定义②：有强迫邻居的点是跳点
                if((!isWalkable(temp.x,temp.y+1)&&isWalkable(temp.x+dir.x,temp.y+1)&&isWalkable(temp.x+dir.x,temp.y))
                    || (!isWalkable(temp.x, temp.y - 1) && isWalkable(temp.x + dir.x, temp.y - 1)&&isWalkable(temp.x+dir.x,temp.y)))
                {
                    //Debug.Log("X轴的跳点");
                    return temp;//该点就是跳点
                }
            }
            if(dir.y!=0&&dir.x==0)//沿Y方向前进时的跳点判断
            {
                if ((!isWalkable(temp.x+1, temp.y) && isWalkable(temp.x+1, temp.y+dir.y)&&isWalkable(temp.x,temp.y+dir.y))
                    || (!isWalkable(temp.x-1, temp.y) && isWalkable(temp.x-1, temp.y+dir.y)&&isWalkable(temp.x,temp.y+dir.y)))
                {
                    //Debug.Log("Y轴的跳点");
                    return temp;//该点就是跳点
                }
            }
            temp = new Point(temp.x + (int)dir.x, temp.y + (int)dir.y);
            //Debug.Log(temp.x + " " + temp.y);
        }
        return null;
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

    private void StraightSearch(Point _curPoint)//以当前点的4条直线搜索
    {
        Point ans1 = LineSearch(_curPoint, new Vector2(1, 0));
        if (ans1 != null&&isInList(closeList,ans1)==null)
        {
            ans1.parent = _curPoint;
            ans1.G = CalculateG(ans1, _curPoint);
            ans1.H = CalculateH(ans1, destination);
            ans1.F = CalculateF(ans1);
            openList.Add(ans1);
        }
        Point ans2 = LineSearch(_curPoint, new Vector2(0, 1));
        if (ans2 != null&&isInList(closeList,ans2)==null)
        {
            ans2.parent = _curPoint;
            ans2.G = CalculateG(ans2, _curPoint);
            ans2.H = CalculateH(ans2, destination);
            ans2.F = CalculateF(ans2);
            openList.Add(ans2);
        }
        Point ans3 = LineSearch(_curPoint, new Vector2(-1, 0));
        if (ans3 != null&&isInList(closeList,ans3)==null)
        {
            ans3.parent = _curPoint;
            ans3.G = CalculateG(ans3, _curPoint);
            ans3.H = CalculateH(ans3, destination);
            ans3.F = CalculateF(ans3);
            openList.Add(ans3);
        }
        Point ans4 = LineSearch(_curPoint, new Vector2(0, -1));
        if (ans4 != null&&isInList(closeList,ans4)==null)
        {
            ans4.parent = _curPoint;
            ans4.G = CalculateG(ans4, _curPoint);
            ans4.H = CalculateH(ans4, destination);
            ans4.F = CalculateF(ans4);
            openList.Add(ans4);
        }
    }

    private Point LineSearch2(Point _curPoint,Vector2 dir)//斜向搜索
    {
        if (dir.magnitude == 0)
        {
            Debug.Log("Error");
            return null;
        }
        Point temp = new Point(_curPoint.x + (int)dir.x, _curPoint.y + (int)dir.y);
        if (!isWalkable(temp.x, temp.y)) return null;
        if (dir.x > 0 && dir.y > 0)//往右上方搜索
        {
            //跳点定义②：有强迫邻居的点是跳点
            if((!isWalkable(temp.x,temp.y-dir.y)&&isWalkable(temp.x+1,temp.y-dir.y)&&isWalkable(temp.x+1,temp.y))
                || (!isWalkable(temp.x - dir.x, temp.y) && isWalkable(temp.x - dir.x, temp.y + 1) && isWalkable(temp.x, temp.y + 1)))
            {
                return temp;
            }
            //跳点定义③：沿当前方向的水平和垂直分量有满足定义①和②的点时跳点
            Point SuspiciousJP = LineSearch(temp, new Vector2(dir.x, 0));
            Point SuspiciousJP2 = LineSearch(temp, new Vector2(0, dir.y));
            if (SuspiciousJP!=null||SuspiciousJP2!=null)
            {
                return temp;
            }
        }else if (dir.x > 0 && dir.y < 0)//沿着右下方搜索
        {
            if ((!isWalkable(temp.x, temp.y - dir.y) && isWalkable(temp.x + 1, temp.y - dir.y) && isWalkable(temp.x + 1, temp.y))
                || (!isWalkable(temp.x - dir.x, temp.y) && isWalkable(temp.x - dir.x, temp.y - 1) && isWalkable(temp.x, temp.y - 1)))
            {
                return temp;
            }
            //跳点定义③：沿当前方向的水平和垂直分量有满足定义①和②的点时跳点
            Point SuspiciousJP = LineSearch(temp, new Vector2(dir.x, 0));
            Point SuspiciousJP2 = LineSearch(temp, new Vector2(0, dir.y));
            if (SuspiciousJP != null || SuspiciousJP2 != null)
            {
                return temp;
            }
        }else if (dir.x < 0 && dir.y > 0)//向左上方搜索
        {
            if((!isWalkable(temp.x-dir.x,temp.y)&&isWalkable(temp.x-dir.x,temp.y+1)&&isWalkable(temp.x,temp.y+1))
                || (!isWalkable(temp.x, temp.y - dir.y) && isWalkable(temp.x - 1, temp.y - dir.y) && isWalkable(temp.x - 1, temp.y)))
            {
                return temp;
            }
            Point SuspiciousJP = LineSearch(temp, new Vector2(dir.x, 0));
            Point SuspiciousJP2 = LineSearch(temp, new Vector2(0, dir.y));
            if (SuspiciousJP != null || SuspiciousJP2 != null)
            {
                return temp;
            }
        }else if (dir.x < 0 && dir.y < 0)
        {
            if ((!isWalkable(temp.x - dir.x, temp.y) && isWalkable(temp.x - dir.x, temp.y - 1) && isWalkable(temp.x, temp.y-1))
                || (!isWalkable(temp.x, temp.y - dir.y) && isWalkable(temp.x - 1, temp.y - dir.y) && isWalkable(temp.x - 1, temp.y)))
            {
                return temp;
            }
            Point SuspiciousJP = LineSearch(temp, new Vector2(dir.x, 0));
            Point SuspiciousJP2 = LineSearch(temp, new Vector2(0, dir.y));
            if (SuspiciousJP != null || SuspiciousJP2 != null)
            {
                return temp;
            }
        }
        return null;
    }

    private void DiagonalSearch(Point _curPoint)
    {
        Point ans1 = LineSearch2(_curPoint, new Vector2(1, 1));
        if (ans1 != null&&isInList(closeList, ans1) == null)
        {
            ans1.parent = _curPoint;
            ans1.G = CalculateG(ans1,_curPoint );
            ans1.H = CalculateH(ans1, destination);
            ans1.F = CalculateF(ans1);
            openList.Add(ans1);
        }
        Point ans2 = LineSearch2(_curPoint, new Vector2(1, -1));
        if (ans2 != null&& isInList(closeList, ans2) == null)
        {
            ans2.parent = _curPoint;
            ans2.G = CalculateG(ans2, _curPoint);
            ans2.H = CalculateH(ans2, destination);
            ans2.F = CalculateF(ans2);
            openList.Add(ans2);
        }
        Point ans3 = LineSearch2(_curPoint, new Vector2(-1, 1));
        if (ans3 != null&& isInList(closeList, ans3) == null)
        {
            ans3.parent = _curPoint;
            ans3.G = CalculateG(ans3, _curPoint);
            ans3.H = CalculateH(ans3, destination);
            ans3.F = CalculateF(ans3);
            openList.Add(ans3);
        }
        Point ans4 = LineSearch2(_curPoint, new Vector2(-1, -1));
        if (ans4 != null&& isInList(closeList, ans4) == null)
        {
            ans4.parent = _curPoint;
            ans4.G = CalculateG(ans4, _curPoint);
            ans4.H = CalculateH(ans4, destination);
            ans4.F = CalculateF(ans4);
            openList.Add(ans4);
        }
    }

    private Point JPS_search(Point start)
    {
        openList.Add(start);
        while (openList.Count > 0)
        {
            Point curPoint = FindleastF();
            openList.Remove(curPoint);
            //以当前点开始直线搜索寻找跳点
            StraightSearch(curPoint);
            DiagonalSearch(curPoint);
            closeList.Add(curPoint);
            Point resPoint = isInList(openList, destination);//如果目标点在openList中，则说明找到了路径
            if (resPoint != null)
            {
                return resPoint;
            }
        }
        return null;
    }
    public List<Point> GetPath(Point start)
    {
        Point res = JPS_search(start);
        List<Point> path = new List<Point>();
        while (res!= null)
        {
            path.Add(res);
            GizmosListForline.Add(res);
            res = res.parent;
        }
        openList.Clear();
        closeList.Clear();
        return path;
    }
}
