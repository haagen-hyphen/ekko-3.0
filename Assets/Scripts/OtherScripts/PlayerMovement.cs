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
        if(Input.GetButtonDown("Fire1")){
           Debug.Log(MousePositionToCellPosition()); 
           EnemyManager.Instance.HandleGoblin(gridManager.playerPosition,5,new Vector3Int(1,0,0));
        }
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
        //can't walk don't walk
        if(!gridManager.CheckIfWalkable(aimedDestination)){
            moveBuffer = Vector3Int.zero;
        }
        //can walk see if got box
        else{
            var cell3 = GridManager.Instance.GetCell(3, aimedDestination);
            //no box then go
            if(cell3 == null){
                transform.position += moveBuffer;
            }
            //got box see if can push box(es)
            else if(gridManager.IsOfType<Box>(cell3)){
                Box box = (Box)cell3;
                //can push so push
                if(box.CheckPushability(aimedDestination+moveBuffer,moveBuffer)){
                    box.PushBoxes(aimedDestination,moveBuffer);
                    transform.position += moveBuffer;
                }
                //can't push don't walk
                else{
                    moveBuffer = Vector3Int.zero;
                }
            }
        }
        moveBuffer = Vector3Int.zero;
        gridManager.playerPosition = new Vector3Int((int)transform.position.x,(int)transform.position.y,(int)transform.position.z);
    }
    void CheckDeath(){
        if(gridManager.CheckIfLayer4HasObject(gridManager.playerPosition)){
            Die(gridManager.GetCell(4,gridManager.playerPosition));
        }
    }

    public void Die(Cell killedBy){
        moveBuffer = Vector3Int.zero;
        uIManager.SetAbilityImage(killedBy);

        //GetAbility
        tickManager.HandleDeath();
    }

    public Vector3Int MousePositionToCellPosition(){
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 cellPositionFloat = gridManager.GetLayer(1).WorldToCell(worldPosition);
        Vector3Int cellPosition = new Vector3Int((int)cellPositionFloat.x,(int)cellPositionFloat.y,0);
        return cellPosition;
        
    }
    
    


}
