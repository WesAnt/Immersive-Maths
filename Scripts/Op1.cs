using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Op1 : MonoBehaviour
{

    public AudioSource audio; // The main audio object
    public AudioClip yeahSound; // The sound for the correct answer
    public AudioClip errorSound; // The sound for the wrong answer


    [System.NonSerialized] public bool correctAnswer = false;

    // Use this for initialization
    void Start()
    {
        audio = GetComponent<AudioSource>();
    }


    private void OnTriggerEnter(Collider col)
    {
        
        if (GameObject.Find("GameController").GetComponent<InteractiveGameController>().gameOver == false)
        {
            if (!correctAnswer && col.GetComponent<Cube>().correctNumber && 
                col.GetComponent<Cube>().op1Allowed)
            {
                col.GetComponent<Cube>().hasBeenPlaced1 = true;
                audio.clip = yeahSound;
                audio.Play();
                correctAnswer = true;
                GetComponent<Renderer>().material.color = new Color(0, 0.9f, 0);
            }
            else if (!correctAnswer && (col.GetComponent<Cube>().correctNumber == false ||
                    col.GetComponent<Cube>().op1Allowed == false))
            {
                GetComponent<Renderer>().material.color = new Color(1.0f, 0, 0);
                   
                audio.clip = errorSound;
                audio.Play();
                GameObject.Find("GameController").GetComponent<InteractiveGameController>().numberOfTries += 1.0f;
            }
        }  
    }
}