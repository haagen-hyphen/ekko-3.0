using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[System.Serializable]
public class GameState
{
    public Vector3Int playerPosition;
    public Dictionary<Vector3Int, Cell> layer1;
    public Dictionary<Vector3Int, Cell> layer2;
    public Dictionary<Vector3Int, Cell> layer3;
    public List<Button> buttons;
    public List<Slime> slimes;

    public GameState(Vector3Int playerCurrPos, Dictionary<Vector3Int, Cell> currLayer1, Dictionary<Vector3Int, Cell> currLayer2, Dictionary<Vector3Int, Cell> currLayer3, List<Button> currButtons)
    {
        playerPosition = playerCurrPos;
        layer1 = new Dictionary<Vector3Int, Cell>(currLayer1);
        layer2 = new Dictionary<Vector3Int, Cell>(currLayer2);
        layer3 = new Dictionary<Vector3Int, Cell>(currLayer3);
        buttons = new List<Button>(currButtons);
    }

    public void SetSlimes(List<Slime> newSlimes){
        slimes = new List<Slime>(newSlimes);
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
        if (playerMovement.isDead)
        {
            HandleDeath();
        }

        GameState currState = gridManager.GetGameState();
        currState.SetSlimes(enemyManager.slimes);
        gameStates.Add(currState);

        enemyManager.AnythingToBeDoneWheneverTicks(tickPassed);
        playerMovement.AnythingToBeDoneWheneverTicks(tickPassed);
        gridManager.AnythingToBeDoneWheneverTicks(tickPassed);
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
        gridManager.DictToTilemap(1, state.layer1);
        gridManager.DictToTilemap(2, state.layer2);
        gridManager.DictToTilemap(3, state.layer3);
        gridManager.SetButtons(state.buttons);
        enemyManager.slimes = new List<Slime>(state.slimes);
    }
}
