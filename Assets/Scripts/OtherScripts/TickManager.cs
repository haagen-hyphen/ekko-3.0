using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TickManager : MonoBehaviour
{
    [HideInInspector]public int tickPassed = 0;
    public float secondPerTick;

    public PlayerMovement playerMovement;
    public GridManagerScript gridManagerScript;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Time.time/secondPerTick > tickPassed){
            tickPassed += 1;
            CallEveryOtherAction();
        }
    }
    void CallEveryOtherAction(){
        playerMovement.AnythingToBeDoneWheneverTicks(tickPassed);
        gridManagerScript.AnythingToBeDoneWheneverTicks(tickPassed);
    }
}
