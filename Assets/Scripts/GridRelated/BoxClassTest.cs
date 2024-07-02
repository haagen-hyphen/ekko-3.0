using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class Box : Cell
{
    public bool CheckPushability(Vector3Int targetPosition, Vector3Int unitDirection){
        var cell3 = GridManager.Instance.GetCell(3, targetPosition);

        if (GridManager.Instance.IsOfType<Box>(cell3)){
            Box box = (Box)cell3; // Cast to Box
            bool x = box.CheckPushability(targetPosition + unitDirection, unitDirection);
            return x;
        }
        else{
            return GridManager.Instance.CheckIfWalkable(targetPosition);
        }
    }

    public void PushBoxes(Vector3Int firstBoxPosition, Vector3Int unitDirection){
        var cell3 = GridManager.Instance.GetCell(3, firstBoxPosition + unitDirection);

        if (GridManager.Instance.IsOfType<Box>(cell3)){
            Box box = (Box)cell3;
            box.PushBoxes(firstBoxPosition + unitDirection, unitDirection);
            GridManager.Instance.MoveCell(3,firstBoxPosition,firstBoxPosition + unitDirection);
            
        }
        else{
            GridManager.Instance.MoveCell(3,firstBoxPosition,firstBoxPosition + unitDirection);
        }
    }
}