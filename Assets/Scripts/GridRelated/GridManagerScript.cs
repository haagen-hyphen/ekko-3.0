using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GridManagerScript : MonoBehaviour
{
    [SerializeField]private Tilemap layer0;
    [SerializeField]private Tilemap layer1;
    [SerializeField]private List<GridTypeClass> gridTypeClassList;
    //public Dictionary<Vector3,TileBase> positionToTileBase = new Dictionary<Vector3,TileBase>();
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

    public bool CheckIfWalkable(Vector3 v){
        Vector3Int gridPosition = layer0.WorldToCell(v);
        TileBase tileToCheck = layer0.GetTile(gridPosition);
        GridTypeClass gridClassItBelongsTo = tileBaseToGridClassData[tileToCheck];
        bool itsWalkability = gridClassItBelongsTo.walkable;
        return itsWalkability;
    }

    public bool CheckIfInteractable(Vector3 v){
        try{
            Vector3Int gridPosition = layer1.WorldToCell(v);
            TileBase tileToCheck = layer1.GetTile(gridPosition);
            GridTypeClass gridClassItBelongsTo = tileBaseToGridClassData[tileToCheck];
            return true;
        }
        catch{
            return false;
        }
    }
    public bool CheckIfPushable(Vector3 v){
        Vector3Int gridPosition = layer1.WorldToCell(v);
        TileBase tileToCheck = layer1.GetTile(gridPosition);
        GridTypeClass gridClassItBelongsTo = tileBaseToGridClassData[tileToCheck];
        bool itsPushability = gridClassItBelongsTo.pushable;
        return itsPushability;
    }

    public void MoveTile(Vector3Int from, Vector3Int to){
        TileBase tileToMove = layer1.GetTile(from);
        layer1.SetTile(from, null);
        layer1.SetTile(to, tileToMove);
    }

    //for positioning purpose, most likely could be replaced by the following
    //Vector3Int gridPosition = layer0.WorldToCell(new Vector3 v));
    //TileBase tileToCheck = layer0.GetTile(gridPosition);
}