using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance {get; private set;}
    public Animator transition;
    public float transitionTime = 1f;

    public void Awake(){
        if (Instance != null && Instance != this) 
        { 
            Destroy(this); 
        } 
        else 
        { 
            Instance = this; 
        } 
    }

    public void LoadLevel(int index){
        StartCoroutine(LoadTransitionLevel(index));
    }

    IEnumerator LoadTransitionLevel(int index){
        transition.SetTrigger("Start");

        yield return new WaitForSecondsRealtime(transitionTime);
        SceneManager.LoadScene(index);
    }
}
