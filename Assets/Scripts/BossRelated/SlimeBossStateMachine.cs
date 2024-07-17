using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.Timeline;

public class SlimeBossState
{
    public enum StateName{SpawnSlimesAndWait, Attack};
    public enum Event{Enter, Ontick, Exit};
    public StateName stateName;
    public SlimeBossState nextState;
    public Event myEvent;
    public SlimeBossState(){
        myEvent = Event.Enter;
    }
    public virtual void Enter(){myEvent = Event.Ontick;}
    public virtual void Ontick(){myEvent = Event.Ontick;}
    public virtual void Exit(){myEvent = Event.Exit;}
    public SlimeBossState Process(){
        if(myEvent == Event.Enter){
            Enter();
        }
        if(myEvent == Event.Ontick){
            Ontick();
        }
        if(myEvent == Event.Exit){
            Exit();
            return nextState;
        }
        return this;
    }

    public SpriteRenderer self;
    public SpriteRenderer shadow;
    public bool jumping;
    public Cell slimeDeadly;
    public Cell floor;
    public Cell slimyWall;
    public Tilemap layer1;

    #region jump
    IEnumerator JumpTillLand(){
        Vector3Int aimLandAt = DecideWhereToLand();
        //jump
        jumping=true;
        // SetCells(4, Vector3ToVector3Int(transform.position),null);
        // transform.position = aimLandAt;
        shadow.enabled = true;
        self.enabled = false;
        
        yield return new WaitForSeconds(1.5f);
        //land
        shadow.enabled = false;
        self.enabled = true;
        
        SetCells(4, aimLandAt, slimeDeadly);
        jumping=false;
    }

    Vector3Int DecideWhereToLand(){
        Vector3Int playerPosition = GridManager.Instance.playerPosition;
        if(playerPosition.y < -4){
            playerPosition.y = -4;
        }
        else if(playerPosition.y>3){
            playerPosition.y=3;
        }
        if(playerPosition.x<-7){
            playerPosition.x=-7;
        }
        else if(playerPosition.x>6){
            playerPosition.x=6;
        }
        return new Vector3Int(playerPosition.x,playerPosition.y,0);
    }
    public void SetCells(int layer, Vector3Int Mid, Cell toSet){
        for(int i=-1; i<=1;i++){
            for(int j=-1;j<=1;j++){
                Vector3Int xError = new Vector3Int(i,0,0);
                Vector3Int yError = new Vector3Int(0,j,0);
                GridManager.Instance.SetCell(layer, Mid+xError+yError, toSet);
            }
        }
    }
#endregion


#region spawn slimes
    
#endregion

#region others
    Vector3Int Vector3ToVector3Int(Vector3 v){ //boss
        return new Vector3Int((int)v.x, (int)v.y, (int)v.z);
    }
#endregion
}

public class SpawnSlimesAndWait : SlimeBossState
{
    public SpawnSlimesAndWait()
    : base()
    {
        stateName = StateName.SpawnSlimesAndWait;
    }
    public override void Enter()
    {
        SpawnSlimesRandomly(40);
        base.Enter();
    }
    public override void Ontick()
    {
        if(Input.GetKey(KeyCode.Q)){        //actually should be "if all slimes died"
            nextState = new Attack();
            myEvent = Event.Exit;
        }
    }
    public override void Exit()
    {
        base.Exit();
    }



    #region f() for SSAW
    public void SpawnSlimesRandomly(int ratio){
        for (int i = layer1.cellBounds.min.x+1; i < layer1.cellBounds.max.x-3; i++)
            {
                for (int j = layer1.cellBounds.min.y+1; j < layer1.cellBounds.max.y-1; j++)
                {
                    int x = Random.Range(1,ratio+1);
                    if(x == 1 && new Vector3Int(i,j,0)!=GridManager.Instance.playerPosition){
                        Slime s = new Slime(new Vector3Int(i,j,0));
                        EnemyManager.Instance.enemies.Add(s);
                    }
                }
            }
    }
    #endregion
}

    public class Attack : SlimeBossState
{
    public Attack()
    : base()
    {
        stateName = StateName.Attack;
    }
    public override void Enter()
    {
        //what to do when first get into Attack state
        base.Enter();
    }
    public override void Ontick()
    {
        // if(something happened){
        //     nextState = new Patrol();
        //     myEvent = Event.Exit;
        // }

        // what one should do during Attack state
    }
    public override void Exit()
    {
        //what one should do when Exiting Attack state
        base.Exit();
    }
}