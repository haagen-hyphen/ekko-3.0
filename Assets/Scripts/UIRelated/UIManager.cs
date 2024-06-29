using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Unity.VisualStudio.Editor;
using Unity.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public TickManager tickManager;
    #region Timeline-Related Variables
        public RectTransform axisTransform;
        float t = 0f;
    #endregion
    #region Ability-Related Variables
        public RectTransform abilitySlot;
        public Sprite[] abilityImages;
    #endregion
    
    // Start is called before the first frame update
    void Start()
    {
        SetAbilityImage(0);
    }

    // Update is called once per frame

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

    public void SetAbilityImage(int imageIndex){
        abilitySlot.GetComponent<UnityEngine.UI.Image>().sprite = abilityImages[imageIndex];
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
