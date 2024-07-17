using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.Timeline;

public class SlimeBossState
{
    public enum StateName{SpawnSlimes, Rolling, State2};
    public enum Event{Enter, Ontick, Exit};
    public StateName stateName;
    public SlimeBossState nextState;
    public Event myEvent;


    protected Tilemap layer1;
    protected Transform player;
    protected GameObject boss;
    public SlimeBossState(Tilemap _layer1, Transform _player, GameObject _boss){
        layer1 = _layer1;
        player = _player;
        boss = _boss;
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
}
public class SpawnSlimes : SlimeBossState
{

    public SpawnSlimes(Tilemap _layer1, Transform _player, GameObject _boss)
    : base(_layer1, _player, _boss)
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
                        Slime s = new Slime(new Vector3Int(i,j,0), 2, 3);
                        EnemyManager.Instance.enemies.Add(s);
                    }
                }
        }
        base.Enter();
    }
    public override void Ontick()
    {
        if(EnemyManager.Instance.enemies.Count == 1){   //bug: if player die immediately
            nextState = new Rolling(layer1, player, boss);
            myEvent = Event.Exit;
        }
    }
    public override void Exit()
    {
        base.Exit();
    }
}

public class Rolling : SlimeBossState
{
    public Rolling(Tilemap _layer1, Transform _player, GameObject _boss)
    : base(_layer1, _player, _boss)
    {
        stateName = StateName.Rolling;
    }
    public override void Enter()
    {
        base.Enter();
    }
    public override void Ontick()
    {
        boss.transform.position = player.position;
    }
    public override void Exit()
    {
        //what one should do when leaving Idle state
        base.Exit();
    }
}

public class State2 : SlimeBossState
{
    public State2(Tilemap _layer1, Transform _player, GameObject _boss)
    : base(_layer1, _player, _boss)
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