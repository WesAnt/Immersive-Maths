using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameControllerHex : MonoBehaviour
{

    // Maths variables
    private int decValue; // First operand
    private string hexQuestion; // The question in hex;
    public TextMeshPro questionText;

    // Timer variables
    public TextMeshPro timer;
    public float timeLeft = 60f; // Initial value of timer
    private bool gameOver = false;
    private AudioSource audio;
    public AudioClip timeOutSound;
    public AudioClip popupSound;

    // Moles variables
    public GameObject moleContainer;
    private MoleHex[] moles;
    private float showMoleTimer = 1.5f;
    private int lastNumber = 0;
    private int randomNumber = 0;
    private int correctMole = 0;
    private float correctMoleTimer = 8f;

    public TextMeshPro scoreText;
    public float score = 0f;

    // Use this for initialization
    void Start()
    {

        // Get all moles from moleContainer
        moles = moleContainer.GetComponentsInChildren<MoleHex>();


        // Set the initial time
        timer.text = "Time: " + timeLeft;
        scoreText.text = "Score: " + score;

        // Set the first question
        SetQuestion();

        // Set the correct answer and wrong answers
        SetAnswers();

    }

    void Awake()
    {
        // Get the AudioSource component
        audio = GetComponent<AudioSource>();
    }


    // Update is called once per frame
    void Update()
    {

        timeLeft -= Time.deltaTime; // subtract 1 second from timer
        correctMoleTimer -= Time.deltaTime; // subtract 1 from the show correct mole timer

        scoreText.text = "Score: " + score; // update score
        // check if timer has reached zero, if not update the timer.text with the timerLeft
        if (timeLeft > 1)
        {

            timer.text = "Time: " + Mathf.Floor(timeLeft); // update the timer

            // Show mole if showMoleTimer is 0
            showMoleTimer -= Time.deltaTime;
            if (showMoleTimer < 0f)
            {
                // Generate a random number
                if (correctMoleTimer < 0f)
                {
                    randomNumber = correctMole;
                    correctMoleTimer = 8f;
                }
                else
                {


                    while (randomNumber == lastNumber)
                    {
                        if (correctMole >= 6 && correctMole < 9)
                        {
                            randomNumber = Random.Range(6, 8);
                        }



                        else if (correctMole > 0 && correctMole < 6)
                        {
                            randomNumber = Random.Range(0, 6);

                        }
                        else if (correctMole == 0)
                        {
                            randomNumber = Random.Range(0, 3);
                        }


                    }
                }

                // Show the next mole randomly and assign randomNumber to lastNumber
                moles[randomNumber].ShowMole();
                lastNumber = randomNumber;

                // play sound
                audio.clip = popupSound;
                audio.Play();

                // reset mole timer 
                showMoleTimer = 1.5f;
            }
        }
        else
        // Timer has reached zero so game over
        {
            if (!gameOver)
            {
                audio.clip = timeOutSound;
                audio.Play();
                timer.text = "GAME OVER!";
                gameOver = true;
            }

        }


    }

    // Set the next question
    public void SetQuestion()
    {
        decValue = Random.Range(9, 32);
        hexQuestion = decValue.ToString("X");
        questionText.text = hexQuestion;


    }

    // Set the next answers
    public void SetAnswers()
    {
        // Set all the correctAnswerMole booleans to false
        for (int i = 0; i < 9; i++)
        {
            moles[i].correctAnswerMole = false;
        }

        int correctAnsMole = Random.Range(0, 9); // Randomise the correct answer mole
        correctMole = correctAnsMole;

        moles[correctAnsMole].correctAnswerMole = true; // Set this moles correct answer bool to true

        for (int i = 0; i < 9; i++)
        {
            if ((i != correctAnsMole))
            {
                int ans = decValue;
                while (ans == decValue)
                {
                    ans = Random.Range(9, 32);
                }
                moles[i].answerText.text = ans + "";
            }
            else
            {
                moles[correctAnsMole].answerText.text = decValue + "";
            }
        }
    }
}
