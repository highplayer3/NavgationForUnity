using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coord_shuffle 
{
    public static Coord[] Knuth(Coord[] dataArray)
    {
        for (int i = dataArray.Length - 1; i >= 0; i--)
        {
            int randNum = UnityEngine.Random.Range(0, i);
            Coord temp = dataArray[randNum];
            dataArray[randNum] = dataArray[i];
            dataArray[i] = temp;
        }
        return dataArray;
    }
}
