using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Boss : MonoBehaviour
{
    public Cell slimeDeadly;
    public Cell floor;
    public Cell slimyWall;
    public Tilemap layer1;
    public int mode;
    private Dictionary<int,int> modeXPair = new();
    private List<int> modeOverTime = new();
    public bool jumping;
    public float tickMyself;
    public static Boss Instance { get; private set; }
    public SpriteRenderer self;
    public SpriteRenderer shadow;
    public BossState currentState;
    // Start is called before the first frame update
    
    void Awake(){
        if (Instance != null && Instance != this) 
        { 
            Destroy(this); 
        } 
        else 
        { 
            Instance = this; 
        }
        mode = 1;
        modeXPair[1]=-7;
        modeXPair[2]=-4;
        modeXPair[3]=-1;
        modeXPair[4]=0;
        modeXPair[5]=5;
    }

    // Update is called once per frame
    void Update()
    {
        if(Time.time - tickMyself > 0.5f){
            OnTick();
        }
        if(Input.GetKeyDown(KeyCode.Q)){
            SpawnSlimesRandomly(20);
        }
        
    }

    public void OnTick(){
        tickMyself += 0.5f;
        modeOverTime.Add(mode);
        if(!jumping && mode < 6){
            StartCoroutine(JumpTillLand());
            SpawnWalls(mode,DecideWhereToSpawnWalls());
            //actually should land then spawn wall will feel better, currently the coroutine is weird
        }
        else if(mode == 6){
            SpawnSlimesRandomly(20);
            mode += 1;
        }
    }
    public void RevertTime(){
        if(modeOverTime.Count >= 5){
            mode = modeOverTime[modeOverTime.Count - 5];
            modeOverTime.RemoveRange(modeOverTime.Count - 5, 5);
        }
        else if (modeOverTime.Count > 0){
            mode = modeOverTime[0];
            modeOverTime = new();
        }
        
    }

#region jump
    IEnumerator JumpTillLand(){
        Vector3Int aimLandAt = DecideWhereToLand();
        //jump
        jumping=true;
        SetCells(4, Vector3ToVector3Int(transform.position),null);
        transform.position = aimLandAt;
        shadow.enabled = true;
        self.enabled = false;
        
        yield return new WaitForSeconds(1.5f);
        //land
        shadow.enabled = false;
        self.enabled = true;
        
        SetCells(4, aimLandAt, slimeDeadly);
        jumping=false;
    }

    Vector3Int DecideWhereToLand(){
        Vector3Int playerPosition = GridManager.Instance.playerPosition;
        if(playerPosition.y < -4){
            playerPosition.y = -4;
        }
        else if(playerPosition.y>3){
            playerPosition.y=3;
        }
        if(playerPosition.x<-7){
            playerPosition.x=-7;
        }
        else if(playerPosition.x>6){
            playerPosition.x=6;
        }
        return new Vector3Int(playerPosition.x,playerPosition.y,0);
    }
    void SetCells(int layer, Vector3Int Mid, Cell toSet){
        for(int i=-1; i<=1;i++){
            for(int j=-1;j<=1;j++){
                Vector3Int xError = new Vector3Int(i,0,0);
                Vector3Int yError = new Vector3Int(0,j,0);
                GridManager.Instance.SetCell(layer, Mid+xError+yError, toSet);
            }
        }
    }
#endregion

#region spawn walls
    Vector3Int DecideWhereToSpawnWalls(){
        Vector3Int playerPosition = GridManager.Instance.playerPosition;
        if(playerPosition.y < -3){
            playerPosition.y = -3;
        }
        else if(playerPosition.y>2){
            playerPosition.y=2;
        }
        return new Vector3Int(modeXPair[mode],playerPosition.y,0);
    }
    void SpawnWalls(int localMode, Vector3Int leftMostMidPoint){
        switch(localMode){
            case 1: //S
                GridManager.Instance.AdvancedSetCell(1,leftMostMidPoint + new Vector3Int(2,2,0), slimyWall);
                GridManager.Instance.AdvancedSetCell(1,leftMostMidPoint + new Vector3Int(1,2,0), slimyWall);
                GridManager.Instance.AdvancedSetCell(1,leftMostMidPoint + new Vector3Int(0,2,0), slimyWall);
                GridManager.Instance.AdvancedSetCell(1,leftMostMidPoint + new Vector3Int(0,1,0), slimyWall);
                GridManager.Instance.AdvancedSetCell(1,leftMostMidPoint + new Vector3Int(0,0,0), slimyWall);
                GridManager.Instance.AdvancedSetCell(1,leftMostMidPoint + new Vector3Int(1,0,0), slimyWall);
                GridManager.Instance.AdvancedSetCell(1,leftMostMidPoint + new Vector3Int(2,0,0), slimyWall);
                GridManager.Instance.AdvancedSetCell(1,leftMostMidPoint + new Vector3Int(2,-1,0), slimyWall);
                GridManager.Instance.AdvancedSetCell(1,leftMostMidPoint + new Vector3Int(2,-2,0), slimyWall);
                GridManager.Instance.AdvancedSetCell(1,leftMostMidPoint + new Vector3Int(1,-2,0), slimyWall);
                GridManager.Instance.AdvancedSetCell(1,leftMostMidPoint + new Vector3Int(0,-2,0), slimyWall);
                mode += 1;
                break;
                
            case 2: //L
                GridManager.Instance.AdvancedSetCell(1,leftMostMidPoint + new Vector3Int(0,2,0), slimyWall);
                GridManager.Instance.AdvancedSetCell(1,leftMostMidPoint + new Vector3Int(0,1,0), slimyWall);
                GridManager.Instance.AdvancedSetCell(1,leftMostMidPoint + new Vector3Int(0,0,0), slimyWall);
                GridManager.Instance.AdvancedSetCell(1,leftMostMidPoint + new Vector3Int(0,-1,0), slimyWall);
                GridManager.Instance.AdvancedSetCell(1,leftMostMidPoint + new Vector3Int(0,-2,0), slimyWall);
                GridManager.Instance.AdvancedSetCell(1,leftMostMidPoint + new Vector3Int(1,-2,0), slimyWall);
                GridManager.Instance.AdvancedSetCell(1,leftMostMidPoint + new Vector3Int(2,-2,0), slimyWall);
                mode += 1;
                break;
                 
            case 3: //I
                GridManager.Instance.AdvancedSetCell(1,leftMostMidPoint + new Vector3Int(0,2,0), slimyWall);
                GridManager.Instance.AdvancedSetCell(1,leftMostMidPoint + new Vector3Int(0,1,0), slimyWall);
                GridManager.Instance.AdvancedSetCell(1,leftMostMidPoint + new Vector3Int(0,0,0), slimyWall);
                GridManager.Instance.AdvancedSetCell(1,leftMostMidPoint + new Vector3Int(0,-1,0), slimyWall);
                GridManager.Instance.AdvancedSetCell(1,leftMostMidPoint + new Vector3Int(0,-2,0), slimyWall);
                mode += 1;
                break;
            case 4: //M
                GridManager.Instance.AdvancedSetCell(1,leftMostMidPoint + new Vector3Int(0,2,0), slimyWall);
                GridManager.Instance.AdvancedSetCell(1,leftMostMidPoint + new Vector3Int(0,1,0), slimyWall);
                GridManager.Instance.AdvancedSetCell(1,leftMostMidPoint + new Vector3Int(0,0,0), slimyWall);
                GridManager.Instance.AdvancedSetCell(1,leftMostMidPoint + new Vector3Int(0,-1,0), slimyWall);
                GridManager.Instance.AdvancedSetCell(1,leftMostMidPoint + new Vector3Int(0,-2,0), slimyWall);
                GridManager.Instance.AdvancedSetCell(1,leftMostMidPoint + new Vector3Int(1,1,0), slimyWall);
                GridManager.Instance.AdvancedSetCell(1,leftMostMidPoint + new Vector3Int(2,0,0), slimyWall);
                GridManager.Instance.AdvancedSetCell(1,leftMostMidPoint + new Vector3Int(3,1,0), slimyWall);
                GridManager.Instance.AdvancedSetCell(1,leftMostMidPoint + new Vector3Int(4,2,0), slimyWall);
                GridManager.Instance.AdvancedSetCell(1,leftMostMidPoint + new Vector3Int(4,1,0), slimyWall);
                GridManager.Instance.AdvancedSetCell(1,leftMostMidPoint + new Vector3Int(4,0,0), slimyWall);
                GridManager.Instance.AdvancedSetCell(1,leftMostMidPoint + new Vector3Int(4,-1,0), slimyWall);
                GridManager.Instance.AdvancedSetCell(1,leftMostMidPoint + new Vector3Int(4,-2,0), slimyWall);
                mode += 1;
                break;
            case 5: //E
                GridManager.Instance.AdvancedSetCell(1,leftMostMidPoint + new Vector3Int(0,2,0), slimyWall);
                GridManager.Instance.AdvancedSetCell(1,leftMostMidPoint + new Vector3Int(0,1,0), slimyWall);
                GridManager.Instance.AdvancedSetCell(1,leftMostMidPoint + new Vector3Int(0,0,0), slimyWall);
                GridManager.Instance.AdvancedSetCell(1,leftMostMidPoint + new Vector3Int(0,-1,0), slimyWall);
                GridManager.Instance.AdvancedSetCell(1,leftMostMidPoint + new Vector3Int(0,-2,0), slimyWall);
                GridManager.Instance.AdvancedSetCell(1,leftMostMidPoint + new Vector3Int(1,2,0), slimyWall);
                GridManager.Instance.AdvancedSetCell(1,leftMostMidPoint + new Vector3Int(2,2,0), slimyWall);
                GridManager.Instance.AdvancedSetCell(1,leftMostMidPoint + new Vector3Int(1,0,0), slimyWall);
                GridManager.Instance.AdvancedSetCell(1,leftMostMidPoint + new Vector3Int(1,-2,0), slimyWall);
                GridManager.Instance.AdvancedSetCell(1,leftMostMidPoint + new Vector3Int(2,-2,0), slimyWall);
                mode += 1;
                break;

            // case 6: // random spawn slimyWall, ratio 1:4
            //     for (int i = layer1.cellBounds.min.x+1; i < layer1.cellBounds.max.x-1; i++)
            //     {
            //         for (int j = layer1.cellBounds.min.y+1; j < layer1.cellBounds.max.y-1; j++)
            //         {
            //             int x = Random.Range(1,5);
            //             if(x == 1){
            //                 GridManager.Instance.SetCell(1,new Vector3Int(i,j,0),slimyWall);
            //             }
            //             else{
            //                 GridManager.Instance.SetCell(1,new Vector3Int(i,j,0),floor);
            //             }
            //         }
            //     }
            //     break;
        
        }
    }
#endregion

#region spawn slimes
    void SpawnSlimesRandomly(int ratio){
        for (int i = layer1.cellBounds.min.x+1; i < layer1.cellBounds.max.x-3; i++)
            {
                for (int j = layer1.cellBounds.min.y+1; j < layer1.cellBounds.max.y-1; j++)
                {
                    int x = Random.Range(1,ratio+1);
                    if(x == 1 && new Vector3Int(i,j,0)!=GridManager.Instance.playerPosition){
                        Slime s = new Slime(new Vector3Int(i,j,0));
                        EnemyManager.Instance.enemies.Add(s);
                    }
                }
            }
    }
#endregion

#region others
    Vector3Int Vector3ToVector3Int(Vector3 v){ //boss
        return new Vector3Int((int)v.x, (int)v.y, (int)v.z);
    }
#endregion
}
