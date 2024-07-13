using System.Collections;
using System.Collections.Generic;
using System.Security;
using Unity.Burst;
using Unity.Mathematics;
using UnityEngine;

public class zdtest : MonoBehaviour {


    [Range(0, 1)]
    public float a;
    public Material material;


    // Update is called once per frame
    void Update() {
        a = 2 * (Time.time % 0.5f);
        material.SetFloat("b", a);
    }
}
