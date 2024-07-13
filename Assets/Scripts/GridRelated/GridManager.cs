using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using TMPro;
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

    public Button Clone()
    {
        return new Button
        {
            position = position,
            doors = doors
        };
    }
}

[System.Serializable]
public class ColourKey{
    public Vector3Int position;
    public Cell keyCell;
    public List<Vector3Int> doors;
    public Cell doorCell;

    [HideInInspector]
    public bool pickProtect = false;
    public bool holdByPlayer = false;

}



public class GridManager : MonoBehaviour
{
    #region Variables
    public static GridManager Instance { get; private set; }
    public Cell wall, floor, spear, trapDeadly, slimeDeadly, slimyWall, slime;
    public PlayerMovement playerMovement;
    [SerializeField]private Tilemap layer1;
    [SerializeField]private Tilemap layer2;
    [SerializeField]private Tilemap layer3;
    [SerializeField]private Tilemap layer4;
    [HideInInspector]public Vector3Int playerPosition;
    [HideInInspector]public Vector3Int playerLastPosition;


    public Dictionary<Vector3Int, Cell> layer1TimeImmune = new();
    public Dictionary<Vector3Int, Cell> layer2TimeImmune = new();
    public Dictionary<Vector3Int, Cell> layer3TimeImmune = new();
    public Dictionary<Vector3Int, Cell> layer4TimeImmune = new();

    public Dictionary<Vector3Int, Cell>[] timeImmuneObjects = new Dictionary<Vector3Int, Cell>[4];

    public List<Button> buttons;
    public List<ColourKey> colourKeys;
    public List<Vector3Int> trapPositions;

    #endregion


    void Awake(){
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }

        timeImmuneObjects = new Dictionary<Vector3Int, Cell>[] {layer1TimeImmune, layer2TimeImmune, layer3TimeImmune, layer4TimeImmune};

        foreach(var trapPos in trapPositions){
            SetCell(4, trapPos, trapDeadly);
        }

        for (int i = layer1.cellBounds.min.x; i < layer1.cellBounds.max.x; i++)
        {
            for (int j = layer1.cellBounds.min.y; j < layer1.cellBounds.max.y; j++)
            {
                Vector3Int pos = new(i,j,0);
                Cell cell = GetCell(1, pos);
                if (cell)
                {
                    if (cell.isTimeImmune)
                    {
                        layer1TimeImmune[pos] = cell;
                    }
                }
            }
        }
        for (int i = layer2.cellBounds.min.x; i < layer2.cellBounds.max.x; i++)
        {
            for (int j = layer2.cellBounds.min.y; j < layer2.cellBounds.max.y; j++)
            {
                Vector3Int pos = new(i,j,0);
                Cell cell = GetCell(2, pos);
                if (cell)
                {
                    if (cell.isTimeImmune)
                    {
                        layer2TimeImmune[pos] = cell;
                    }
                }
            }
        }
        for (int i = layer3.cellBounds.min.x; i < layer3.cellBounds.max.x; i++)
        {
            for (int j = layer3.cellBounds.min.y; j < layer3.cellBounds.max.y; j++)
            {
                Vector3Int pos = new(i,j,0);
                Cell cell = GetCell(3, pos);
                if (cell)
                {
                    if (cell.isTimeImmune)
                    {
                        layer3TimeImmune[pos] = cell;
                    }
                }
            }
        }
        for (int i = layer4.cellBounds.min.x; i < layer4.cellBounds.max.x; i++)
        {
            for (int j = layer4.cellBounds.min.y; j < layer4.cellBounds.max.y; j++)
            {
                Vector3Int pos = new(i,j,0);
                Cell cell = GetCell(4, pos);
                if (cell)
                {
                    if (cell.isTimeImmune)
                    {
                        layer4TimeImmune[pos] = cell;
                    }
                }
            }
        }
    }

    public void AnythingToBeDoneWheneverTicks(int tickPassed){
        CheckAndTriggerButtons();
        CheckAndPickUpColourKeys();
        CheckColourDoorUnlock();
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
                layer1.SetTile(position, cell);
                break;
            case 2:
                layer2.SetTile(position, cell);
                break;
            case 3:
                layer3.SetTile(position, cell);
                break;
            case 4:
                layer4.SetTile(position, cell);
                if(position == playerPosition && cell != null){
                    playerMovement.Die(cell);
                }
                break;
        }
    }
    
    public void AdvancedSetCell(int layer, Vector3Int position, Cell cell){
        switch(layer){
            case 1:
                layer1.SetTile(position, cell);
                if(!CheckIfWalkable(position)){
                    layer2.SetTile(position, null);
                    layer3.SetTile(position, null);
                    layer4.SetTile(position, null);
                }
                break;
            case 2:
                if(CheckIfWalkable(position)){
                    layer2.SetTile(position, cell);
                }
                break;
            case 3:
                if(CheckIfWalkable(position)){
                    layer3.SetTile(position, cell);
                }
                break;
            case 4:
                if(CheckIfWalkable(position)){
                    layer4.SetTile(position, cell);
                    if(position == playerPosition && cell != null){
                        playerMovement.Die(cell);
                    }
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
        return new GameState(playerPosition, TilemapToDict(1), TilemapToDict(2), TilemapToDict(3), TilemapToDict(4));
    }

    public void SetButtons(List<Button> newButtons){
        buttons = newButtons.Select(item => item.Clone()).ToList();
    }

    #endregion

    #region Time Immune

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
                        if (!cell.isTimeImmune)
                        {
                            dict[pos] = cell;
                        }
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
                        if (!cell.isTimeImmune)
                        {
                            dict[pos] = cell;
                        }
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
                        if (!cell.isTimeImmune)
                        {
                            dict[pos] = cell;
                        }
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
                        if (!cell.isTimeImmune)
                        {
                            dict[pos] = cell;
                        }
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
            foreach(var item in dict)
            {
                SetCell(1, item.Key, item.Value);
            }
        }
        if (layer == 2)
        {
            foreach(var item in dict)
            {
                SetCell(2, item.Key, item.Value);
            }
        }
        if (layer == 3)
        {
            foreach(var item in dict)
            {
                SetCell(3, item.Key, item.Value);
            }
        }
        if (layer == 4)
        {
            foreach(var item in dict)
            {
                SetCell(4, item.Key, item.Value);
            }
        }
    }

    public void RevertGameState(GameState state){
        layer1.ClearAllTiles();
        layer2.ClearAllTiles();
        layer3.ClearAllTiles();
        layer4.ClearAllTiles();
        playerPosition = state.playerPosition;
        playerMovement.transform.position = state.playerPosition;
        DictToTilemap(1, state.layer1);
        DictToTilemap(2, state.layer2);
        DictToTilemap(3, state.layer3);
        DictToTilemap(4, state.layer4);
        SetButtons(state.buttons);

        EnemyManager.Instance.OnTimeReversal(state.enemies);

        for (int i = 0; i < 4; i++)
        {
            foreach (var item in timeImmuneObjects[i])
            {
                if (GetCell(i+1, item.Key) == null)
                {
                    SetCell(i+1, item.Key, item.Value);
                }
            }
        }
        
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
        if(!CheckIfLayer3HasObject(position)){
            return false;
        }
        else{
            Cell cell = GetCell(3, position);
            return cell.isPushable;
        }
        
    }
    public bool CheckIfIsSlimyWall(Vector3Int position){
        if(!CheckIfLayer1HasObject(position)){
            return false;
        }
        else{
            Cell cell = GetCell(1, position);
            return cell.isSlimyWall;
        }
        
    }
    public void MoveCell(int layer, Vector3Int from, Vector3Int to){
        Cell cell = GetCell(layer, from);
        if (timeImmuneObjects[layer-1].ContainsKey(from))
        {
            timeImmuneObjects[layer-1].Remove(from);
            timeImmuneObjects[layer-1][to] = cell;
        }
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

    // Need to do time immune dictionary manipulation
    public void AlterTileBase(Vector3Int position, Cell cell1, Cell cell2){
        if(GetCell(1,position) == cell1){
            SetCell(1, position, cell2);
        }
        else if(GetCell(1,position) == cell2){
            SetCell(1,position, cell1);
        }

    }
    
    
    public void CheckAndPickUpColourKeys()
    {

        ColourKey keyPlayerHold  = null;
        ColourKey keyPlayerHoldNext  = null;
        foreach(var key in colourKeys){
            if (key.holdByPlayer)
            {
                keyPlayerHold = key;
                if (keyPlayerHold.pickProtect)
                {
                    if (keyPlayerHold.position != playerPosition) keyPlayerHold.pickProtect = false;
                }
                keyPlayerHold.position = playerPosition;
            }
            if (playerPosition == key.position&& key.holdByPlayer == false)
            {

                keyPlayerHoldNext = key;
            }

        }

        if (keyPlayerHoldNext != null)
        {
            
            if (keyPlayerHold != null)
            {
                if (keyPlayerHold.pickProtect) return;
                
                keyPlayerHold.holdByPlayer = false;
                SetCell(2, playerPosition, keyPlayerHold.keyCell);

            }
            else
            {
                SetCell(2, playerPosition, null);
            }

            keyPlayerHoldNext.holdByPlayer = true;
            keyPlayerHoldNext.pickProtect = true;
            keyPlayerHold = keyPlayerHoldNext;
            
        }

        foreach (var key in colourKeys)
        {
            key.doorCell.isWalkableByKeyPlayerHolding = false;
        }

        if (keyPlayerHold != null)
        {
            keyPlayerHold.doorCell.isWalkableByKeyPlayerHolding = true;
        }
    }
    
    public void CheckColourDoorUnlock()
    {
        foreach(var key in colourKeys){
            foreach (var door in key.doors)
            {
                if (playerPosition == door)
                {
                    SetCell(1, door, floor);
                    key.doors.Remove(door);
                    return;
                }
            }

        }

    }
    
}

