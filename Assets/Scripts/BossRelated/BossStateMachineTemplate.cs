using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.Timeline;

//how to use:
//rename WhatBossState to your boss's name
//change enum StateName to your boss's states
//for each state, make a class for 1 (refer to the example below)

public class WhatBossState : MonoBehaviour
{
    public enum StateName{State1, State2};
    public enum Event{Enter, Ontick, Exit};
    public StateName stateName;
    public WhatBossState nextState;
    public Event myEvent;
    public WhatBossState(){
        myEvent = Event.Enter;
    }
    public virtual void Enter(){myEvent = Event.Ontick;}
    public virtual void Ontick(){myEvent = Event.Ontick;}
    public virtual void Exit(){myEvent = Event.Exit;}
    public WhatBossState Process(){
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
}

public class State1 : WhatBossState
{
    public State1()
    : base()
    {
        stateName = StateName.State1;
    }
    public override void Enter()
    {
        //what to do when first enter State1
        base.Enter();
    }
    public override void Ontick()
    {
        // if(something that should add this state happened){
        //     nextState = new Patrol();
        //     myEvent = Event.Exit;
        //     (and maybe you can consider return here if you don't want Ontick to run one more tick)
        // }

        // what one should do during Idle state
    }
    public override void Exit()
    {
        //what one should do when leaving Idle state
        base.Exit();
    }
}