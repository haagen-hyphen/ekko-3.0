using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DelAnyTIme : MonoBehaviour
{
    public float moveCd = 0.5f;
    private float _lastMoveTimeStamp;
        
    // Start is called before the first frame update
    void Start()
    {
        _lastMoveTimeStamp = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time - _lastMoveTimeStamp > moveCd)
        {
            _lastMoveTimeStamp += moveCd;
            
        }

        transform.localScale = new Vector3((Time.time - _lastMoveTimeStamp) * 8, 2, 1) ;
        transform.position = new Vector3(-6 + (Time.time - _lastMoveTimeStamp) * 4, 3, 0);
    }
    
    
}
