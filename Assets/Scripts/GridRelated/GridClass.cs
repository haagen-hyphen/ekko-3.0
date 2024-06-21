using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UI;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu]
public class GridClass: ScriptableObject
{
    public TileBase[] tile;
    public bool walkable;

    // Basically, this is for 
    // 1. defining some characteristic of a grid, for example walkability, or
    // 2. be inherited by child classes like door, for example as below
    //      public int interactingPairIndex;
    //      public 




}
