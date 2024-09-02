using System;
using System.Collections;
using System.Collections.Generic;
// using System.Numerics;
using Unity.Mathematics;
using UnityEngine;

public class MoveIndicator : MonoBehaviour
{
    GameObject player;
    public PlayerMovement playerMovement;
    public TickManager tickManager;
    public Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        transform.position = player.transform.position + playerMovement.moveBuffer;
        
        if (playerMovement.moveBuffer == Vector3.right) {
            transform.eulerAngles = new Vector3(0, 0, 0);
        }
        else if (playerMovement.moveBuffer == Vector3.up) {
            transform.eulerAngles = new Vector3(0, 0, 90);
        }
        else if (playerMovement.moveBuffer == Vector3.left) {
            transform.eulerAngles = new Vector3(0, 0, 180);
        }
        else if (playerMovement.moveBuffer == Vector3.down) {
            transform.eulerAngles = new Vector3(0, 0, 270);
        }

        animator.Play("move buffer indicator", 0, Mathf.InverseLerp(0, 0.5f, Time.time % 0.5f));
        animator.StopPlayback();
        if (playerMovement.moveBuffer == Vector3Int.zero) {
            GetComponent<SpriteRenderer>().enabled = false;
        } else {
            GetComponent<SpriteRenderer>().enabled = true;
        }
        
    }
}
