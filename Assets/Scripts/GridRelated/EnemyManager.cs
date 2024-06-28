using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using System.Linq;

[System.Serializable]
public class Slime{
    public Vector3Int position;
    public int tickSinceLastMove;
    public int tickPerMove;
    public int searchRadius;
    
}




public class EnemyManager : MonoBehaviour
{
    public GridManager gridManager; 
    public List<Slime> slimes;
    
    void PrintGrid(int[,] grid)
    {
        int rows = grid.GetLength(0); // Get number of rows
        int cols = grid.GetLength(1); // Get number of columns

        // Iterate through each element and log its value
        for (int i = 0; i < rows; i++)
        {
            string rowString = "";
            for (int j = 0; j < cols; j++)
            {
                rowString += grid[i, j] + " ";
            }
            Debug.Log(rowString); // Output each row as a separate log entry
        }
    }
    
    private Vector3Int GetSlimeMove(Slime slime)
    {
        if (
            math.abs((gridManager.playerPosition - slime.position)[0]) <= slime.searchRadius &&
            math.abs((gridManager.playerPosition - slime.position)[1]) <= slime.searchRadius
        )
        {

            int[,] localGrid = gridManager.GetLocalGrid(slime.position, slime.searchRadius);

            (int, int) startingPosition = (slime.searchRadius, slime.searchRadius);
            (int, int) endingPosition = ( slime.searchRadius - (gridManager.playerPosition - slime.position )[1] , slime.searchRadius + (gridManager.playerPosition - slime.position )[0]);
            List<(int r, int c)> thePath = AStarSearch2d(startingPosition, endingPosition, localGrid);
            
            if (thePath.Count == 0) return Vector3Int.zero;
            return new Vector3Int( thePath[0].c - slime.searchRadius , -(thePath[0].r - slime.searchRadius),0);
            
        }

        return Vector3Int.zero;
    }
    private void MoveAllSlime()
    {
        foreach (var slime in slimes)
        {
            slime.tickSinceLastMove += 1;
            if (slime.tickSinceLastMove >= slime.tickPerMove)
            {
                Vector3Int slimeMoveBuffer = GetSlimeMove(slime);
                if (slimeMoveBuffer != Vector3Int.zero)
                {
                    gridManager.MoveTile(3, slime.position, slime.position + slimeMoveBuffer);
                    slime.position += slimeMoveBuffer;
                    slime.tickSinceLastMove = 0;
                }
            }
        }
    }



    public void AnythingToBeDoneWheneverTicks(int tickPassed)
    {
        MoveAllSlime();
    }
    
    private static int Heuristic((int r, int c) a, (int r, int c) b)
    {
        return Math.Abs(a.r- b.r) + Math.Abs(a.c - b.c);
    }
    
    private List<(int r, int c)> AStarSearch2d((int r,int c) startingPosition, (int r,int c) endingPosition, int[,] originalGrid)
    {
        var rows = originalGrid.GetLength(0);
        var cols = originalGrid.GetLength(1);
        int[,] grid = new int[rows, cols];

        Array.Copy(originalGrid, grid, originalGrid.Length);
        
        List<((int r ,int c) position, int heuristicScore, List<(int, int)> path)> node2Visit = new List<((int r, int c), int, List<(int r, int c)>)>();
        node2Visit.Add((startingPosition, Heuristic(startingPosition, endingPosition), new List<(int r, int c)>()));
        
        grid[startingPosition.r,startingPosition.c] = -1;
        while (node2Visit.Count > 0)
        {
            var node = node2Visit.OrderBy(node => node.heuristicScore).First();
            
            // if we reach the ending position return the path
            if (node.position.r == endingPosition.r && node.position.c == endingPosition.c)
            {

                return node.path;
            }
            
            var currentRow = node.position.r;
            var currentCol = node.position.c;
        
            node2Visit.Remove(node);

            if (currentRow + 1 < rows)
            {
                if (grid[currentRow + 1, currentCol] == 0)
                {
                    (int r ,int c) newPosition = node.position;
                    newPosition.r += 1;
                    List<(int r, int c)> newPath = new List<(int r, int c)>(node.path);
                    newPath.Add(newPosition);
                    node2Visit.Add((newPosition, Heuristic(newPosition, endingPosition) + newPath.Count, newPath));
                    grid[newPosition.r, newPosition.c] = -1;
                }
            }
            if (currentRow - 1 >= 0)
            {
                if (grid[currentRow -1, currentCol] == 0)
                {
                    (int r ,int c) newPosition = node.position;
                    newPosition.r -= 1;
                    List<(int r, int c)> newPath = new List<(int r, int c)>(node.path);
                    newPath.Add(newPosition);
                    node2Visit.Add((newPosition, Heuristic(newPosition, endingPosition) + newPath.Count, newPath));
                    grid[newPosition.r, newPosition.c] = -1;
                }
            }
            if (currentCol + 1< cols)
            {
                if (grid[currentRow, currentCol + 1] == 0)
                {
                    (int r ,int c) newPosition = node.position;
                    newPosition.c += 1;
                    List<(int r, int c)> newPath = new List<(int r, int c)>(node.path);
                    newPath.Add(newPosition);
                    node2Visit.Add((newPosition, Heuristic(newPosition, endingPosition) + newPath.Count, newPath));
                    grid[newPosition.r, newPosition.c] = -1;
                }
            }
            if (currentCol - 1 >= 0)
            {
                if (grid[currentRow, currentCol - 1] == 0)
                {
                    (int r ,int c) newPosition = node.position;
                    newPosition.c -= 1;
                    List<(int r, int c)> newPath = new List<(int r, int c)>(node.path);
                    newPath.Add(newPosition);
                    node2Visit.Add((newPosition, Heuristic(newPosition, endingPosition) + newPath.Count, newPath));
                    grid[newPosition.r, newPosition.c] = -1;
                }
            }
        }
        return new List<(int r, int c)>();
    }
}


