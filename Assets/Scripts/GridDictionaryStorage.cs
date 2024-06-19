using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridDictionaryStorage : MonoBehaviour
{
    public Dictionary<Vector3Int, GridClass> positionGridPairDictionary = new Dictionary<Vector3Int, GridClass>();
    // Start is called before the first frame update
    void Start()
    {
        positionGridPairDictionary[new Vector3Int(3,4,0)].SetNotWalkable();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddItemToDictionary(Vector3Int position, GridClass gridClass){
        positionGridPairDictionary[position] = gridClass;
    }
}
