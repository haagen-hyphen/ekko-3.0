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
    [SerializeField]private TileBase[] tileBaseArray;
    [SerializeField]private Tilemap layer1;
    [SerializeField]private Tilemap layer2;
    [SerializeField]private Tilemap layer3;
    [SerializeField]private List<GridType> gridTypeClassList;
    [HideInInspector]public Vector3Int playerPosition;
    public Dictionary<TileBase,GridType> tileBaseToGridClassData = new Dictionary<TileBase,GridType>();
    
    public List<Button> buttons;



    void Awake(){
        PairUpTileBaseAndGridTypeClass();
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
        GridType gridClassItBelongsTo = tileBaseToGridClassData[tileToCheck];
        bool itsWalkability = gridClassItBelongsTo.walkable;
        return itsWalkability;
    }
    public bool CheckIfPushable(Vector3Int v){
        TileBase tileToCheck = GetTile(3,v);
        GridType gridClassItBelongsTo = tileBaseToGridClassData[tileToCheck];
        bool itsPushability = gridClassItBelongsTo.pushable;
        return itsPushability;
    }
    public void MoveTile(int layer, Vector3Int from, Vector3Int to){
        TileBase tileToMove = GetTile(layer, from);
        SetTile(layer, from, null);
        SetTile(layer, to, tileToMove);
    }

    public void CheckAndTriggerButtons(){
        foreach(var button in buttons){
            if(CheckIfLayer3HasObject(button.position) || playerPosition == button.position){
                if(!button.triggeredLastTick){
                    foreach(var door in button.doors){
                        AlterTileBase(door, tileBaseArray[0],tileBaseArray[1]);
                    }
                }
                button.triggeredLastTick = true;
            }
            else{
                if(button.triggeredLastTick){
                    foreach(var door in button.doors){
                        AlterTileBase(door, tileBaseArray[0],tileBaseArray[1]);
                    }
                }
                button.triggeredLastTick = false;
            }
        }
    }
    public void AlterTileBase(Vector3Int tilePosition, TileBase a, TileBase b){
        if(GetTile(1,tilePosition) == a){
            SetTile(1,tilePosition,b);
        }
        else if(GetTile(1,tilePosition) == b){
            SetTile(1,tilePosition,a);
        }
        
    }
    
}