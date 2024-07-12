using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossState
{
    public enum StateName{Idle, Patrol, Attack};
    public enum Event{Enter, Update, Exit};
    public StateName stateName;
    public BossState nextState;
    public Event myEvent;
    public BossState(){
        myEvent = Event.Enter;
    }
    public virtual void Enter(){myEvent = Event.Update;}
    public virtual void Update(){myEvent = Event.Update;}
    public virtual void Exit(){myEvent = Event.Exit;}
    public BossState Process(){
        if(myEvent == Event.Enter){
            Enter();
        }
        if(myEvent == Event.Update){
            Update();
        }
        if(myEvent == Event.Exit){
            Exit();
            return nextState;
        }
        return this;
    }

}

public class Idle : BossState
{
    public Idle()
    : base()
    {
        stateName = StateName.Idle;
    }
    public override void Enter()
    {
        //what one should do when first get into Idle state
        base.Enter();
    }
    public override void Update()
    {
        // if(something happened){
        //     nextState = new Patrol();
        //     myEvent = Event.Exit;
        // }

        // what one should do during Idle state
        base.Update();//??weird
    }
    public override void Exit()
    {
        //what one should do when leaving Idle state
        base.Exit();
    }
}
