using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeLine : MonoBehaviour
{
    public float moveCd = 0.5f;
    public float delay = 0.05f;
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
        
        transform.position = new Vector3(-(Time.time - _lastMoveTimeStamp)*(1/moveCd) + delay, 0, 0);
    }
}
