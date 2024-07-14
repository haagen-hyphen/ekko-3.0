using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

[System.Serializable]
public class GameState
{
    public Vector3Int playerPosition;
    public Dictionary<Vector3Int, Cell> layer1;
    public Dictionary<Vector3Int, Cell> layer2;
    public Dictionary<Vector3Int, Cell> layer3;
    public Dictionary<Vector3Int, Cell> layer4;
    public List<Button> buttons;
    public List<ColourKey> colourKeys;
    public List<Enemy> enemies;

    public GameState(Vector3Int playerCurrPos, Dictionary<Vector3Int, Cell> currLayer1, Dictionary<Vector3Int, Cell> currLayer2, Dictionary<Vector3Int, Cell> currLayer3, Dictionary<Vector3Int, Cell> currLayer4)
    {
        playerPosition = playerCurrPos;
        layer1 = new Dictionary<Vector3Int, Cell>(currLayer1);
        layer2 = new Dictionary<Vector3Int, Cell>(currLayer2);
        layer3 = new Dictionary<Vector3Int, Cell>(currLayer3);
        layer4 = new Dictionary<Vector3Int, Cell>(currLayer4);
    }

    public void SetEnemies(List<Enemy> newEnemies){
        enemies = newEnemies.Where(enemy => enemy.timeImmune == false).Select(item => item.Clone()).ToList();
        //enemies = newEnemies.Select(item => item.Clone()).ToList();
    }

    public void SetButtons(List<Button> newButtons){
        buttons = newButtons.Where(button => button.timeImmune == false).Select(item => item.Clone()).ToList();
    }
    public void SetColourKeys(List<ColourKey> newColourKeys){
        colourKeys = newColourKeys.Where(colourKey => colourKey.timeImmune == false).Select(item => item.Clone()).ToList();
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
    public UIManager uIManager;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if(!uIManager.isGamePaused){
            if(Time.time/secondPerTick > tickPassed){
                tickPassed += 1;
                StoreGameState();
                CallEveryOtherAction();
            }
        }
    }

    void StoreGameState(){
        GameState currState = gridManager.GetGameState();
        currState.SetEnemies(enemyManager.enemies);
        currState.SetButtons(gridManager.buttons);
        gameStates.Add(currState);
    }
    void CallEveryOtherAction(){
        playerMovement.AnythingToBeDoneWheneverTicks(tickPassed);
        enemyManager.AnythingToBeDoneWheneverTicks(tickPassed);
        gridManager.AnythingToBeDoneWheneverTicks(tickPassed);
    }

    public void HandleDeath()
    {
        StartCoroutine(DeathCoroutine());
    }

    IEnumerator DeathCoroutine()
    {
        // Stop time
        Time.timeScale = 0f;

        // Wait for 2 seconds in real-time
        yield return new WaitForSecondsRealtime(2f);
        while(uIManager.isGamePaused){
            yield return null;
        }

        // Resume time
        Time.timeScale = 1f;

        // Revert to the game state from 5 ticks ago if possible
        if (gameStates.Count >= 5)
        {
            GameState previousState = gameStates[gameStates.Count - 5];
            GridManager.Instance.RevertGameState(previousState);
            gameStates.RemoveRange(gameStates.Count - 5, 5);
        }
        else if (gameStates.Count > 0)
        {
            GameState previousState = gameStates[0];
            GridManager.Instance.RevertGameState(previousState);
            gameStates = new();
        }
    }
}
