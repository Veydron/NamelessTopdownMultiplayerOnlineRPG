using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGrid
{
    private int width;
    private int height;
    private float cellSize;
    private int[,] gridArray;


    public MapGrid(int width, int height, float cellSize)
    {
        this.width = width;
        this.height = height;  
        this.cellSize = cellSize;

        gridArray  = new int[width, height];

        for (int x = 0; x < gridArray.GetLength(0); x++)
        {
            for (int z = 0; z < gridArray.GetLength(1); z++)
            {
                Debug.DrawLine(GetWorldPosition(x, z), GetWorldPosition(x, z + 1), Color.black, 100f);
                Debug.DrawLine(GetWorldPosition(x, z), GetWorldPosition(x + 1, z), Color.black, 100f);
            }    
        }
        Debug.DrawLine(GetWorldPosition(0, height), GetWorldPosition(width, height), Color.black, 100f);
        Debug.DrawLine(GetWorldPosition(width, 0), GetWorldPosition(width, height), Color.black, 100f);
    }

    private Vector3 GetWorldPosition(int x, int z)
    {
        return new Vector3(x, 0, z) * cellSize;
    }

}
