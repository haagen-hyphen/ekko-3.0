using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Unity.VisualStudio.Editor;
using Unity.Collections;
using Unity.VisualScripting;
using UnityEditor.U2D.Aseprite;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public TickManager tickManager;
    // public RectTransform axisTransform;
    float t = 0f;
    public RectTransform abilitySlot;
    public Sprite hand;
    public Camera mainCamera;
    public bool isGamePaused; //to be put at every script's update
    // public AudioSource audioSource; 

    void Start()
    {
        SetAbilityImage(hand);
    }

    void Update()
    {
        // RunTimeline();
        if(Input.GetKeyDown(KeyCode.Escape) && !isGamePaused){
            PauseGame();
        }
        else if(Input.GetKeyDown(KeyCode.Escape) && isGamePaused){
            ResumeGame();
        }
    }

    void PauseGame()
    {
        Time.timeScale = 0;
        isGamePaused = true;
        // audioSource.enabled = false;
        gameObject.transform.GetChild(1).gameObject.SetActive(true);
    }
    public void ResumeGame()
    {
        gameObject.transform.GetChild(1).gameObject.SetActive(false);
        // audioSource.enabled = true;
        isGamePaused = false;
        Time.timeScale = 1;
    }

    public void MainMenu(){
        SceneManager.LoadScene(0);
    }

    public void Restart(){
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        Time.timeScale = 1f;
    }

    public void SetAbilityImage(Cell killedBy){
        abilitySlot.GetComponent<UnityEngine.UI.Image>().sprite = killedBy.abilityImage;
    }
    public void SetAbilityImage(Sprite image){
        abilitySlot.GetComponent<UnityEngine.UI.Image>().sprite = image;
    }

    // public void RunTimeline(){
    //     t += 1f * Time.deltaTime;
    //     if(t>=1){
    //         t -= 1f;
    //     }
    //     float x = Mathf.Lerp(0,-50,t);
    //     axisTransform.anchoredPosition = new Vector3(x,0,0);
    // }

    public IEnumerator moveCamera(Vector3 towardsDirection, float inHowmanySeconds){
        int animationFluency = 100;
        for(int i=0; i<animationFluency; i++){
            mainCamera.transform.Translate(towardsDirection/animationFluency);
            yield return new WaitForSeconds(inHowmanySeconds/animationFluency);
        }
    }
}
