using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GeneralBosses : MonoBehaviour
{
    public float tickMyself;
    public static GeneralBosses Instance { get; private set; }
    public SlimeBossState currentState;
    public List<SlimeBossState> bossStates = new();
    
    void Awake(){
        if (Instance != null && Instance != this) 
        { 
            Destroy(this); 
        } 
        else 
        { 
            Instance = this; 
        }
        currentState = new SpawnSlimesAndWait();
    }

    // Update is called once per frame
    void Update()
    {
        if(Time.time - tickMyself > 0.5f){
            OnTick();
        }
        
    }

    public void OnTick(){
        tickMyself += 0.5f;
        currentState = currentState.Process();
        bossStates.Add(currentState);
    }
    public void RevertTime(){
        if (bossStates.Count >= 5)
        {
            currentState = bossStates[bossStates.Count - 5];
            bossStates.RemoveRange(bossStates.Count - 5, 5);
        }
        else if (bossStates.Count > 0)
        {
            currentState = bossStates[0];
            bossStates = new();
        }
        
    }

}
