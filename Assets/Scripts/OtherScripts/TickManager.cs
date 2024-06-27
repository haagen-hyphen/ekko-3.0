using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[System.Serializable]
public class GameState
{
    public Vector3Int playerPosition;
    public Tilemap layer1;
    public Tilemap layer2;
    public Tilemap layer3;
    public List<Button> buttons;

    public GameState(Vector3Int playerCurrPos, Tilemap currLayer1, Tilemap currLayer2, Tilemap currLayer3, List<Button> currButtons)
    {
        playerPosition = playerCurrPos;
        layer1 = currLayer1;
        layer2 = currLayer2;
        layer3 = currLayer3;
        buttons = currButtons;
    }
}

public class TickManager : MonoBehaviour
{
    [HideInInspector]public int tickPassed = 0;
    public float secondPerTick;

    public List<GameState> gameStates = new();

    public PlayerMovement playerMovement;
    public GridManager gridManager;

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
        gameStates.Add(gridManager.GetGameState());
        gridManager.AnythingToBeDoneWheneverTicks(tickPassed);
    }
}
