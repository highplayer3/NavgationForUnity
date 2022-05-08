using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coord
{
    public int x;
    public int y;
    public bool isWalk;//该点是否可走
    public Coord(int _x, int _y,bool _isWalk)
    {
        x = _x;
        y = _y;
        isWalk = _isWalk;
    }
    public  Coord SetisWlak(Coord coord,bool _IW)
    {
        coord.isWalk = _IW;
        return coord;
    }
    public static bool operator !=(Coord _c1, Coord _c2)
    {
        return !(_c1 == _c2);//相当于(_c1.x != _c2.x) || (_c1.y != _c2.y)
    }

    public static bool operator ==(Coord _c1, Coord _c2)
    {
        return (_c1.x == _c2.x) && (_c1.y == _c2.y);
    }
    //1.c#不允许重载 = ，但是可以重载+=，-=（C++里面这些运算符不属于一类）
    //2.c#要求成对的重载比较运算符，并且必须返回布尔类型值。比如重载了!=就必须重载==
}

public class MapController : MonoBehaviour
{
    [Header("Map parameter")]
    public GameObject tilePrefab;//Quad
    public int mapSize=50;//地图大小
    private List<Coord> allTileCoord;//将生成的瓦片放进列表，方便洗牌算法
    public Transform mapManager;//用来管理生成的tile
    //private int[,] mapInfo;//用一个二维数组存储生成的Map的信息

    [Header("Map Obstacles")]
    public GameObject obsPrefab;//Cube
    private Queue<Coord> shuffleQueue;//保存洗牌后的数据
    public int obstacleNumber=20;//障碍物的数量
    // Start is called before the first frame update
    [Header("Temporary Variab")]
    private Vector3 GizmosPos;
    public static MapController instance;
    public int[,] mp;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        mp = new int[mapSize, mapSize];
        allTileCoord = new List<Coord>();//别忘了初始化
        GenerateMap();//生成地图
        GenerateObstacle();//生成障碍物
    }

    private void GenerateMap()
    {
        for(int i = 0; i < mapSize; i++)
        {
            for(int j = 0; j < mapSize; j++)
            {
                Vector3 Pos = new Vector3(transform.position.x + i, transform.position.y, transform.position.z + j);
                GameObject spawnTile = Instantiate(tilePrefab, Pos, Quaternion.Euler(90,0,0));
                spawnTile.transform.SetParent(mapManager);
                allTileCoord.Add(new Coord(i, j,true));//注意结构体是值类型，在Update里使用new比较耗时。
                mp[i, j] = 0;
            }
        }
    }

    private Coord GetRandomCoord()
    {
        Coord randomCoord = shuffleQueue.Dequeue();//队列：先进先出
        shuffleQueue.Enqueue(randomCoord);//将移出的元素放在队列的最后一个，保证队列完整性，大小不变

        return randomCoord;
    }

    private void GenerateObstacle()
    {
        int obsCount = 0;//记录已经生成了多少个障碍物
        shuffleQueue = new Queue<Coord>(Coord_shuffle.Knuth(allTileCoord.ToArray()));//将洗牌后的数字转化后放进数组
        while (obsCount < obstacleNumber)
        {
            Coord randomCoord = GetRandomCoord();//取出一个洗牌后的坐标
            if (randomCoord.x == 0 && randomCoord.y == 0) continue;
            randomCoord.SetisWlak(randomCoord, false);
            mp[randomCoord.x, randomCoord.y] = 1;
            Vector3 obsPos = new Vector3(transform.position.x+randomCoord.x,0.5f,transform.position.z+randomCoord.y);
            GameObject spawnObstacle = Instantiate(obsPrefab, obsPos, Quaternion.identity);
            //mapInfo[randomCoord.x, randomCoord.y] = 1;
            spawnObstacle.transform.SetParent(mapManager);//将生成出来的障碍物进行统一管理
            obsCount++;//别忘了++，不然会死循环。
        }
    }

    private void OnDrawGizmos()
    {
        if (allTileCoord != null)
        {
            foreach (var Coord in allTileCoord)
            {
                GizmosPos = new Vector3(transform.position.x + Coord.x, transform.position.y, transform.position.z + Coord.y);
                Gizmos.color = Coord.isWalk == true ? Color.green : Color.red;
                Gizmos.DrawWireCube(GizmosPos, new Vector3(0.9f, 0, 0.9f));
            }
        }
    }
    
    /*private int[,] ListToArray(List<Coord> arr)
    {
        int[,] mapInfo= new int[mapSize, mapSize];
        foreach(Coord coord in arr)
        {
            if (coord.isWalk == true)
            {
                mapInfo[coord.x, coord.y] = 0;
            }
            else
            {
                mapInfo[coord.x, coord.y] = 1;
            }
        }
        return mapInfo;
    }//转化为0/1二维数组*/

    
}
