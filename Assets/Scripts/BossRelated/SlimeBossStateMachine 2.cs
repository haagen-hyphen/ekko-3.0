using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.Timeline;

public class SlimeBossState2
{
    public enum StateName{SpawnSlimes, State2};
    public enum Event{Enter, Ontick, Exit};
    public StateName stateName;
    public SlimeBossState2 nextState;
    public Event myEvent;


    protected Tilemap layer1;
    public SlimeBossState2(Tilemap _layer1){
        layer1 = _layer1;
        myEvent = Event.Enter;
    }
    public virtual void Enter(){myEvent = Event.Ontick;}
    public virtual void Ontick(){myEvent = Event.Ontick;}
    public virtual void Exit(){myEvent = Event.Exit;}
    public SlimeBossState2 Process(){
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
public class SpawnSlimes : SlimeBossState2
{

    public SpawnSlimes(Tilemap _layer1)
    : base(_layer1)
    {
        stateName = StateName.SpawnSlimes;
    }

    public override void Enter()
    {
        for (int i = layer1.cellBounds.min.x+1; i < layer1.cellBounds.max.x-3; i++)
            {
                for (int j = layer1.cellBounds.min.y+1; j < layer1.cellBounds.max.y-1; j++)
                {
                    int x = UnityEngine.Random.Range(1,40+1);
                    if(x == 1 && new Vector3Int(i,j,0)!=GridManager.Instance.playerPosition){
                        Slime s = new Slime(new Vector3Int(i,j,0));
                        EnemyManager.Instance.enemies.Add(s);
                    }
                }
        }
        base.Enter();
    }
    public override void Ontick()
    {
        if(EnemyManager.Instance.enemies.Count == 1){
            nextState = new State2(layer1);
            myEvent = Event.Exit;
        }
    }
    public override void Exit()
    {
        base.Exit();
    }
}

public class State2 : SlimeBossState2
{
    public State2(Tilemap _layer1)
    : base(_layer1)
    {
        stateName = StateName.SpawnSlimes;
    }
    public override void Enter()
    {
        Debug.Log("got to state2");
        base.Enter();
    }
    public override void Ontick()
    {

    }
    public override void Exit()
    {
        //what one should do when leaving Idle state
        base.Exit();
    }
}