using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public class FloorManager : MonoBehaviour
{
    public int FloorStartFromX;
    public int FloorEndAtX;
    public int FloorStartFromY;
    public int FloorEndAtY;
    private GameObject emptyObjectWhereGridClassIsPlaced;
    // Start is called before the first frame update
    void Awake()
    {
        SetUpFloor();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void SetUpFloor(){
        // emptyObjectWhereGridClassIsPlaced = new GameObject("empty");
        // emptyObjectWhereGridClassIsPlaced.transform.parent = this.gameObject.transform;
        // emptyObjectWhereGridClassIsPlaced.AddComponent<SpriteRenderer>();
        // for(int i = FloorStartFromX; i < FloorEndAtX + 1; i++){
        //     for(int j = FloorStartFromY; j < FloorEndAtY + 1; j++){
        //         //create gameobject
        //         GameObject x = Instantiate(emptyObjectWhereGridClassIsPlaced,gameObject.transform);
        //         //name the gameobject in the scene
        //         x.name = "floor" + i.ToString() + " " + j.ToString();
        //         x.GetComponent<SpriteRenderer>().sortingOrder = -1;
        //         //create and pair a gridclass with each gameobject created
        //         GridClass gridClassForClone = new GridClass(x, new Vector3Int(i,j,0), true);
        //         //store the gridclass into dictionary
        //         GetComponentInParent<GridDictionaryStorage>().AddItemToDictionary(new Vector3Int(i, j, 0), gridClassForClone);
        //         //set up the gridclass(sprite and local position)
        //         gridClassForClone.SetUpAsAWhole();
        //     }
        // } 
    }


}
