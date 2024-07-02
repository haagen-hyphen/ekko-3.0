using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu]
public class Cell : Tile
{
    public bool isWalkable;
    public bool isTimeImmune;
    public Sprite abilityImage;
}

