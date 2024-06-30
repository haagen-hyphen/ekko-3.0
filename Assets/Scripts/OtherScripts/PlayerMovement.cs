using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.UI;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;
using UnityEngine.Serialization;
using UnityEngine.UIElements;

public class PlayerMovement : MonoBehaviour
{
    private float lastMoveTimeStamp;
    public float gracePeriod;
    public TickManager tickManager;
    private Vector3Int moveBuffer;
    private float secondPerTick;
    public GridManager gridManager;
    public UIManager uIManager;
    public bool isDead = false;
    
    // Start is called before the first frame update
    void Awake(){
        gridManager.playerPosition = new Vector3Int((int)transform.position.x,(int)transform.position.y,(int)transform.position.z);
    }
    
    void Start()
    {
        secondPerTick = tickManager.secondPerTick;
    }

    // Update is called once per frame
    void Update()
    {
        StoreMoveBuffer();
    }

    public void AnythingToBeDoneWheneverTicks(int tickPassed){
        CheckDeath();
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
        Vector3Int currentPositionInt = new((int)transform.position.x,(int)transform.position.y,(int)transform.position.z);
        Vector3Int aimedDestination = currentPositionInt + moveBuffer;
        Vector3Int positionToBePushedTo = currentPositionInt + 2*moveBuffer;        //for button
        if(!gridManager.CheckIfWalkable(aimedDestination)){
            moveBuffer = Vector3Int.zero;
            return;
        }
        if(gridManager.CheckIfWalkable(aimedDestination)){
            if(gridManager.CheckIfLayer3HasObject(aimedDestination)){
                if(gridManager.CheckIfPushable(aimedDestination)){
                    if(gridManager.CheckIfWalkable(positionToBePushedTo)){
                        gridManager.MoveTile(3, aimedDestination, positionToBePushedTo);
                        transform.position += moveBuffer;
                    }
                }
            }
            else{
                transform.position += moveBuffer;
            }
        }
        moveBuffer = Vector3Int.zero;
        gridManager.playerPosition = new Vector3Int((int)transform.position.x,(int)transform.position.y,(int)transform.position.z);
    }
    void CheckDeath(){
        if(gridManager.CheckIfLayer4HasObject(gridManager.playerPosition)){
            Die(gridManager.GetCell(4,gridManager.playerPosition));
            Debug.Log("died bcoz checkdeath");
        }
    }

    public void Die(Cell killedBy){
        moveBuffer = Vector3Int.zero;
        uIManager.SetAbilityImage(killedBy);

        //GetAbility
        tickManager.HandleDeath();
    }


}
