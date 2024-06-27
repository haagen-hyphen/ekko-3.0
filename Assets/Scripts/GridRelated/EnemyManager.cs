using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Slime{
    public Vector3Int position;
    public int tickSinceLastMove;
    public int tickPerMove;

    
    public Vector3Int GetMove()
    {
        return new Vector3Int(0, -1, 0);
    }
}


public class EnemyManager : MonoBehaviour
{
    public GridManager gridManager; 
    public List<Slime> slimes;

    private void MoveAllSlime()
    {
        foreach (var slime in slimes)
        {
            slime.tickSinceLastMove += 1;
            if (slime.tickSinceLastMove >= slime.tickPerMove)
            {
                gridManager.MoveTile(3, slime.position,slime.position + slime.GetMove());
                slime.position += slime.GetMove(); 
                slime.tickSinceLastMove = 0;
            }
        }
    }

    public void AnythingToBeDoneWheneverTicks(int tickPassed)
    {
        MoveAllSlime();
    }
    
}
