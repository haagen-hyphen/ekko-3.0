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
    public EnemyManager enemyManager;

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
        enemyManager.AnythingToBeDoneWheneverTicks(tickPassed);
    }

    void HandleDeath()
    {
        StartCoroutine(DeathCoroutine());
    }

    IEnumerator DeathCoroutine()
    {
        // Stop time
        Time.timeScale = 0f;

        // Wait for 2 seconds in real-time
        yield return new WaitForSecondsRealtime(2f);

        // Resume time
        Time.timeScale = 1f;

        // Revert to the game state from 5 ticks ago if possible
        if (gameStates.Count >= 5)
        {
            GameState previousState = gameStates[gameStates.Count - 5];
            RevertToGameState(previousState);
            gameStates.RemoveRange(gameStates.Count - 5, 5);
        }
        else if (gameStates.Count > 0)
        {
            GameState previousState = gameStates[0];
            RevertToGameState(previousState);
            gameStates = new();
        }

        // Clear the death status
        playerMovement.isDead = false;
    }

    void RevertToGameState(GameState state)
    {
        gridManager.playerPosition = state.playerPosition;
        playerMovement.transform.position = state.playerPosition;
        gridManager.SetLayer(1, state.layer1);
        gridManager.SetLayer(2, state.layer2);
        gridManager.SetLayer(3, state.layer3);
        gridManager.SetButtons(state.buttons);
    }
}
