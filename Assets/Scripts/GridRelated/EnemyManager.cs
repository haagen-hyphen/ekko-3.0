using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using System.Linq;
using UnityEngine.UIElements;
using System.Security.Cryptography;
using Unity.Collections;
using Unity.Burst.Intrinsics;

#region Declare Enemy
public class Enemy
{
    public Vector3Int position;
    public int tickSinceLastMove;
    [HideInInspector]public int tickPerMove;
    [HideInInspector]public int searchRadius;
    [HideInInspector]public bool movable;
    [HideInInspector]public bool ranged;
    [HideInInspector]public int shootingRange;
    public virtual void SetEnemyTypeData(){
    }
}
[System.Serializable]
public class Slime : Enemy
{
    public override void SetEnemyTypeData(){
        movable = true;
        ranged = false;
        tickPerMove = 2;
        searchRadius = 3;
    }
}
[System.Serializable]
public class SpearGoblin : Enemy
{
    public override void SetEnemyTypeData()
    {
        shootingRange = 5;
        ranged = true;
        movable = false;
    }
}
#endregion



public class EnemyManager : MonoBehaviour
{
    public GridManager gridManager; 
    public List<Slime> slimes;
    public List<SpearGoblin> spearGoblins;
    
    void Awake(){
        foreach (var slime in slimes){
            slime.SetEnemyTypeData();
        }
        foreach (var spearGoblin in spearGoblins){
            spearGoblin.SetEnemyTypeData();
        }
    }
    public void AnythingToBeDoneWheneverTicks(int tickPassed)
    {
        MoveAllSlime();
        AllCheckShoot();
    }
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
    
    private Vector3Int GetEnemyMove(Enemy enemy)
    {
        if (
            math.abs((gridManager.playerPosition - enemy.position)[0]) <= enemy.searchRadius &&
            math.abs((gridManager.playerPosition - enemy.position)[1]) <= enemy.searchRadius
        )
        {

            int[,] localGrid = gridManager.GetLocalGrid(enemy.position, enemy.searchRadius);

            (int, int) startingPosition = (enemy.searchRadius, enemy.searchRadius);
            (int, int) endingPosition = ( enemy.searchRadius - (gridManager.playerPosition - enemy.position )[1] , enemy.searchRadius + (gridManager.playerPosition - enemy.position )[0]);
            List<(int r, int c)> thePath = AStarSearch2d(startingPosition, endingPosition, localGrid);
            
            if (thePath.Count == 0) return Vector3Int.zero;
            return new Vector3Int( thePath[0].c - enemy.searchRadius , -(thePath[0].r - enemy.searchRadius),0);
            
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
                Vector3Int slimeMoveBuffer = GetEnemyMove(slime);
                if (slimeMoveBuffer != Vector3Int.zero)
                {
                    gridManager.MoveTile(3, slime.position, slime.position + slimeMoveBuffer);
                    slime.position += slimeMoveBuffer;
                    slime.tickSinceLastMove = 0;
                }
            }
        }
    }

    private void AllCheckShoot(){
        foreach (var spearGoblin in spearGoblins){
            if( gridManager.playerPosition.x == spearGoblin.position.x && math.abs(gridManager.playerPosition.y - spearGoblin.position.y)<=spearGoblin.shootingRange){
                Vector3Int unitDirection = (gridManager.playerPosition - spearGoblin.position)/math.abs((gridManager.playerPosition - spearGoblin.position).y);
                StartCoroutine(PrepareAndShoot(spearGoblin.position, spearGoblin.shootingRange, unitDirection));
            }
            else if(gridManager.playerPosition.y == spearGoblin.position.y && math.abs(gridManager.playerPosition.x - spearGoblin.position.x)<=spearGoblin.shootingRange){
                Vector3Int unitDirection = (gridManager.playerPosition - spearGoblin.position)/math.abs((gridManager.playerPosition - spearGoblin.position).x);
                StartCoroutine(PrepareAndShoot(spearGoblin.position, spearGoblin.shootingRange, unitDirection));
            }
        }
    }

    IEnumerator PrepareAndShoot(Vector3Int from, int shootingRange, Vector3Int unitDirection){
        //put "!" beside goblin
        //no need to wait for 1 tick here because tickmng calls player after enemy, enemy alr lag 1 tick
        yield return null;

        gridManager.SetCell(4, from + 1*unitDirection, gridManager.spear);
        yield return new WaitForSeconds(0.5f/shootingRange);
        for(int i = 2; i <= shootingRange; i++){
            gridManager.SetCell(4, from + (i-1)*unitDirection, null);
            gridManager.SetCell(4, from + i*unitDirection, gridManager.spear);
            yield return new WaitForSeconds(0.5f/shootingRange);
        }
        gridManager.SetCell(4, from + shootingRange*unitDirection, null);
        //a minor bug: when one coroutine setnull but another coroutine setspear at the same cell
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


