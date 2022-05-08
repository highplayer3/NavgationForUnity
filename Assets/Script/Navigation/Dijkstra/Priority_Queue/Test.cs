using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CPPSTL;

public class Test : MonoBehaviour
{
    PriorityQueue<Point> q = new PriorityQueue<Point>(new MyCom(),new MyComPosX(),new MyComPosY());
    // Start is called before the first frame update
    void Start()
    {
        Point t = new Point(1, 2);
        t.F = 3;
        Point t1 = new Point(2, 3);
        t1.F = 5;
        Point t2 = new Point(2, 4);
        t2.F = 7;
        Point t3 = new Point(3, 8);
        t3.F = 1;
        Point t4 = new Point(1, 7);
        t4.F = 9;
        q.Push(t);
        q.Push(t1);
        q.Push(t2);
        q.Push(t3);
        q.Push(t4);
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Point t5 = new Point(3, 3);
            t5.F= 11;
            Point t6 = new Point(4, 4);
            t6.F = 12;
            q.Push(t5);
            q.Push(t6);
            foreach(var p in q)
            {
                Debug.Log(p.x + " " + p.y + "--" + p.F);
            }
            t5.F = 0;
            t6.F = 0;
            q.UpdateHeap(t5);
            Debug.Log("---------");
            foreach (var p in q)
            {
                Debug.Log(p.x + " " + p.y + "--" + p.F);
            }
        }
        
    }

    
}
