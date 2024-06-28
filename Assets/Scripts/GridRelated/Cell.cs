using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu]
public class Cell : Tile
{
    public bool isWalkable;
    public bool isPushable;
    public bool isTimeImmune;
}
