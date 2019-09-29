using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

// The following code has been adapted from Jamie Gant's video tutorial from Ganttech.com
// --------------------------------------------------------------------------------------

public class Mole : MonoBehaviour {

    private AudioSource audio;
    public AudioClip yeahSound;
    public AudioClip owSound;
    

    public float visibleHeight = 2f; // transform.y value when mole is hidden
    public float hiddenHeight = 0.15f; // transform.y value when mole is visible
    private Vector3 newPosition; // XYZ position of current mole
    private float speed = 15f; // speed at which the mole appears and hides
    public float hideMoleTimer = 1.5f; // Timer to hide the mole
    public TextMeshPro answerText; // The text component to display the answer
    private bool moleHidden = false; // Is the mole hidden
    [System.NonSerialized] public bool correctAnswerMole; // Is this the correctAnswerMole?
    
   

	// Use this for initialization
	void Start () {
        
	}

    void Awake()
    {
        HideMole(); // Hide the moles on start up
        audio = GetComponent<AudioSource>();
        transform.localPosition = newPosition; // Set the initial position
    }
    // Update is called once per frame
    void Update () {
        // move mole to the new position
        transform.localPosition = Vector3.Lerp(
            transform.localPosition,
            newPosition,
            Time.deltaTime * speed
        );

        // hide mole is mole < 0
        hideMoleTimer -= Time.deltaTime;
        if (hideMoleTimer < 0)
        {
            HideMole();
            hideMoleTimer = 1.5f; // reset hideMoleTimer
        }
	}

    // Hide the mole
    public void HideMole()
    {
        moleHidden = true;
        // Set the current position to hiddenHeight
        newPosition = new Vector3(
            transform.localPosition.x,
            hiddenHeight,
            transform.localPosition.z);  
    }

    // Show the mole
    public void ShowMole()
    {
        // Set the current position to visibleHeight
        moleHidden = false;
        newPosition = new Vector3(
            transform.localPosition.x,
            visibleHeight,
            transform.localPosition.z);
    }

    // When mole is hit, hide mole instantly and increase score
    public void OnTriggerEnter(Collider col)
    {
        
        if ((correctAnswerMole) && !moleHidden) {
            GameObject.Find("GameController").GetComponent<GameController>().score += 10;
            GameObject.Find("GameController").GetComponent<GameController>().SetQuestion();
            GameObject.Find("GameController").GetComponent<GameController>().SetAnswers();
            GameObject.Find("GameController").GetComponent<GameController>().ResetTimer();
            GameObject.Find("GameController").GetComponent<GameController>().amountCorrect++;
            audio.clip = yeahSound;
            audio.Play();
        }
        else if (!moleHidden)
        {
            GameObject.Find("GameController").GetComponent<GameController>().score -= 10;
            audio.clip = owSound;
            audio.Play();
        }
     
        HideMole();
    }
}
