using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameController : MonoBehaviour {

    // Maths variables
    private int op1; // First operand
    private int op2; // Second operand
    private int sum; // The sum
    public TextMeshPro questionText;
    public int amountCorrect = 0; // The number of questions that have been answered correctly

    // Timer variables
    public TextMeshPro timer;
    private float timeLeft = 15f; // Initial value of timer
    private float preStartTimer = 4.0f;
    private bool preStartGame = true; // Should the prestart trigger?
    private bool gameOver = false;

    // Audio clips
    private AudioSource audio;
    public AudioClip timeOutSound;
    public AudioClip popupSound;
    public AudioClip getReadySound;
    public AudioClip goSound;


    // Moles variables
    public GameObject moleContainer;
    private Mole[] moles;
    private float showMoleTimer = 1.5f;
    private int lastNumber = 0;
    private int randomNumber = 0;
    private int correctMole = 0;
    private float correctMoleTimer = 7f;

    public TextMeshPro scoreText;
    public float score = 0f;

    // Use this for initialization
    void Start () {
        
        // Get all moles from moleContainer
        moles = moleContainer.GetComponentsInChildren<Mole>();

        // Set the initial time
        scoreText.text = "Score: " + score;

        // Set the first question
        SetQuestion();

        // Set the correct answer and wrong answers
        SetAnswers();
        GameObject.Find("Camera").GetComponent<MusicHandler>().music.Stop();
    }

    void Awake()
    {
        // Get the AudioSource component
        audio = GetComponent<AudioSource>();
        
        amountCorrect = 0;
        showMoleTimer = 1.5f;
        lastNumber = 0;
        randomNumber = 0;
        correctMole = 0;
        correctMoleTimer = 7f;
}


    // Update is called once per frame
    void Update () {

        if (preStartGame)
        {
            PreStartGame();
        }
        else
        {

            timeLeft -= Time.deltaTime; // subtract 1 second from timer
            correctMoleTimer -= Time.deltaTime; // subtract 1 from the show correct mole timer

            scoreText.text = "Score: " + score; // update score
                                                // check if timer has reached zero, if not update the timer.text with the timerLeft
            if (timeLeft > 0)
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
                        correctMoleTimer = 7f;
                    }
                    else
                    {
                        while (randomNumber == lastNumber)
                        {
                            if (correctMole >= 6 && correctMole < 9)
                            {
                                randomNumber = Random.Range(6, 9);
                            }
                            else if (correctMole >= 0 && correctMole < 6)
                            {
                                randomNumber = Random.Range(0, 5);
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
                    GameObject.Find("Camera").GetComponent<MusicHandler>().music.Stop();
                    audio.Play();
                    timer.text = "GAME OVER!";
                    gameOver = true;
                }
            }
        }
	}

    // Pre start routine
    public void PreStartGame()
    {
        audio.clip = getReadySound;
        if (preStartTimer == 4.0f)
        {
            audio.Play();
        }

        if (preStartTimer < 0f)
        {
            audio.Stop();
            gameOver = false;
            preStartGame = false;
            audio.clip = goSound;
            audio.Play();
            GameObject.Find("Camera").GetComponent<MusicHandler>().music.Play();
        }
        preStartTimer -= Time.deltaTime;
    }


    // Set the next question
    public void SetQuestion()
    {
        op1 = Random.Range(1, 20);
        op2 = Random.Range(1, 20);
        sum = op1 + op2; 
        questionText.text = op1 + " + " + op2;
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
                int ans = sum;
                while (ans == sum)
                {
                    ans = Random.Range(1, 40);
                }
                moles[i].answerText.text = ans + "";
            }
            else
            {
                moles[correctAnsMole].answerText.text = sum + ""; 
            }
        }
    }

    // Reset the timer
    public void ResetTimer()
    {
        if (amountCorrect < 4)
        {
            timeLeft = 15f;
        }
        else if (amountCorrect >= 4 && amountCorrect < 8)
        {
            timeLeft = 10f;
        }
        
        else if (amountCorrect >= 8 && amountCorrect < 14)
        {
            timeLeft = 8f;

        }

        else if (amountCorrect >= 14 && amountCorrect < 20)
        {
            timeLeft = 5f;
        }

        else if (amountCorrect >= 20)
        {
            timeLeft = 3f;
        }
    
        
    }

    // Reset the important variables
    public void ResetAll()
    {
        timeLeft = 10f;
        showMoleTimer = 1.5f;
        correctMoleTimer = 8f;
        lastNumber = 0;
        randomNumber = 0;
        correctMole = 0;
        score = 0;
        gameOver = false;
        

    }
}   
