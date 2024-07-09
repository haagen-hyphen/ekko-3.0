using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.VisualScripting;
using UnityEditor.Tilemaps;
using UnityEditor.UI;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;
using UnityEngine.Serialization;
using UnityEngine.UIElements;

public class PlayerMovement : MonoBehaviour
{
    public TickManager tickManager;
    private Vector3Int moveBuffer;
    private float secondPerTick;
    public GridManager gridManager;
    public UIManager uIManager;
    public bool isDead = false;
    public string currentAbility = "Hand";
    public GameObject trajectory;
    public int shootingRange;
    public Vector3Int shootBuffer;
    Vector3[] startAndEndPoints = new Vector3[2];

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
        if(!uIManager.isGamePaused){
            StoreMoveBuffer(false);
            AimAndStoreShootBuffer();
            CancelAimIfNeeded();
        }
    }

    public void AnythingToBeDoneWheneverTicks(int tickPassed){
        StoreMoveBuffer(true);
        CheckDeath();
        MoveAndClearMoveBuffer();
        ShootAndClearShootBuffer();
    }
    #region walking
    void StoreMoveBuffer(bool onTickCall){
        if (!onTickCall)
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
        else
        {
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
        }
    }
    void MoveAndClearMoveBuffer(){
        Vector3Int currentPositionInt = new((int)transform.position.x,(int)transform.position.y,(int)transform.position.z);
        Vector3Int aimedDestination = currentPositionInt + moveBuffer;
        //can't walk don't walk
        if(!gridManager.CheckIfWalkable(aimedDestination)){
            if(gridManager.GetCell(1,aimedDestination).isSlimyWall && canPassSlimyWall){
                transform.position += moveBuffer;
            }
            else{
                moveBuffer = Vector3Int.zero;
            }
        }
        //can walk see if got box
        else{
            var cell3 = GridManager.Instance.GetCell(3, aimedDestination);
            //no box then go
            if(cell3 == null){
                transform.position += moveBuffer;
            }
            //got box see if can push box(es)
            else if(cell3.isPushable){
                //can push so push
                if(cell3.CheckNotBlocked(aimedDestination+moveBuffer,moveBuffer) && canPushThings){
                    cell3.PushBoxes(aimedDestination,moveBuffer);
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
    #endregion
    #region shooting spear
    bool intendedToShoot = false;
    public void AimAndStoreShootBuffer(){
        
        LineRenderer trajectory = GetComponentInChildren<LineRenderer>(true);
        if(Input.GetButton("Fire1") && canShoot){
            intendedToShoot = true;
            trajectory.enabled = true;
            Vector3Int difference = MousePositionToCellPosition() - gridManager.playerPosition;
            if(difference != Vector3Int.zero){
                if(Mathf.Abs(difference.x) > Mathf.Abs(difference.y)){
                startAndEndPoints[1] = new Vector3Int(shootingRange*difference.x/Mathf.Abs(difference.x),0,0);
                }
                else{
                startAndEndPoints[1] = new Vector3Int (0,shootingRange*difference.y/Mathf.Abs(difference.y),0);
                }
            }
            trajectory.SetPositions(startAndEndPoints);
        }
        if(Input.GetButtonUp("Fire1") && canShoot && intendedToShoot){
            trajectory.enabled = false;
            shootBuffer = new Vector3Int ((int)startAndEndPoints[1].x, (int)startAndEndPoints[1].y,0);
            intendedToShoot = false;
        }
    }
    void CancelAimIfNeeded(){
        if(Input.GetButton("Fire2")){
            LineRenderer trajectory = GetComponentInChildren<LineRenderer>(true);
            shootBuffer = Vector3Int.zero;
            trajectory.enabled = false;
        }
    }
    void ShootAndClearShootBuffer(){
        if(shootBuffer != Vector3Int.zero){
            EnemyManager.Instance.HandleGoblin(gridManager.playerPosition, shootingRange, shootBuffer/shootingRange);
        }
        shootBuffer = Vector3Int.zero;
    }
    public Vector3Int MousePositionToCellPosition(){
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 cellPositionFloat = gridManager.GetLayer(1).WorldToCell(worldPosition);
        Vector3Int cellPosition = new Vector3Int((int)cellPositionFloat.x,(int)cellPositionFloat.y,0);
        return cellPosition;
    }
    #endregion


    #region Death & Ability
    void CheckDeath(){
        if(gridManager.CheckIfLayer4HasObject(gridManager.playerPosition)){
            Die(gridManager.GetCell(4,gridManager.playerPosition));
        }
    }

    //2 ways to die: CheckDeath every tick, and, setTile(4,) at playerPosition
    public void Die(Cell killedBy){
        moveBuffer = Vector3Int.zero;
        uIManager.SetAbilityImage(killedBy);
        currentAbility = killedBy.abilityName;
        SetAbilityToNone();
        SetAbility(currentAbility);
        EnemyManager.Instance.StopAllCoroutines(); //fix the bug where the spear keeps moving after player death
        tickManager.HandleDeath();
        try{
            Boss.Instance.RevertTime();
        }
        catch{}
        
    }

    public void SetAbilityToNone(){
        //revert hand
        canPushThings = false;
        //revert slime
        canPassSlimyWall = false;
        //revert spear
        canShoot = false;
    }
    bool canPushThings = true;
    bool canPassSlimyWall;
    bool canShoot;
    public void SetAbility(string currentAbility){
        switch(currentAbility){
            case "Hand":
                canPushThings = true;
                break;
            case "Spear":
                canShoot = true;
                break;
            case "Slime":
                canPassSlimyWall = true;
                break;
        }
    }
    #endregion







}
