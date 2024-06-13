using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerMovement : MonoBehaviour
{
    // Start is called before the first frame update
    public float moveCd = 0.5f;
    public int moveMode;
    public float gracePeriod = 0.2f;


    private Vector3 _moveBuffer;
    private float _lastMoveTimeStamp;
    void Start()
    {
        _lastMoveTimeStamp = 0;
        _moveBuffer = Vector3.zero;
    }

    // Update is called once per frame

    private void Update()
    {
        if (moveMode == 1)
        {
            if (Time.time - _lastMoveTimeStamp > moveCd)
            {
                if (Input.GetKey(KeyCode.W))
                {
                    transform.position += new Vector3(0, 1, 0);
                    _lastMoveTimeStamp = Time.time;
                }
                else if (Input.GetKey(KeyCode.S))
                {
                    transform.position += new Vector3(0, -1, 0);
                    _lastMoveTimeStamp = Time.time;
                }
                else if (Input.GetKey(KeyCode.A))
                {
                    transform.position += new Vector3(-1, 0, 0);
                    _lastMoveTimeStamp = Time.time;
                }
                else if (Input.GetKey(KeyCode.D))
                {
                    transform.position += new Vector3(1, 0, 0);
                    _lastMoveTimeStamp = Time.time;
                }
                

            }
        }else if (moveMode == 2)
        {
            if (_moveBuffer == Vector3.zero)
            {
                if (Input.GetKeyDown(KeyCode.W))
                {
                    _moveBuffer = new Vector3(0, 1, 0);
                    
                }
                else if (Input.GetKeyDown(KeyCode.S))
                {
                    _moveBuffer = new Vector3(0, -1, 0);
                }
                else if (Input.GetKeyDown(KeyCode.A))
                {
                    _moveBuffer = new Vector3(-1, 0, 0);
                }
                else if (Input.GetKeyDown(KeyCode.D))
                {
                    _moveBuffer = new Vector3(1, 0, 0);
                }
            }

            if (Time.time - _lastMoveTimeStamp > moveCd)
            {
                transform.position += _moveBuffer;
                _moveBuffer = Vector3.zero;
                _lastMoveTimeStamp = Time.time;
                
            }
            
        }else if (moveMode == 3)
        {

            if (Input.GetKey(KeyCode.W))
            {
                _moveBuffer = new Vector3(0, 1, 0);
                
            }
            else if (Input.GetKey(KeyCode.S))
            {
                _moveBuffer = new Vector3(0, -1, 0);
            }
            else if (Input.GetKey(KeyCode.A))
            {
                _moveBuffer = new Vector3(-1, 0, 0);
            }
            else if (Input.GetKey(KeyCode.D))
            {
                _moveBuffer = new Vector3(1, 0, 0);
            }
            else
            {
                _moveBuffer = Vector3.zero;
            }


            if (Time.time - _lastMoveTimeStamp > moveCd)
            {
                transform.position += _moveBuffer;
                _moveBuffer = Vector3.zero;
                _lastMoveTimeStamp += moveCd;
                
            }
            
        }else if (moveMode == 4)
        {
            if (Time.time - _lastMoveTimeStamp > moveCd - gracePeriod)
            {

                if (Input.GetKeyDown(KeyCode.W))
                {
                    _moveBuffer = new Vector3(0, 1, 0);
                
                }
                else if (Input.GetKeyDown(KeyCode.S))
                {
                    _moveBuffer = new Vector3(0, -1, 0);
                }
                else if (Input.GetKeyDown(KeyCode.A))
                {
                    _moveBuffer = new Vector3(-1, 0, 0);
                }
                else if (Input.GetKeyDown(KeyCode.D))
                {
                    _moveBuffer = new Vector3(1, 0, 0);
                }
            }else
            {
                if (Input.GetKey(KeyCode.W))
                {
                    _moveBuffer = new Vector3(0, 1, 0);

                }
                else if (Input.GetKey(KeyCode.S))
                {
                    _moveBuffer = new Vector3(0, -1, 0);
                }
                else if (Input.GetKey(KeyCode.A))
                {
                    _moveBuffer = new Vector3(-1, 0, 0);
                }
                else if (Input.GetKey(KeyCode.D))
                {
                    _moveBuffer = new Vector3(1, 0, 0);
                }
                else
                {
                    _moveBuffer = Vector3.zero;
                }
            }

            if (Time.time - _lastMoveTimeStamp > moveCd)
            {
                transform.position += _moveBuffer;
                _moveBuffer = Vector3.zero;
                _lastMoveTimeStamp += moveCd;
                
            }
            
        }
        
    }
}
