using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu]
public class Cell : Tile
{
    public bool isWalkable;
    public bool isPushable;
    public bool isTimeImmune;
    public Sprite abilityImage;
    public string abilityName;
    public bool isSlimyWall;

    public bool CheckNotBlocked(Vector3Int targetPosition, Vector3Int unitDirection){
        var cell3 = GridManager.Instance.GetCell(3, targetPosition);

        if (GridManager.Instance.CheckIfPushable(targetPosition)){
            return cell3.CheckNotBlocked(targetPosition + unitDirection, unitDirection);
             
        }
        else{
            return GridManager.Instance.CheckIfWalkable(targetPosition);
        }
    }

    public void PushBoxes(Vector3Int firstBoxPosition, Vector3Int unitDirection){
        var cell3 = GridManager.Instance.GetCell(3, firstBoxPosition + unitDirection);

        if (GridManager.Instance.CheckIfPushable(firstBoxPosition+unitDirection)){
            cell3.PushBoxes(firstBoxPosition + unitDirection, unitDirection);
            GridManager.Instance.MoveCell(3,firstBoxPosition,firstBoxPosition + unitDirection);
            
        }
        else{
            GridManager.Instance.MoveCell(3,firstBoxPosition,firstBoxPosition + unitDirection);
        }
    }
}

