using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InteractiveGameControllerAlgebra : MonoBehaviour {


    // Variables for accessing game objects within the scene
    public GameObject cubeContainer; // Used to access the cubeContainer game object
    private Cube[] formula; // Holds an array of cubes which will contain formulas
    public GameObject questionContainer; // Holds an instance of the questionContainer game object


    // Variables for controlling game state
    public TextMeshPro levelText; // A pointer to the level text component
    private float level = 1;       // The current level
    private bool gameOver = true;     // Is the game over?
    private bool preStartGame = true; // Should the prestart trigger?
    private bool nextQuestion = false; // Is this the next question?
    private Vector3[] originalPosition; // A Vector array which Stores the old positions of each answer tile
    private bool winner = false; // Has the player got the answer correct?



    // Variables for audio
    private AudioSource audio; // The audio component
    public AudioClip timeOutSound; // An instance of the end of time sound
    public AudioClip correctMusic; // An instance of the correct answer sound effect
    public AudioClip clockTick; // An instance of the 10 seconds left clock tick
    public AudioClip levelUpSound; // An instance of the next level sound
    public AudioClip getReadySound; // An instance of the get ready voice clip
    public AudioClip goSound; // An instance of the go voice clip


    // Variables for the answer slots
    private LeftSide1 op1; // First formula slot
    private RightSide1 op2; // Second formula slot

    // Maths variables
    public TextMeshPro xText; // The text component for the 'x = '
    public TextMeshPro yText; // The text component for the 'y = '
    public TextMeshPro yHelperText; // The text component for the y side helper text
    private int sum = 0; // Variable to hold the sum
    private int x = 0; // Variable to store x
    private int y = 0; // Variable to store y

    private int multiplierX; // Variable to store the random number to multiply 'x' by
    private int multiplierY; // Variable to store the random number to multiply 'y' by
    private int adderX; // Variable to store the random number to add to 'x' side of equation
    private int adderY; // Variable to store the random number to add to 'y' side of equation
    private int subractorX; // Variable to store the random number to subtract to 'x' side 
    private int subractorY; // Variable to store the random number to subtract to 'y' side
    private int correctTileX = 0; // Keeps track of which tile contains the correct answer
    private int correctTileY = 0; // Keeps track of which tile contains the correct answer

    // Timer variables for the answer slots
    private float timeLeft = 60f;
    private float previousTime = 60f;
    public TextMeshPro timerText;  // Right countdown timer
    public TextMeshPro timerText2; // Left countdown timer
    private float timer1 = 1f; 
    private float timer2 = 1f;
    private float restartTimer = 2.0f;
    private float preStartTimer = 5.0f;



    // Use this for initialization
    void Start () {

        // Get the AudioSource component
        audio = GetComponent<AudioSource>();
        audio.clip = clockTick;

        formula = cubeContainer.GetComponentsInChildren<Cube>();
        op1 = questionContainer.GetComponentInChildren<LeftSide1>();
        op2 = questionContainer.GetComponentInChildren<RightSide1>();
       
        // Store original positions of answer boards
        originalPosition = new Vector3[formula.Length];
        for (int i = 0; i < formula.Length; i++)
        {
            originalPosition[i] = new Vector3(formula[i].transform.position.x, formula[i].transform.position.y,
                                                formula[i].transform.position.z);
        }
        SetQuestionLevel1();
    }

    // Update is called once per frame
    void Update ()
    {
        if (preStartGame)
        {
            PreStartGame();
        }
        else if (!gameOver)
        {
            UpdateTimer(); // Update the count down timer
            CheckAnswers(); // Check each answer
        }    
    }

    // Method to check the answers that have been placed
    private void CheckAnswers()
    {
        if (!gameOver)
        {
            if (!nextQuestion)
            {
                if (op1.occupied)
                {
                    for (int i = 0; i < formula.Length; i++)
                    {
                        if (formula[i].hasBeenPlaced1)
                        {
                            formula[i].transform.position = new Vector3(op1.transform.position.x,
                                  op1.transform.position.y, op1.transform.position.z - 0.1f);
                            formula[i].transform.rotation = Quaternion.identity;

                            if (timer1 < 0)
                            {
                                // If the correct answer hasnt been placed yet, allow other answers to be placed
                                if (op1.correctAnswer == false)
                                {
                                    formula[i].hasBeenPlaced1 = false;
                                    timer1 = 1f;
                                    op1.occupied = false;
                                }

                            }
                            timer1 -= Time.deltaTime;
                        }
                    }
                }

                if (op2.occupied)
                {
                    for (int i = 0; i < formula.Length; i++)
                    {
                        if (formula[i].hasBeenPlaced2)
                        {
                            formula[i].transform.position = new Vector3(op2.transform.position.x,
                                  op2.transform.position.y, op2.transform.position.z - 0.1f);
                            formula[i].transform.rotation = Quaternion.identity;

                            if (timer2 < 0)
                            {
                                // If the correct answer hasnt been placed yet, allow other answers to be placed
                                if (op2.correctAnswer == false)
                                {
                                    formula[i].hasBeenPlaced2 = false;
                                    timer2 = 1f;
                                    op2.occupied = false;
                                }

                            }
                            timer2 -= Time.deltaTime;
                        }
                    }
                }

                if (op1.correctAnswer && op2.correctAnswer && !winner)
                {
                    timeLeft = 60f;
                    level += 0.5f;
                      
                    restartTimer = 2.0f;
                    winner = true;
                }

                if (winner)
                {
                    if (restartTimer == 2.0f)
                    {
                        audio.clip = correctMusic;
                        audio.Play();
                    }
                    
                    restartTimer -= Time.deltaTime;
                    
                    if (restartTimer < 0)
                    {
                        ResetGame();
                        
                        switch ((int)level)
                        {
                            case 1:
                                SetQuestionLevel1();
                                break;
                            case 2:
                                if (level == 2.0f)
                                {
                                    audio.clip = levelUpSound;
                                    audio.Play();
                                }
                                
                                SetQuestionLevel2();
                                break;
                            case 3:
                                if (level == 3.0f)
                                {
                                    audio.clip = levelUpSound;
                                    audio.Play();
                                }
                                SetQuestionLevel3();
                                break;
                            case 4:
                                if (level == 4.0f)
                                {
                                    audio.clip = levelUpSound;
                                    audio.Play();
                                }
                                SetQuestionLevel4();
                                break;
                        }
                        winner = false;
                    }
                }
            }
        }
    }
    // Set the next question and assign the answers to the cubes
    private void SetQuestionLevel1()
    {
        levelText.text = "Level: " + (int)level;
        // Randomise the next sum, x and y variables
        bool notDivideable = true;

        while (notDivideable)
        {
            sum = Random.Range(20, 40);
            x = Random.Range(2, 6);
            if (sum % x == 0)
            {
                notDivideable = false;
            }
        }
        xText.text = "x = " + x;
        yText.text = "";

        // Calculate the correct equations
        multiplierX = (sum / x);
       

        // Randomise the tiles which contain the correct equations
        correctTileX = Random.Range(0, formula.Length);
        correctTileY = correctTileX;
        while (correctTileX == correctTileY)
        {
            correctTileY = Random.Range(0, formula.Length);
        }

        formula[correctTileY].num = multiplierX;
        formula[correctTileX].numText.text = multiplierX + "x";
        formula[correctTileX].shouldBePlaced = 1;

        formula[correctTileX].num = sum;
        formula[correctTileY].numText.text = "" + sum;
        formula[correctTileY].shouldBePlaced = 2;
    
        // Place other random answers on remaining tiles
        int assignX = 1;
        int randomNumberSum = sum;
        int randomNumberForm;
        bool distinct = false;


        for (int i = 0; i < formula.Length; i++)
        { 
            if (i != correctTileX && i != correctTileY)
            {
                if (assignX == 1)
                {
                    randomNumberSum = sum;

                    // Make sure each sum on boards are distinct
                    
                    while (!distinct)
                    {
                        distinct = true;
                        randomNumberSum = Random.Range(1, 60);
                        
                        for (int j = 0; j < formula.Length; j++)
                        {
                            if (randomNumberSum == formula[j].num)
                            {
                                distinct = false;
                            }
                        }                   
                    }

                    formula[i].num = randomNumberSum;
                    formula[i].numText.text = "" + randomNumberSum;
                    distinct = false;
                }

                else if (assignX == -1)
                {
                    randomNumberForm = multiplierX;
                    while (!distinct)
                    {
                        distinct = true;
                        randomNumberForm = Random.Range(2, 20);

                        for (int j = 0; j < formula.Length; j++)
                        {
                            if (randomNumberForm == formula[j].num || randomNumberForm * x == formula[j].num)
                            {
                                distinct = false;
                            }
                        }
                    }
                    formula[i].num = randomNumberForm;
                    formula[i].numText.text = randomNumberForm + "x";
                    distinct = false;
                }
                assignX = -assignX;
            }
        }
    }

    // Set the next question and assign the answers to the cubes
    private void SetQuestionLevel2()
    {
        levelText.text = "Level: " + (int)level;
        
        // Randomise the next sum, x and y variables
        sum = Random.Range(20, 40);
        x = Random.Range(2, 5);
        
        xText.text = "x = " + x;
        yText.text = "";

        // Calculate the correct equations
        multiplierX = (sum / x) - 1;
        adderX = (sum % x) + x;

        Debug.Log("Sum: " + sum + " RemainderX: " + sum % x);

        // Randomise the tiles which contain the correct equations
        correctTileX = Random.Range(0, formula.Length);
        correctTileY = correctTileX;
        while (correctTileX == correctTileY)
        {
            correctTileY = Random.Range(0, formula.Length);
        }

        formula[correctTileX].numText.text = multiplierX + "x + " + adderX;
        formula[correctTileX].shouldBePlaced = 1;

        formula[correctTileY].num = sum;
        formula[correctTileY].numText.text = "" + sum;
        formula[correctTileY].shouldBePlaced = 2;
    
        // Place other random answers on remaining tiles
        int assignX = 1;
        int randomNumberSum = sum;
        int randomNumberForm;
        int randomNumberAdder = adderX;
        bool distinct = false;

        for (int i = 0; i < formula.Length; i++)
        {
            if (i != correctTileX && i != correctTileY)
            {
                if (assignX == 1)
                {
                    randomNumberSum = sum;

                    // Make sure each sum on boards are distinct

                    while (!distinct)
                    {
                        distinct = true;
                        randomNumberSum = Random.Range(1, 60);

                        for (int j = 0; j < formula.Length; j++)
                        {
                            if (randomNumberSum == formula[j].num)
                            {
                                distinct = false;
                            }
                        }
                    }

                    formula[i].num = randomNumberSum;
                    formula[i].numText.text = "" + randomNumberSum;
                    distinct = false;
                }

                else if (assignX == -1)
                {
                    randomNumberForm = multiplierX;
                    while (!distinct)
                    {
                        distinct = true;
                        randomNumberForm = Random.Range(2, 20);
                        randomNumberAdder = Random.Range(1, 15);

                        for (int j = 0; j < formula.Length; j++)
                        {
                            if ((randomNumberForm * x) + randomNumberAdder == formula[j].num)
                            {
                                distinct = false;
                            }
                        }
                    }
                    formula[i].num = randomNumberForm;
                    formula[i].numText.text = randomNumberForm + "x" + " + " + randomNumberAdder;
                    distinct = false;
                }
                assignX = -assignX;
            }
        }
    }

 
    // Set the next question and assign the answers to the cubes
    private void SetQuestionLevel3()
    {
        levelText.text = "Level: " + (int)level;
        yHelperText.text = "Place 'Y' here";

        // Randomise the next sum, x and y variables
        bool notDivideable = true;
        while (notDivideable)
        {
            sum = Random.Range(20, 40);
            y = Random.Range(2, 5);
            x = Random.Range(2, 5);
            if (sum % y == 0 && sum % x != 0)
            {
                notDivideable = false;
            }
        }
        
        xText.text = "x = " + x;
        yText.text = "y = " + y;

        // Calculate the correct equations
        
        multiplierX = (sum / x) - 1;
        adderX = (sum % x) + x;
        multiplierY = (sum / y);
        
        
        Debug.Log("Sum: " + sum + " RemainderX: " + sum % x);

        // Randomise the tiles which contain the correct equations
        correctTileX = Random.Range(0, formula.Length);
        correctTileY = correctTileX;
        while (correctTileX == correctTileY)
        {
            correctTileY = Random.Range(0, formula.Length);
        }

        formula[correctTileX].num = sum;
        formula[correctTileX].numText.text = multiplierX + "x + " + adderX;
        formula[correctTileX].shouldBePlaced = 1;

        formula[correctTileY].num = sum;
        formula[correctTileY].numText.text = multiplierY + "y";
        formula[correctTileY].shouldBePlaced = 2;
        // formula[correctTileY].hasBeenPlaced2 = true;
        //  op2.occupied = true;

        // Place other random answers on remaining tiles
        int assignX = 1;
        int randomNumberMultiplierX;
        int randomNumberMultiplierY;
        int randomNumberAdder = adderX;
        int randomNumberSum;
        bool distinct = false;

        for (int i = 0; i < formula.Length; i++)
        {
            if (i != correctTileX && i != correctTileY)
            {
                if (assignX == 1)
                {
                    randomNumberMultiplierX = multiplierX;
                    randomNumberSum = sum;

                    // Make sure each sum on boards are distinct

                    while (!distinct)
                    {
                        distinct = true;
                        randomNumberMultiplierX = Random.Range(1, 8);
                        randomNumberAdder = Random.Range(1, 15);

                        for (int j = 0; j < formula.Length; j++)
                        {
                            if (randomNumberMultiplierX + randomNumberAdder
                                == formula[j].num)
                            {
                                distinct = false;
                            }
                        }
                    }

                    formula[i].num = randomNumberSum;
                    formula[i].numText.text = randomNumberMultiplierX + "x" + " + " + randomNumberAdder;
                    distinct = false;
                }

                else if (assignX == -1)
                {
                    randomNumberMultiplierY = multiplierY;
                    while (!distinct)
                    {
                        distinct = true;
                        randomNumberMultiplierY = Random.Range(2, 20);
                        randomNumberAdder = Random.Range(1, 15);

                        for (int j = 0; j < formula.Length; j++)
                        {
                            if ((randomNumberMultiplierY * y) == formula[j].num)
                            {
                                distinct = false;
                            }
                        }
                    }
                    formula[i].num = sum;
                    formula[i].numText.text = randomNumberMultiplierY + "y";
                    distinct = false;
                }
                assignX = -assignX;
            }
        }    
    }

    // Set the next question and assign the answers to the cubes
    private void SetQuestionLevel4()
    {
        levelText.text = "Level: " + (int)level;
        // Randomise the next sum, x and y variables
        sum = Random.Range(20, 40);
        x = Random.Range(2, 5);
        y = x;
        while (x == y)
        {
            y = Random.Range(2, 5);
        }

        xText.text = "x = " + x;
        yText.text = "y = " + y;

        // Calculate the correct equations
        multiplierX = (sum / x) - 1;
        adderX = (sum % x) + x;
        multiplierY = (sum / y) - 1;
        adderY = (sum % y) + y;

        Debug.Log("Sum: " + sum + " RemainderX: " + sum % x);

        // Randomise the tiles which contain the correct equations
        correctTileX = Random.Range(0, formula.Length);
        correctTileY = correctTileX;
        while (correctTileX == correctTileY)
        {
            correctTileY = Random.Range(0, formula.Length);
        }

        formula[correctTileX].numText.text = multiplierX + "x + " + adderX;
        formula[correctTileX].shouldBePlaced = 1;

        formula[correctTileY].numText.text = multiplierY + "y + " + adderY;
        formula[correctTileY].shouldBePlaced = 2;
        // formula[correctTileY].hasBeenPlaced2 = true;
        //  op2.occupied = true;

        // Place other random answers on remaining tiles
        int assignX = 1;
        int randomNumberMultiplier;
        int randomNumberAdder;

        for (int i = 0; i < formula.Length; i++)
        {
            if (i != correctTileX && i != correctTileY)
            {
                
                if (assignX == 1)
                {
                    randomNumberMultiplier = multiplierX;
                    randomNumberAdder = adderX;
                    while ((randomNumberMultiplier * x) + adderX == sum)
                    {
                        randomNumberMultiplier = Random.Range(1, 20);
                        randomNumberAdder = Random.Range(1, 9);
                    }

                    formula[i].numText.text = randomNumberMultiplier + "x + " + adderX;
                }

                else if (assignX == -1)
                {
                    randomNumberMultiplier = multiplierY;
                    randomNumberAdder = adderY;
                    while ((randomNumberMultiplier * y) + adderY == sum)
                    {
                        randomNumberMultiplier = Random.Range(1, 20);
                        randomNumberAdder = Random.Range(1, 9);
                    }
                    formula[i].numText.text = randomNumberMultiplier + "y + " + adderY;
                }
                assignX = -assignX;
            }
        }
    }

    // Update the countdown timer
    private void UpdateTimer()
    {
        if (timeLeft > 0)
        {
            
            timerText.text = "Time:" + Mathf.Floor(timeLeft);
            timerText2.text = "Time:" + Mathf.Floor(timeLeft);
            
            timeLeft -= Time.deltaTime;
            // If there's less than 10 seconds left play ticking sound
            if (timeLeft < 10)
            {
                // If one second has passed trigger sound
                if (previousTime - timeLeft >= 1.0f)
                {
                    audio.Stop();
                    audio.clip = clockTick;
                    audio.Play();
                    previousTime = timeLeft;
                }
            }         
        }

        else if (timeLeft <= 0 && !gameOver)
        {
            timerText.text = "Game Over!";
            timerText2.text = "Game Over!";         
            audio.clip = timeOutSound;
            audio.Play();
            gameOver = true;
        }

    }

    // Reset the game variables
    private void ResetGame()
    {
        for (int i = 0; i < formula.Length; i++)
        {
            formula[i].shouldBePlaced = 0;
            formula[i].hasBeenPlaced1 = false;
            formula[i].hasBeenPlaced2 = false;
            op1.occupied = false;
            op2.occupied = false;
            op1.correctAnswer = false;
            op2.correctAnswer = false;
            formula[i].transform.position = originalPosition[i];
            formula[i].transform.rotation = Quaternion.identity;
            nextQuestion = false;
            op1.GetComponent<Renderer>().material.color = new Color(0.6f, 0.6f, 0.6f);
            op2.GetComponent<Renderer>().material.color = new Color(0.6f, 0.6f, 0.6f);
            audio.Stop();
        }
    }

    // Pre start the game
    private void PreStartGame()
    {
        audio.clip = getReadySound;
        if (preStartTimer == 5.0f)
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
        }
        preStartTimer -= Time.deltaTime;
    }
}
