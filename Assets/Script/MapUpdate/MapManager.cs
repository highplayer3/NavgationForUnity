using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    public int[,] grid;
    public GameObject[,] gridObj;
    public int mapSize = 10;
    public GameObject cubePrefab;
    // Start is called before the first frame update
    void Start()
    {
        grid = new int[mapSize, mapSize];
        gridObj = new GameObject[mapSize, mapSize];
        MapCreate();
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            RefreahMap();
        }
    }
    void RefreahMap()
    {
        int[,] newGrid = (int[,])grid.Clone();
        for (int i = 0; i < mapSize; i++)
        {
            for (int j = 0; j < mapSize; j++)
            {
                if (grid[i, j] == 1)
                {
                    if (GetWallNumber(i, j) >= 3)
                    {

                    }
                    else
                    {
                        newGrid[i, j] = 0;
                    }
                }
                else
                {
                    if (GetWallNumber(i, j) >= 5)
                    {
                        newGrid[i, j] = 1;
                    }
                }
                
            }
        }
        Check(newGrid);
    }
    private void Check(int[,] newGrid)
    {
        for(int i = 0; i < mapSize; i++)
        {
            for(int j = 0; j < mapSize; j++)
            {
                if (newGrid[i, j] != grid[i, j])
                {
                    grid[i, j] = newGrid[i, j];
                    if (newGrid[i, j] == 1)
                    {
                        gridObj[i, j] = Instantiate(cubePrefab, transform.position + new Vector3(i, 0, j), Quaternion.identity);
                    }
                    else
                    {
                        Destroy(gridObj[i, j]);
                    }
                }
            }
        }
    }
    

    private int GetWallNumber(int Posx,int Posy)
    {
        int num = 0;
        for(int i = -1; i <= 1; i++)
        {
            for(int j = -1; j <= 1; j++)
            {
                if (i == 0 && j == 0) continue;//两个都为零时，代表自身
                if (Posx+i < 0 || Posx+i >= mapSize - 1 || Posy+j < 0 || Posy+j >= mapSize - 1)
                {
                    num++;
                    continue;
                }
                if (grid[Posx + i, Posy + j] == 1) num++;
            }
        }
        return num;
    }
    void MapCreate()
    {
        for(int i = 0; i < mapSize; i++)
        {
            for(int j = 0; j < mapSize; j++)
            {
                if (Random.Range(0, 100) < 45)
                {
                    grid[i, j] = 1;
                }
                else
                {
                    grid[i, j] = 0;
                }
                if (grid[i, j] == 1)
                {
                    gridObj[i, j] = Instantiate(cubePrefab, transform.position + new Vector3(i, 0, j), Quaternion.identity);
                }
            }
        }
    }
    
}
