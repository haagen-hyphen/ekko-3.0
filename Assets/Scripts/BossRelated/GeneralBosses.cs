using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GeneralBosses : MonoBehaviour
{
    private float tickMyself;
    public static GeneralBosses Instance { get; private set; }
    public SlimeBossState2 currentState;
    public List<SlimeBossState2> bossStates = new();

    [Header("Something every bosses need")]
    public Tilemap layer1;
    //animator, playerTransform, etc.

    void Start(){
        currentState = new SpawnSlimes(layer1);
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
