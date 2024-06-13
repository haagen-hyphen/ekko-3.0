using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Ekko : MonoBehaviour
{
    Vector3[] _positionArray;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.R))
        {
            _positionArray[0] = new Vector3(Time.time,0,0);
            for(int i = 0; i < _positionArray.Length; i++)
            {
                Debug.Log(_positionArray[i]);
            }
        }   
    }
}
