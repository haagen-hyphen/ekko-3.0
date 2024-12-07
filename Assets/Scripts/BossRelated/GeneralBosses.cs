using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GeneralBosses : MonoBehaviour
{
    private float tickMyself;
    public static GeneralBosses Instance { get; private set; }
    public SlimeBossState currentState;
    public List<SlimeBossState> bossStates = new();

    [Header("Something every bosses need")]
    public Tilemap layer1;
    public Transform player;
    public GameObject boss;

    void Start(){
        currentState = new SpawnSlimes(layer1, player, boss);
    }


    public void AnythingToBeDoneWheneverTicks(int tickPassed){
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
