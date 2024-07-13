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
using UnityEngine.UI;

#region Declare Enemy
[Serializable]
public class Enemy
{
    public Vector3Int position;
    public Cell cell;
    public int tickSinceLastMove;
    public int tickPerMove;
    public int searchRadius;
    public bool movable;
    public bool ranged;
    public int shootingRange;
    public bool isTimeImmune;
    public virtual void Init(){
        cell = GridManager.Instance.GetCell(3, position);
    }

    public virtual void OnTick(){

    }

    public Enemy Clone(){
        return (Enemy)MemberwiseClone();
    }
    public virtual Vector3Int GetEnemyMove()
    {
        if (
            math.abs((GridManager.Instance.playerLastPosition - position)[0]) <= searchRadius &&
            math.abs((GridManager.Instance.playerLastPosition - position)[1]) <= searchRadius
        )
        {

            int[,] localGrid = GetLocalGrid(position, searchRadius);

            (int, int) startingPosition = (searchRadius, searchRadius);
            (int, int) endingPosition = (searchRadius - (GridManager.Instance.playerLastPosition - position )[1] , searchRadius + (GridManager.Instance.playerPosition - position )[0]);
            List<(int r, int c)> thePath = EnemyManager.Instance.AStarSearch2d(startingPosition, endingPosition, localGrid);

            if (thePath.Count == 0) return Vector3Int.zero;
            return new Vector3Int( thePath[0].c - searchRadius , -(thePath[0].r - searchRadius),0);

        }

        return Vector3Int.zero;
    }
    public virtual int[,] GetLocalGrid(Vector3Int centerPosition, int radius)
    {
        int[,] localGrid = new int[radius * 2 + 1, radius * 2 + 1];
        for (int r = 0; r < radius * 2 + 1; r++)
        {
            for (int c = 0; c < radius * 2 + 1; c++)
            {
                Vector3Int aimedDestination = centerPosition + new Vector3Int(c - radius, - (r - radius), 0);
                if(!GridManager.Instance.CheckIfWalkable(aimedDestination) || GridManager.Instance.CheckIfLayer3HasObject(aimedDestination))
                {
                    localGrid[r, c] = 1;
                }
            }
        }
        return localGrid;
    }
}
[Serializable]
public class Slime : Enemy
{
    public Slime(Vector3Int position){
        this.position = position;
        movable = true;
        ranged = false;
        tickPerMove = 2;
        searchRadius = 3;
        GridManager.Instance.SetCell(3,position,GridManager.Instance.slime);
        GridManager.Instance.SetCell(4,position,GridManager.Instance.slimeDeadly);
    }
    public override void Init(){
        movable = true;
        ranged = false;
        tickPerMove = 2;
        searchRadius = 3;
    }

    public override void OnTick()
    {
        tickSinceLastMove += 1;
        if (tickSinceLastMove >= tickPerMove)
        {
            Vector3Int unitDirection = GetEnemyMove();
            if (unitDirection != Vector3Int.zero)
            {
                GridManager.Instance.MoveCell(3, position, position + unitDirection);
                GridManager.Instance.MoveCell(4, position, position + unitDirection);
                position += unitDirection;
                tickSinceLastMove = 0;
            }
        }
    }
    public override int[,] GetLocalGrid(Vector3Int centerPosition, int radius)
    {
        int[,] localGrid = new int[radius * 2 + 1, radius * 2 + 1];
        for (int r = 0; r < radius * 2 + 1; r++)
        {
            for (int c = 0; c < radius * 2 + 1; c++)
            {
                Vector3Int aimedDestination = centerPosition + new Vector3Int(c - radius, - (r - radius), 0);
                if((!GridManager.Instance.CheckIfWalkable(aimedDestination)&&!GridManager.Instance.CheckIfIsSlimyWall(aimedDestination)) || GridManager.Instance.CheckIfLayer3HasObject(aimedDestination))
                {
                    localGrid[r, c] = 1;
                }
            }
        }
        return localGrid;
    }
}
[Serializable]
public class SpearGoblin : Enemy
{
    public override void Init()
    {
        base.Init();
        shootingRange = 5;
        ranged = true;
        movable = false;
    }

    public override void OnTick()
    {
        Vector3Int unitDirection = GetEnemyMove();
            if (unitDirection != Vector3Int.zero){
            PlayerMovement.Instance.CallShootSpear(position, shootingRange, unitDirection);
        }
    }
    public override Vector3Int GetEnemyMove(){
        Vector3Int minus = GridManager.Instance.playerLastPosition - position;
        int multiplicationOfXY = minus.x * minus.y;
        int theNoneZeroXY = minus.x + minus.y;
        if(multiplicationOfXY == 0){
            if(Mathf.Abs(theNoneZeroXY) <= shootingRange && Mathf.Abs(theNoneZeroXY)!=0){
                Vector3Int unitDirection = minus/Mathf.Abs(theNoneZeroXY);
                if(EnemyManager.Instance.CheckIfPathClear(unitDirection,Mathf.Abs(theNoneZeroXY),GetLocalGrid(position,shootingRange))){
                    return unitDirection;
                }
            }
        }
        return Vector3Int.zero;
    }
}
#endregion



public class EnemyManager : MonoBehaviour
{
    public static EnemyManager Instance;
    public GridManager gridManager;
    public List<Slime> slimes;
    public List<SpearGoblin> spearGoblins;
    public List<Enemy> enemies = new();
    public List<Enemy> timeImmuneEnemies = new();

    void Awake(){
        // Instance
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }


        // Init enemies
        foreach (Slime slime in slimes)
        {
            gridManager.SetCell(4, slime.position, gridManager.slimeDeadly);
            enemies.Add(slime.Clone());
        }

        foreach (SpearGoblin spearGoblin in spearGoblins)
        {
            enemies.Add(spearGoblin.Clone());
        }
        foreach(Enemy enemy in enemies){
            enemy.Init();
            if (enemy.isTimeImmune)
            {
                timeImmuneEnemies.Add(enemy);
            }
        }
    }
    public void AnythingToBeDoneWheneverTicks(int tickPassed)
    {
        foreach (var enemy in enemies)
        {
            enemy.OnTick();
        }
    }

    #region Get Set

    public void SetEnemies(List<Enemy> newEnemies){
        enemies = newEnemies.Select(item => item.Clone()).ToList();
    }

    public void OnTimeReversal(List<Enemy> newEnemies)
    {
        SetEnemies(newEnemies);
        enemies = enemies.Concat(timeImmuneEnemies).ToList();
        
        foreach (var enemy in timeImmuneEnemies)
        {
            if (gridManager.GetCell(3, enemy.position) == null)
            {
                gridManager.SetCell(3, enemy.position, enemy.cell);
            }
        }
    }

    #endregion

    public void PrintGrid(int[,] grid)
    {
        int rows = grid.GetLength(0); // Get number of rows
        int cols = grid.GetLength(1); // Get number of columns
        string toPrint = "";
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                toPrint += grid[i, j] + " ";
            }
            toPrint += "\n";
        }
        Debug.Log(toPrint);
    }

    public void KillAnEnemy(Vector3Int position){ 
        //in some sense, you can also use this function to "kill" a box, this is a stronger function 
        gridManager.SetCell(3,position,null);   //to kill an enemy's layer3 visually
        gridManager.SetCell(4,position,null);   //to kill an enemy's layer4 deadly
        RemoveEnemyByPosition(position);        //to kill an enemy's soul, i.e. remove it from the list
    }

    

    


    private static int HeuristicByMoveDistance((int r, int c) a, (int r, int c) b)
    {
        return Math.Abs(a.r- b.r) + Math.Abs(a.c - b.c);
    }
    
    private static int HeuristicByAbsDistance((int r, int c) a, (int r, int c) b)
    {
        return (int)(Mathf.Pow((a.r - b.r), 2) + Mathf.Pow((a.c - b.c), 2));
    }


    public List<(int r, int c)> AStarSearch2d((int r,int c) startingPosition, (int r,int c) endingPosition, int[,] originalGrid)
    {
        var rows = originalGrid.GetLength(0);
        var cols = originalGrid.GetLength(1);
        int[,] grid = new int[rows, cols];

        Array.Copy(originalGrid, grid, originalGrid.Length);

        List<((int r ,int c) position, int heuristicScore, List<(int, int)> path)> node2Visit = new List<((int r, int c), int, List<(int r, int c)>)>();
        node2Visit.Add((startingPosition, HeuristicByAbsDistance(startingPosition, endingPosition), new List<(int r, int c)>()));

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
                    node2Visit.Add((newPosition, HeuristicByAbsDistance(newPosition, endingPosition) + newPath.Count, newPath));
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
                    node2Visit.Add((newPosition, HeuristicByAbsDistance(newPosition, endingPosition) + newPath.Count, newPath));
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
                    node2Visit.Add((newPosition, HeuristicByAbsDistance(newPosition, endingPosition) + newPath.Count, newPath));
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
                    node2Visit.Add((newPosition, HeuristicByAbsDistance(newPosition, endingPosition) + newPath.Count, newPath));
                    grid[newPosition.r, newPosition.c] = -1;
                }
            }
        }
        return new List<(int r, int c)>();
    }
    public bool CheckIfPathClear(Vector3Int unitDirection, int Range, int[,] localGrid){
        int length = localGrid.GetLength(0);
        bool check=true;
        for(int i=1; i<=Range;i++){
            if(localGrid[(length-1)/2 - i*unitDirection.y, (length-1)/2 + i*unitDirection.x] == 1){
                //some array issue
                check = false;
            }
        }
        return check;
    }
    public void RemoveEnemyByPosition(Vector3Int position)
    {
        var enemyToRemove = enemies.FirstOrDefault(e => e.position.Equals(position));
        if (enemyToRemove != null)
        {
            enemies.Remove(enemyToRemove);
        }
    }
}


