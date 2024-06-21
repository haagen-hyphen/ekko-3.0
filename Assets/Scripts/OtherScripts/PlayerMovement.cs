using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.UI;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

public class PlayerMovement : MonoBehaviour
{
    private float lastMoveTimeStamp;
    public float gracePeriod;
    public GameObject tickManagerGameObj;
    private Vector3Int moveBuffer;
    private float secondPerTick;
    public GameObject gridManager;
    private GridManagerScript gridManagerScript;
    
    // Start is called before the first frame update
    void Start()
    {
        secondPerTick = tickManagerGameObj.GetComponent<TickManager>().secondPerTick;
        gridManagerScript = gridManager.GetComponent<GridManagerScript>();
    }

    // Update is called once per frame
    void Update()
    {
        StoreMoveBuffer();
    }

    public void AnythingToBeDoneWheneverTicks(int tickPassed){
        lastMoveTimeStamp = Time.time;
        MoveAndClearMoveBuffer();
    }

    void StoreMoveBuffer(){
        if (Time.time - lastMoveTimeStamp > secondPerTick - gracePeriod)
            {

                if (Input.GetKeyDown(KeyCode.W))
                {
                    moveBuffer = new Vector3Int(0, 1, 0);
                }
                else if (Input.GetKeyDown(KeyCode.S))
                {
                    moveBuffer = new Vector3Int(0, -1, 0);
                }
                else if (Input.GetKeyDown(KeyCode.A))
                {
                    moveBuffer = new Vector3Int(-1, 0, 0);
                }
                else if (Input.GetKeyDown(KeyCode.D))
                {
                    moveBuffer = new Vector3Int(1, 0, 0);
                }
            }
        else{
            if (Input.GetKey(KeyCode.W))
            {
                moveBuffer = new Vector3Int(0, 1, 0);

            }
            else if (Input.GetKey(KeyCode.S))
            {
                moveBuffer = new Vector3Int(0, -1, 0);
            }
            else if (Input.GetKey(KeyCode.A))
            {
                moveBuffer = new Vector3Int(-1, 0, 0);
            }
            else if (Input.GetKey(KeyCode.D))
            {
                moveBuffer = new Vector3Int(1, 0, 0);
            }
            else
            {
                moveBuffer = Vector3Int.zero;
            }
            }
    
    }
    void MoveAndClearMoveBuffer(){
        Vector3Int currentPositionInt = new Vector3Int((int)transform.position.x,(int)transform.position.y,(int)transform.position.z);
        Vector3Int aimedDestination = currentPositionInt + moveBuffer;
        try {
            if(gridManagerScript.CheckIfWalkable(aimedDestination) == true){                    //if it's not a wall
                if(gridManagerScript.CheckIfInteractable(aimedDestination) == true){
                    if(gridManagerScript.CheckIfPushable(aimedDestination) == true){            //and it's a box, but here will jump to catch
                        Vector3Int positionToBePushedTo = currentPositionInt += 2*moveBuffer;
                        if(gridManagerScript.CheckIfWalkable(positionToBePushedTo) == true){    //and behind got space
                            gridManagerScript.MoveTile(aimedDestination, positionToBePushedTo);     //push it
                            transform.position += moveBuffer;
                        }
                    }
                }
                else{                                                                           //else it is not a box, just walk
                    transform.position += moveBuffer;
                }
            }
        }
        catch{}
        moveBuffer = Vector3Int.zero;
    }

    // BELOW ARE THINGS RELATED TO EKKO
    // if (Time.time - _lastMoveTimeStamp > moveCd)
    //         {   _positionArray.Add(transform.position);
    //             numberOfTickPassed += 1;
    //             playerShadow.transform.position = _positionArray[numberOfTickPassed - 5];

    // if(Input.GetKeyDown(KeyCode.F))
    //     {
    //         transform.position = _positionArray[numberOfTickPassed - 5];
    //         playerShadow.transform.position = _positionArray[numberOfTickPassed - 2*5 ];
    //         _positionArray.RemoveRange(_positionArray.Count-5,5);
    //         numberOfTickPassed -= 5;
    //     }

}
