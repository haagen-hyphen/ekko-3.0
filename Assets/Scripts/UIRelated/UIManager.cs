using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Unity.VisualStudio.Editor;
using Unity.Collections;
using Unity.VisualScripting;
using UnityEditor.U2D.Aseprite;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public TickManager tickManager;
    public RectTransform axisTransform;
    float t = 0f;
    public RectTransform abilitySlot;
    public Sprite hand;

    void Start()
    {
        SetAbilityImage(hand);
    }

    void Update()
    {
        RunTimeline();
        if(Input.GetKeyDown(KeyCode.Escape)){
            PauseGame();
        }
    }

    void PauseGame()
    {
        Time.timeScale = 0;
        gameObject.transform.GetChild(1).gameObject.SetActive(true);
    }
    public void ResumeGame()
    {
        gameObject.transform.GetChild(1).gameObject.SetActive(false);
        Time.timeScale = 1;
    }

    public void SetAbilityImage(Cell killedBy){
        abilitySlot.GetComponent<UnityEngine.UI.Image>().sprite = killedBy.abilityImage;
    }
    public void SetAbilityImage(Sprite image){
        abilitySlot.GetComponent<UnityEngine.UI.Image>().sprite = image;
    }

    public void RunTimeline(){
        t += 1f * Time.deltaTime;
        if(t>=1){
            t -= 1f;
        }
        float x = Mathf.Lerp(0,-50,t);
        axisTransform.anchoredPosition = new Vector3(x,0,0);
    }
}
