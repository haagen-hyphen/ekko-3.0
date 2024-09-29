using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class MenuScript : MonoBehaviour
{
    public void LoadLevel(int level){
        Time.timeScale = 1f;
        LevelManager.Instance.LoadLevel(level);
    }

    public void QuitGame(){
        Application.Quit();
    }
}
