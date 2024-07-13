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
    public static PlayerMovement Instance;

    // Start is called before the first frame update
    void Awake(){
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
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
            }else if (gridManager.GetCell(1, aimedDestination).isWalkableByKeyPlayerHolding)
            {
                transform.position += moveBuffer;
            }else{
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
        gridManager.playerLastPosition = gridManager.playerPosition;
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
            CallShootSpear(gridManager.playerPosition, shootingRange, shootBuffer/shootingRange);
        }
        shootBuffer = Vector3Int.zero;
    }
    public Vector3Int MousePositionToCellPosition(){
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 cellPositionFloat = gridManager.GetLayer(1).WorldToCell(worldPosition);
        Vector3Int cellPosition = new Vector3Int((int)cellPositionFloat.x,(int)cellPositionFloat.y,0);
        return cellPosition;
    }

    public void CallShootSpear(Vector3Int from, int shootingRange, Vector3Int unitDirection){
        StartCoroutine(ShootSpear(from, shootingRange, unitDirection));
    }
    public IEnumerator ShootSpear(Vector3Int from, int shootingRange, Vector3Int unitDirection){
        //no need to wait here because tickmng calls player after enemy, enemy alr lag 1 tick
        //+2 so the coroutine can be done in 0.5 sec, anyways the duration itself is not important
        yield return new WaitForSeconds(0.5f/(shootingRange+2));
        if(gridManager.CheckIfLayer3HasObject(from + 1*unitDirection) || !gridManager.CheckIfWalkable(from + 1*unitDirection)){
            EnemyManager.Instance.KillAnEnemy(from + 1*unitDirection);
            yield break;
        }
        gridManager.AdvancedSetCell(4, from + 1*unitDirection, gridManager.spear);
        yield return new WaitForSeconds(0.5f/(shootingRange+2));
        for(int i = 2; i <= shootingRange; i++){
            gridManager.AdvancedSetCell(4, from + (i-1)*unitDirection, null);
            if(gridManager.CheckIfLayer3HasObject(from + i*unitDirection) || !gridManager.CheckIfWalkable(from + i*unitDirection)){
                EnemyManager.Instance.KillAnEnemy(from + i*unitDirection);
                yield break;
            }
            gridManager.AdvancedSetCell(4, from + i*unitDirection, gridManager.spear);
            yield return new WaitForSeconds(0.5f/(shootingRange+2));
        }
        gridManager.AdvancedSetCell(4, from + shootingRange*unitDirection, null);
    }
    #endregion


    #region Death & Ability
    void CheckDeath(){
        if(gridManager.CheckIfLayer4HasObject(gridManager.playerPosition)){
            Die(gridManager.GetCell(4,gridManager.playerPosition));
        }
        else if(gridManager.CheckIfIsSlimyWall(gridManager.playerPosition) && !canPassSlimyWall){
            Die(gridManager.slimeDeadly);
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
