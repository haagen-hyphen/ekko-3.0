using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using Unity.VisualScripting;
using UnityEditor.UI;
using UnityEngine;
using UnityEngine.Tilemaps;

[System.Serializable]
public class Button{
    public Vector3Int position;
    public List<Vector3Int> doors;

    [HideInInspector]
    public bool triggeredLastTick = false;
}

[System.Serializable]
public class Trap{
    public Vector3Int position;
}

public class GridManager : MonoBehaviour
{
    public Cell wall, floor, spear, trapEmpty;
    public PlayerMovement playerMovement;
    [SerializeField]private Tilemap layer1;
    [SerializeField]private Tilemap layer2;
    [SerializeField]private Tilemap layer3;
    [SerializeField]private Tilemap layer4;
    [HideInInspector]public Vector3Int playerPosition;
    
    public List<Button> buttons;
    public List<Trap> traps;



    void Awake(){
        foreach(var trap in traps){
            SetCell(4, trap.position, trapEmpty);
        }
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
            4 => (Cell)layer4.GetTile(position),
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
            case 4:
                layer4.SetTile(position, cell);
                if(position == playerPosition){
                    playerMovement.Die(cell);
                }
                break;
        }
    }

    public Tilemap GetLayer(int layer)
    {
        return layer switch
        {
            1 => layer1,
            2 => layer2,
            3 => layer3,
            4 => layer4,
            _ => null,
        };
    }

    public GameState GetGameState()
    {
        return new GameState(playerPosition, TilemapToDict(1), TilemapToDict(2), TilemapToDict(3), TilemapToDict(4), buttons);
    }

    public void SetButtons(List<Button> newButtons){
        buttons = new List<Button>(newButtons);
    }

    #endregion

    public Dictionary<Vector3Int, Cell> TilemapToDict(int layer)
    {
        Dictionary<Vector3Int, Cell> dict = new();
        if (layer == 1)
        {
            for (int i = layer1.cellBounds.min.x; i < layer1.cellBounds.max.x; i++)
            {
                for (int j = layer1.cellBounds.min.y; j < layer1.cellBounds.max.y; j++)
                {
                    Vector3Int pos = new(i,j,0);
                    Cell cell = GetCell(1, pos);
                    if (cell)
                    {
                        dict[pos] = cell; 
                    }
                }
            }
        }
        if (layer == 2)
        {
            for (int i = layer2.cellBounds.min.x; i < layer2.cellBounds.max.x; i++)
            {
                for (int j = layer2.cellBounds.min.y; j < layer2.cellBounds.max.y; j++)
                {
                    Vector3Int pos = new(i,j,0);
                    Cell cell = GetCell(2, pos);
                    if (cell)
                    {
                        dict[pos] = cell;    
                    }
                }
            }
        }
        if (layer == 3)
        {
            for (int i = layer3.cellBounds.min.x; i < layer3.cellBounds.max.x; i++)
            {
                for (int j = layer3.cellBounds.min.y; j < layer3.cellBounds.max.y; j++)
                {
                    Vector3Int pos = new(i,j,0);
                    Cell cell = GetCell(3, pos);
                    if (cell)
                    {
                        dict[pos] = cell; 
                    }
                }
            }
        }
        if (layer == 4)
        {
            for (int i = layer4.cellBounds.min.x; i < layer4.cellBounds.max.x; i++)
            {
                for (int j = layer4.cellBounds.min.y; j < layer4.cellBounds.max.y; j++)
                {
                    Vector3Int pos = new(i,j,0);
                    Cell cell = GetCell(4, pos);
                    if (cell)
                    {
                        dict[pos] = cell; 
                    }
                }
            }
        }
        return dict;
    }

    public void DictToTilemap(int layer, Dictionary<Vector3Int, Cell> dict)
    {
        if (layer == 1)
        {
            layer1.ClearAllTiles();
            foreach(var item in dict)
            {
                SetCell(1, item.Key, item.Value);
            }
        }
        if (layer == 2)
        {
            layer2.ClearAllTiles();
            foreach(var item in dict)
            {
                SetCell(2, item.Key, item.Value);
            }
        }
        if (layer == 3)
        {
            layer3.ClearAllTiles();
            foreach(var item in dict)
            {
                SetCell(3, item.Key, item.Value);
            }
        }
        if (layer == 4)
        {
            layer4.ClearAllTiles();
            foreach(var item in dict)
            {
                SetCell(4, item.Key, item.Value);
            }
        }
    }

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
    public bool CheckIfLayer4HasObject(Vector3Int position){
        Cell cell = GetCell(4,position);
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
        else if(GetCell(1,position) == cell2){
            SetCell(1,position, cell1);
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