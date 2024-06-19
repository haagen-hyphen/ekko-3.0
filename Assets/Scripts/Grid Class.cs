using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UI;
using UnityEngine;

public class GridClass
{
    public Vector3Int position;
    private GameObject self;
    private bool walkable;
    private Sprite floor = Resources.Load("Arts/basic_floor",typeof(Sprite)) as Sprite;

    public GridClass(GameObject self, Vector3Int position, bool walkable){
        this.position = position;
        this.self = self;
        this.walkable = walkable;
    }

    public void SetUpAsAWhole(){
        SetPosition();
        SetSprite();
    }
    public void SetPosition(){
        self.transform.localPosition = position;
    }
    public void SetSprite(){
        if(walkable){
            self.gameObject.GetComponent<SpriteRenderer>().sprite = floor;
        }
    }

    public bool CheckIfWalkable()
    {
        return walkable;
    }

    public void SetNotWalkable(){
        walkable = false;
    }

}
