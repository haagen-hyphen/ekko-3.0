using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GridManagerScript : MonoBehaviour
{
    [SerializeField]private Tilemap layer0;
    [SerializeField]private Tilemap layer1;
    [SerializeField]private List<GridClass> gridClassList;
    public Dictionary<TileBase,GridClass> tileBaseToTileData = new Dictionary<TileBase,GridClass>();
    
    void Awake(){
        foreach (var gridClass in gridClassList){     //tileDataList is set above, contains gridClass
            foreach (var tile in gridClass.tile){    //in each class, there might be more than one kind of tile, e.g. 2 kinds of floor
                tileBaseToTileData[tile] = gridClass;
            }
        }
    }

    public bool CheckIfWalkable(Vector3 v){
        Vector3Int gridPosition = layer0.WorldToCell(v);
        TileBase tileToCheck = layer0.GetTile(gridPosition);
        GridClass gridClassItBelongsTo = tileBaseToTileData[tileToCheck];
        bool itsWalkability = gridClassItBelongsTo.walkable;
        return itsWalkability;
    }

    //for positioning purpose, most likely could be replaced by the following
    //Vector3Int gridPosition = layer0.WorldToCell(new Vector3 v));
    //TileBase tileToCheck = layer0.GetTile(gridPosition);
}