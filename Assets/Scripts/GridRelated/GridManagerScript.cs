using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GridManagerScript : MonoBehaviour
{
    [SerializeField]private Tilemap layer1;
    [SerializeField]private Tilemap layer2;
    [SerializeField]private Tilemap layer3;
    [SerializeField]private List<GridTypeClass> gridTypeClassList;
    [HideInInspector]public Vector2Int playerPosition;
    public Dictionary<TileBase,GridTypeClass> tileBaseToGridClassData = new Dictionary<TileBase,GridTypeClass>();
    

    void Awake(){
        PairUpTileBaseAndGridTypeClass();
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
    // public bool CheckIfWalkable(Vector3 v){
    //     Vector3Int gridPosition = layer1.WorldToCell(v);
    //     TileBase tileToCheck = layer1.GetTile(gridPosition);
    //     GridTypeClass gridClassItBelongsTo = tileBaseToGridClassData[tileToCheck];
    //     bool itsWalkability = gridClassItBelongsTo.walkable;
    //     return itsWalkability;
    // }
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

}