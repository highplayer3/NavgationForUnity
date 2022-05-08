using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    public GameObject playerPrefab;
    GameObject Player;
    [Header("A*寻路")]
    private Astar astar=new Astar();
    List<Point> path = new List<Point>();
    Point begin,destination;
    [Header("DFS")]
    private DFS dFS=new DFS();
    private List<PointNormal> pathDFS = new List<PointNormal>();
    PointNormal destinationDFS;
    [Header("BFS")]
    private BFS bFS = new BFS();
    private List<PointNormal> pathBFS = new List<PointNormal>();
    PointNormal destinationBFS;
    [Header("Dijkstra")]
    private Dijkstra dij = new Dijkstra();
    private List<PointforDij> pathDij = new List<PointforDij>();
    PointforDij destinationDij;
    [Header("双向BFS")]
    private BinaryBFS bBFS = new BinaryBFS();
    private List<PointNormal> pathDBFS = new List<PointNormal>();
    //委托来实现动态函数调用
    private bool flag = false;
    private delegate void NavDelegate();
    NavDelegate navDelegate;//委托变量

    private void Start()
    {
        PlayerInit();
        astar.initMap(MapController.instance.mp);
        dFS.initMap(MapController.instance.mp);
        bFS.initMap(MapController.instance.mp);
        dij.InitMap(MapController.instance.mp);
        bBFS.initMap(MapController.instance.mp);
        //for(int i = 0; i < MapController.instance.mapSize; i++)
        //{
        //    for(int j = 0; j < MapController.instance.mapSize; j++)
        //    {
        //        print(astar.map[i, j]);
        //    }
        //}
        //print(astar.map == MapController.instance.mp);
        navDelegate = Navigation;//默认为A*寻路
    }

    private void Update()
    {
        NavManager();
        //NavigationforDBFS();
    }

    private void NavManager()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            navDelegate = Navigation;
            flag = true;
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            navDelegate = NavigationforDFS;
            flag = true;
        }
        else if (Input.GetKeyDown(KeyCode.B))
        {
            navDelegate = NavigationforBFS;
            flag = true;
        }
        else if (Input.GetKeyDown(KeyCode.I))
        {
            navDelegate = NavigationDij;
            flag = true;
        }else if (Input.GetKeyDown(KeyCode.F))
        {
            navDelegate = NavigationforDBFS;
            flag = true;
        }
        if (flag == true) ClearGizmos();
        navDelegate();//调用函数
    }

    IEnumerator Move1()
    {
        for(int i=0;i<path.Count;)
        {
            //Player.transform.Translate(new Vector3(path[i].x, Player.transform.position.y, path[i].y) - Player.transform.position);
            Player.transform.position = Vector3.MoveTowards(Player.transform.position, new Vector3(path[i].x, Player.transform.position.y, path[i].y), 0.01f*Time.deltaTime);
            if(Player.transform.position== new Vector3(path[i].x, Player.transform.position.y, path[i].y))
            {
                i++;
                yield return null;
            }
            //yield return null;
        }
        
    }

    IEnumerator Move2()
    {
        for(int i = 0; i < pathDFS.Count; i++)
        {
            Vector3 target = new Vector3(pathDFS[i].x, 1.0f, pathDFS[i].y);
            while (Vector3.Distance(Player.transform.position,target)>0.001f)
            {
                Player.transform.position = Vector3.MoveTowards(Player.transform.position, target, 0.1f * Time.deltaTime);
            }
            yield return null;
        }
    }//TODO:移动有问题，不流畅 resolve:yield return new WaitForSeconds换为null,但不知道原因

    private void Navigation()
    {
        if (Input.GetMouseButtonDown(0))
        {
            flag = false;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if(Physics.Raycast(ray,out hit))
            {
                if (hit.collider.gameObject.CompareTag("Plane"))
                {
                    destination = new Point((int)Mathf.Round(hit.point.x),(int)Mathf.Round(hit.point.z));
                    begin = new Point((int)Mathf.Round(Player.transform.position.x), (int)Mathf.Round(Player.transform.position.z));
                    path.Clear();

                    path = astar.GetPath(begin, destination);
                }
            }
        }
        
    }//Astar

    private void NavigationDij()
    {
        if (Input.GetMouseButtonDown(0))
        {
            flag = false;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.gameObject.CompareTag("Plane"))
                {
                    destinationDij = new PointforDij((int)Mathf.Round(hit.point.x), (int)Mathf.Round(hit.point.z));
                    PointforDij begin = new PointforDij((int)Mathf.Round(Player.transform.position.x), (int)Mathf.Round(Player.transform.position.z));
                    pathDij.Clear();
                    pathDij = dij.Getpath(begin, destinationDij);
                }
            }
        }
    }//Dijkstra

    private void NavigationforDFS()
    {
        if (Input.GetMouseButtonDown(0))
        {
            flag = false;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.gameObject.CompareTag("Plane"))
                {
                    destinationDFS = new PointNormal((int)Mathf.Round(hit.point.x), (int)Mathf.Round(hit.point.z));
                    PointNormal beginDFS = new PointNormal((int)Mathf.Round(Player.transform.position.x), (int)Mathf.Round(Player.transform.position.z));
                    pathDFS.Clear();
                    pathDFS=dFS.GetPath(beginDFS, destinationDFS);
                }
            }
            //foreach(var p in pathDFS)
            //{
            //    print(p.x + " " + p.y);
            //    print("-------");
            //}
        }
    }//DFS

    private void NavigationforBFS()
    {
        if (Input.GetMouseButtonDown(0))
        {
            flag = false;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.gameObject.CompareTag("Plane"))
                {
                    destinationBFS = new PointNormal((int)Mathf.Round(hit.point.x), (int)Mathf.Round(hit.point.z));
                    PointNormal beginBFS = new PointNormal((int)Mathf.Round(Player.transform.position.x), (int)Mathf.Round(Player.transform.position.z));
                    pathBFS.Clear();
                    if(bFS.Gizmosqueue!=null) bFS.Gizmosqueue.Clear();
                    pathBFS = bFS.GetPath(beginBFS, destinationBFS);
                }
            }
            //foreach(var p in pathBFS)
            //{
            //    print(p.x + " " + p.y);
            //    print("-------");
            //}
        }
    }//BFS

    private void NavigationforDBFS()
    {
        if (Input.GetMouseButtonDown(0))
        {
            flag = false;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.gameObject.CompareTag("Plane"))
                {
                    PointNormal destinationDBFS = new PointNormal((int)Mathf.Round(hit.point.x), (int)Mathf.Round(hit.point.z));
                    PointNormal beginDBFS = new PointNormal((int)Mathf.Round(Player.transform.position.x), (int)Mathf.Round(Player.transform.position.z));
                    pathDBFS.Clear();
                    pathDBFS = bBFS.GetPath(beginDBFS, destinationDBFS);
                }
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (Player != null)
        {
            Gizmos.color = Color.blue;
            Vector3 PlayerPos = new Vector3(Mathf.Round(Player.transform.position.x), 0.05f, Mathf.Round(Player.transform.position.z));
            Gizmos.DrawCube(PlayerPos, new Vector3(0.9f, 0f, 0.9f));
        }

        if (astar.GizmosList!=null&&astar.GizmosList.Count > 0)
        {
            Gizmos.color = Color.blue;
            foreach(var po in astar.GizmosList)
            {
                Gizmos.DrawCube(new Vector3(po.x, 0.008f, po.y), new Vector3(0.9f, 0f, 0.9f));
            }
        }
        if (path.Count > 0)
        {
            Gizmos.color = Color.yellow;
            foreach(var p in path)
            {
                Gizmos.DrawCube(new Vector3(p.x, 0.01f, p.y), new Vector3(0.9f, 0f, 0.9f));
            }
        }

        if (dij.GizmosList != null && dij.GizmosList.Count > 0)
        {
            Gizmos.color = Color.blue;
            foreach (var po in dij.GizmosList)
            {
                Gizmos.DrawCube(new Vector3(po.x, 0.008f, po.y), new Vector3(0.9f, 0f, 0.9f));
            }
        }
        if (pathDij.Count > 0)
        {
            Gizmos.color = Color.yellow;
            foreach (var p in pathDij)
            {
                Gizmos.DrawCube(new Vector3(p.x, 0.01f, p.y), new Vector3(0.9f, 0f, 0.9f));
            }
        }

        if (dFS.Gizmosstack!=null&&dFS.Gizmosstack.Count > 0)
        {
            Gizmos.color = Color.blue;
            foreach(var p in dFS.Gizmosstack)
            {
                Gizmos.DrawCube(new Vector3(p.x, 0.001f, p.y), new Vector3(0.9f, 0f, 0.9f));
            }
        }
        if (pathDFS.Count > 0)
        {
            Gizmos.color = Color.yellow;
            foreach (var p in pathDFS)
            {
                Gizmos.DrawCube(new Vector3(p.x, 0.01f, p.y), new Vector3(0.9f, 0f, 0.9f));
            }
        }

        if (bFS.Gizmosqueue != null && bFS.Gizmosqueue.Count > 0)
        {
            Gizmos.color = Color.blue;
            foreach (var p in bFS.Gizmosqueue)
            {
                Gizmos.DrawCube(new Vector3(p.x, 0.001f, p.y), new Vector3(0.9f, 0f, 0.9f));
            }
        }
        if (pathBFS.Count > 0)
        {
            Gizmos.color = Color.yellow;
            foreach(var p in pathBFS)
            {
                Gizmos.DrawCube(new Vector3(p.x, 0.01f, p.y), new Vector3(0.9f, 0f, 0.9f));
            }
        }

        if (bBFS.GizmosList1 != null && bBFS.GizmosList1.Count > 0)
        {
            Gizmos.color = Color.blue;
            foreach(var p in bBFS.GizmosList1)
            {
                Gizmos.DrawCube(new Vector3(p.x, 0.001f, p.y), new Vector3(0.9f, 0f, 0.9f));
            }
        }
        if (bBFS.GizmosList2 != null && bBFS.GizmosList2.Count > 0)
        {
            Gizmos.color = Color.green;
            foreach (var p in bBFS.GizmosList2)
            {
                Gizmos.DrawCube(new Vector3(p.x, 0.001f, p.y), new Vector3(0.9f, 0f, 0.9f));
            }
        }
        if (pathDBFS.Count > 0)
        {
            Gizmos.color = Color.yellow;
            foreach(var p in pathDBFS)
            {
                Gizmos.DrawCube(new Vector3(p.x, 0.01f, p.y), new Vector3(0.9f, 0f, 0.9f));
            }
        }
    }//绘制路线

    private void ClearGizmos()
    {
        if (astar.GizmosList != null)
        {
            path.Clear();
            astar.GizmosList.Clear();
        }
        if (bFS.Gizmosqueue != null)
        {
            bFS.Gizmosqueue.Clear();
            pathBFS.Clear();
        }
        if (dFS.Gizmosstack != null)
        {
            dFS.Gizmosstack.Clear();
            pathDFS.Clear();
        }
        if (dij.GizmosList != null)
        {
            dij.GizmosList.Clear();
            pathDij.Clear();
        }
        if (bBFS.GizmosList1 != null && bBFS.GizmosList2 != null)
        {
            bBFS.GizmosList1.Clear();
            bBFS.GizmosList2.Clear();
            pathDBFS.Clear();
        }
    }

    private void PlayerInit()
    {
        GameObject map = GameObject.Find("Map");
        Player = Instantiate(playerPrefab);
        Player.transform.SetParent(map.transform);
        Player.name = playerPrefab.name;
        Player.transform.position = new Vector3(0, 1, 0);
        Player.transform.rotation = Quaternion.Euler(0, -90f, 0);
    }
}
