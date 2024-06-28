using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;
using UnityEngine.Tilemaps;

[System.Serializable]
public class Button{
    public Vector3Int position;
    public List<Vector3Int> doors;

    [HideInInspector]
    public bool triggeredLastTick = false;
}

public class GridManager : MonoBehaviour
{
    public Cell wall, floor;
    [SerializeField]private Tilemap layer1;
    [SerializeField]private Tilemap layer2;
    [SerializeField]private Tilemap layer3;
    [HideInInspector]public Vector3Int playerPosition;
    
    public List<Button> buttons;



    void Awake(){

    }

    public void AnythingToBeDoneWheneverTicks(int tickPassed){
        CheckAndTriggerButtons();
    }

    #region Get Set
    
    public Cell GetCell(int layer, Vector3Int position){
        return layer switch
        {
            1 => (Cell)layer1.GetTile(position),
            2 => (Cell)layer2.GetTile(position),
            3 => (Cell)layer3.GetTile(position),
            _ => null,
        };
    }
    public void SetCell(int layer, Vector3Int position, Cell cell){
        switch(layer){
            case 1:
                layer1.SetTile(position, cell);break;
            case 2:
                layer2.SetTile(position, cell);break;
            case 3:
                layer3.SetTile(position, cell);break;
        }
    }

    public Tilemap GetLayer(int layer)
    {
        return layer switch
        {
            1 => layer1,
            2 => layer2,
            3 => layer3,
            _ => null,
        };
    }

    public void SetLayer(int layer, Tilemap tilemap)
    {
        switch (layer)
        {
            case 1:
                layer1 = tilemap; break;
            case 2:
                layer2 = tilemap; break;
            case 3:
                layer3 = tilemap; break;
        }
    }

    public GameState GetGameState()
    {
        return new GameState(playerPosition, layer1, layer2, layer3, buttons);
    }

    public void SetButtons(List<Button> newButtons){
        buttons = newButtons;
    }

    #endregion

    public bool CheckIfLayer1HasObject(Vector3Int position){
        Cell cell = GetCell(1,position);
        if (cell){
            return true;
        }
        return false;
    }

    public bool CheckIfLayer2HasObject(Vector3Int position){
        Cell cell = GetCell(2,position);
        if (cell){
            return true;
        }
        return false;
    }
    public bool CheckIfLayer3HasObject(Vector3Int position){
        Cell cell = GetCell(3,position);
        if (cell){
            return true;
        }
        return false;
    }
    public bool CheckIfWalkable(Vector3Int position){
        if(!CheckIfLayer1HasObject(position)){
            return false;
        }
        else{
            Cell cell = GetCell(1, position);
            return cell.isWalkable;
        }
        
    }

    public bool CheckIfPushable(Vector3Int position){
        Cell cell = GetCell(3, position);
        return cell.isPushable;
    }
    public void MoveTile(int layer, Vector3Int from, Vector3Int to){
        Cell cell = GetCell(layer, from);
        SetCell(layer, from, null);
        SetCell(layer, to, cell);
    }

    public void CheckAndTriggerButtons(){
        foreach(var button in buttons){
            if(CheckIfLayer3HasObject(button.position) || playerPosition == button.position){
                if(!button.triggeredLastTick){
                    foreach(var door in button.doors){
                        AlterTileBase(door, floor, wall);
                    }
                }
                button.triggeredLastTick = true;
            }
            else{
                if(button.triggeredLastTick){
                    foreach(var door in button.doors){
                        AlterTileBase(door, floor, wall);
                    }
                }
                button.triggeredLastTick = false;
            }
        }
    }
    public void AlterTileBase(Vector3Int position, Cell cell1, Cell cell2){
        if(GetCell(1,position) == cell1){
            SetCell(1, position, cell2);
        }
        else if(GetCell(1,v) == b){
            SetCell(1,v,a);
        }
        
    }

    public int[,] GetLocalGrid(Vector3Int centerPosition, int radius)
    {
        int[,] localGrid = new int[radius * 2 + 1, radius * 2 + 1];
        for (int r = 0; r < radius * 2 + 1; r++)
        {
            for (int c = 0; c < radius * 2 + 1; c++)
            {
                Vector3Int aimedDestination = centerPosition + new Vector3Int(c - radius, - (r - radius), 0);
                if(!CheckIfWalkable(aimedDestination) || CheckIfLayer3HasObject(aimedDestination))
                {
                    localGrid[r, c] = 1;
                }
            }
        }
        return localGrid;
    }
}