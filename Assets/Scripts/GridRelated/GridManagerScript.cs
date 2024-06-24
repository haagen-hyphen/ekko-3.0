using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GridManagerScript : MonoBehaviour
{
    [SerializeField]private TileBase[] tileBaseArray;
    [SerializeField]private Tilemap layer1;
    [SerializeField]private Tilemap layer2;
    [SerializeField]private Tilemap layer3;
    [SerializeField]private List<GridTypeClass> gridTypeClassList;
    [HideInInspector]public Vector3Int playerPosition;
    public Dictionary<TileBase,GridTypeClass> tileBaseToGridClassData = new Dictionary<TileBase,GridTypeClass>();
    
    public List<Vector3Int> buttons;
    [System.Serializable]
    public class serializableClass{
        public List<Vector3Int> DoorsControlledByTheSameButton;
    }
    public List<serializableClass> listToBePairedWithButton = new List<serializableClass>();
    public Dictionary<Vector3Int, serializableClass> buttonDoorPairs = new Dictionary<Vector3Int,serializableClass>();



    void Awake(){
        PairUpTileBaseAndGridTypeClass();
        PairUpButtonAndDoors();
    }

    public void AnythingToBeDoneWheneverTicks(int tickPassed){
        CheckAndTriggerButtons();
    }

    public void PairUpTileBaseAndGridTypeClass(){
        foreach (var gridTypeClass in gridTypeClassList){     //tileDataList is set above, contains gridClass
            foreach (var tile in gridTypeClass.tile){    //in each class, there might be more than one kind of tile, e.g. 2 kinds of floor
                tileBaseToGridClassData[tile] = gridTypeClass;
            }
        }
    }
    public void PairUpButtonAndDoors(){
        for(int i = 0; i < buttons.Count; i++){
            buttonDoorPairs[buttons[i]] = listToBePairedWithButton[i];
        }
    }
    public TileBase GetTile(int layer, Vector3Int v){
        switch(layer){
            case 1:
                return layer1.GetTile(v);
            case 2:
                return layer2.GetTile(v);
            case 3:
                return layer3.GetTile(v);
        }
        return null;
    }
    public void SetTile(int layer, Vector3Int v, TileBase t){
        switch(layer){
            case 1:
                layer1.SetTile(v,t);break;
            case 2:
                layer2.SetTile(v,t);break;
            case 3:
                layer3.SetTile(v,t);break;
        }
    }
    public bool CheckIfLayer2HasObject(Vector3Int v){
        TileBase tileToCheck = GetTile(2,v);
        if (tileToCheck){
            return true;
        }
        return false;
    }
    public bool CheckIfLayer3HasObject(Vector3Int v){
        TileBase tileToCheck = GetTile(3,v);
        if (tileToCheck){
            return true;
        }
        return false;
    }
    public bool CheckIfWalkable(Vector3Int v){
        TileBase tileToCheck = GetTile(1,v);
        GridTypeClass gridClassItBelongsTo = tileBaseToGridClassData[tileToCheck];
        bool itsWalkability = gridClassItBelongsTo.walkable;
        return itsWalkability;
    }
    public bool CheckIfPushable(Vector3Int v){
        TileBase tileToCheck = GetTile(3,v);
        GridTypeClass gridClassItBelongsTo = tileBaseToGridClassData[tileToCheck];
        bool itsPushability = gridClassItBelongsTo.pushable;
        return itsPushability;
    }
    public void MoveTile(int layer, Vector3Int from, Vector3Int to){
        TileBase tileToMove = GetTile(layer, from);
        SetTile(layer, from, null);
        SetTile(layer, to, tileToMove);
    }
    
    private bool wasTriggeredLastTick = false;
    public void CheckAndTriggerButtons(){
        foreach(var buttonPosition in buttons){
            if(CheckIfLayer3HasObject(buttonPosition) || playerPosition == buttonPosition){
                if(!wasTriggeredLastTick){
                    foreach(var door in buttonDoorPairs[buttonPosition].DoorsControlledByTheSameButton){
                        AlterTileBase(door, tileBaseArray[0],tileBaseArray[1]);
                    }
                }
                wasTriggeredLastTick = true;
            }
            else{
                if(wasTriggeredLastTick){
                    foreach(var door in buttonDoorPairs[buttonPosition].DoorsControlledByTheSameButton){
                        AlterTileBase(door, tileBaseArray[0],tileBaseArray[1]);
                    }
                }
                wasTriggeredLastTick = false;
            }
        }
    }
    public void AlterTileBase(Vector3Int v, TileBase a, TileBase b){
        if(GetTile(1,v) == a){
            SetTile(1,v,b);
        }
        else if(GetTile(1,v) == b){
            SetTile(1,v,a);
        }
    }
}