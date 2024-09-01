using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class MenuScript : MonoBehaviour
{
    public void LoadLevel(int level){
        SceneManager.LoadScene(level);
        Time.timeScale = 1f;
    }

    public void QuitGame(){
        Application.Quit();
    }
}
