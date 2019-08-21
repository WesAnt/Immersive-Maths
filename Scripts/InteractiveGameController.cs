using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class InteractiveGameController : MonoBehaviour
{
    // Booleans for conditions dependent on type of game
    public bool isMultiplicationGame;
    public bool isAdditionGame;
    public bool isSubtractionGame;
    public bool isDivisionGame;
    public bool is2OperatorGame;
    
    // Number graphics variables
    public GameObject cubeContainer;
    private Cube[] cubes;
    public GameObject questionContainer;
    private Vector3[] originalPosition; // A Vector array which Stores the old positions of each answer tile

    // Sound effects
    public AudioSource audio;
    public AudioClip correctMusic;
    public AudioClip clockTick;
    public AudioClip levelUpSound;
    public AudioClip timeOutSound;
    public AudioClip crowdSound;
    public AudioClip getReadySound;
    public AudioClip goSound;
    
    // Misc variables
    public bool winner = false; // Is current answer correct?
    public bool gameOver = false; // Is the game over?
    private bool preStartGame = true; // Should the prestart trigger?
    private int numbersPlaced = 0; // How many numbers have been placed on that particular round, 0,1 or 2
    private int firstNumberPlaced = 0; // The number that was placed first
    private int secondNumberPlaced = 0; // The number that was placed second
    int highestNumber = 9; // The highest number available to pick up
    int lowestNumber = 1; // The lowest number available to pick up


    // Score variables
    private float amountCorrect = 0; // The amount of anwers correct
    public TextMeshPro amountCorrectText; // The text component to print correct text
    public TextMeshPro numberOfTriesText; // The text component to print number of tries text
    private float accuracy = 0; // The accuracy calculated (amountCorrect / The number of tries)
    [System.NonSerialized] public float numberOfTries = 0; // The number of tries
    private float level = 1.0f; // The current level
    public TextMeshPro levelText; // The text component to print the level

    // Timer variables
    private float startingTime = 30f;  
    private float timeLeft;
    private float previousTime;
    public TextMeshPro timerText;
    public float restartTimer = 0f;
    private float preStartTimer = 4.0f;

    // Maths variables
    private Op1 op1; // First operand
    private Op2 op2; // Second operand
    private Op3 op3; // Third operand ("2 operator" game only)
    public TextMeshPro operatorText;
    public TextMeshPro operator2Text;
    public TextMeshPro answerText;
    public int sum;


    // Use this for initialization
    void Start()
    {
        cubes = cubeContainer.GetComponentsInChildren<Cube>();  // Get the graphical numbers to use
        op1 = questionContainer.GetComponentInChildren<Op1>(); // Get access to the script of the 1st operand
        op2 = questionContainer.GetComponentInChildren<Op2>(); // Get access to the script of the 2nd operand 
        op3 = questionContainer.GetComponentInChildren<Op3>(); // Get access to the script of the 3rd operand

        audio = GetComponent<AudioSource>(); // Get the audio component

        // Assign numbers to the variables and cubes
        for (int i = 0; i < 9; i++)
        {
            cubes[i].num = i + 1;
            cubes[i].numText.text = "" + (i + 1);         
            cubes[i + 9].num = i + 1;
            cubes[i + 9].numText.text = "" + (i + 1);
        }

        // Store original positions of number graphics
        originalPosition = new Vector3[cubes.Length];
        for (int i = 0; i < cubes.Length; i++)
        {
            originalPosition[i] = new Vector3(cubes[i].transform.position.x, cubes[i].transform.position.y,
                                                cubes[i].transform.position.z);
        }


        if (isMultiplicationGame)
        {
            ResetGameMult();
            SetQuestionMult();
        }
        else if (isAdditionGame)
        {
            LoadLevel();
        }
        else if (is2OperatorGame)
        {
            level = 15.0f;
            LoadLevel();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (preStartGame)
        {
            PreStartGame();
        }

        // If multiplication game, check multiplication answers
        else if (isMultiplicationGame)
        {
            CheckAnswersMult(); // Check answers for Multiplication game
        }
        else if (isAdditionGame)
        {
            CheckAnswersAdd(); // Check answers for Addition game
        }
        else if (is2OperatorGame)
        {
            CheckAnswers2Op(); // Check answers for 2 operator game
        }
        else if (isSubtractionGame)
        {
            CheckAnswersSub(); // Check answers for subtraction game
        }
        else if (isDivisionGame)
        {
            CheckAnswersDiv(); // Check answers for division game
        }
        if (!preStartGame)
        {
            updateTimer(); // Update Timer  
        }  
    }

    // Update the countdown timer
    private void updateTimer()
    {
        if (timeLeft > 0)
        {
            timerText.text = "Time: " + Mathf.Floor(timeLeft);
            timeLeft -= Time.deltaTime;
            amountCorrectText.text = "Correct: " + amountCorrect; // Update amount correct text
            numberOfTriesText.text = "Tries: " + numberOfTries;

            // If there's less than 10 seconds left play ticking sound
            if (timeLeft < 10 && !winner)
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

        else if (timeLeft <= 0 && !gameOver && !winner)
        {
            GameOver();
        }
    }

    // Correct answer routine
    private void CorrectRoutine()
    {
        audio.Stop();
        audio.clip = correctMusic;
        audio.Play();
        winner = true;
        timerText.text = "Correct!";
        restartTimer = 2.0f;
        amountCorrect++;
        numberOfTries++;
        level += 0.5f;
    }
    
    // Load the next level
    private void LoadLevel()
    {
        
        // Check what level the player is at
        switch ((int)level) 
        {
            // Level 1
            case 1: 
                ResetGameAdd();
                sum = Random.Range(3, 17);            
                highestNumber = 9;
                SetQuestionAdd();
                break;

            // Level 2
            case 2:
                if (level == 2.0f)
                {
                    audio.clip = levelUpSound;
                    audio.Play();
                }
                
                ResetGameAdd();
                // Assign new numbers to the variables and cubes
                SetNumbers(10);

                highestNumber = 18;
                bool valid = false;

                while (!valid)
                {
                    sum = Random.Range(11, 37);
                    if ((sum <= 27) || (sum >= 21))
                    {
                        valid = true;
                    }
                }
                SetQuestionAdd();
                break;

            // Level 3
            case 3:
                if (level == 3.0f)
                {
                    audio.clip = levelUpSound;
                    audio.Play();
                }
                ResetGameAdd();
                // Assign new numbers to the variables and cubes
                SetNumbers(20);

                highestNumber = 28;
                valid = false;

                while (!valid)
                {
                    sum = Random.Range(21, 55);
                    if ((sum <= 37) || (sum >= 41))
                    {
                        valid = true;
                    }
                }
                SetQuestionAdd();
                break;

            // Level 4
            case 4:
                if (level == 4.0f)
                {
                    audio.clip = levelUpSound;
                    audio.Play();
                }
                ResetGameAdd();
                // Assign new numbers to the variables and cubes
                SetNumbers(30);
                highestNumber = 38;
                valid = false;

                while (!valid)
                {
                    sum = Random.Range(31, 75);
                    if ((sum <= 47) || (sum >= 61))
                    {
                        valid = true;
                    }
                }
                SetQuestionAdd();
                break;

            // Level 5
            case 5:
                if (level == 5.0f)
                {
                    audio.clip = levelUpSound;
                    audio.Play();
                }
                ResetGameAdd();
                // Assign new numbers to the variables and cubes
                SetNumbers(40);

                highestNumber = 48;
                valid = false;

                while (!valid)
                {
                    sum = Random.Range(41, 95);
                    if ((sum <= 57) || (sum >= 81))
                    {
                        valid = true;
                    }
                }
                SetQuestionAdd();
                break;

            // Level 6
            case 6:
                if (level == 6.0f)
                {
                    audio.clip = levelUpSound;
                    audio.Play();
                }
                // Assign new numbers to the variables and cubes
                SetNumbers(1);
                ResetGameSub();
                sum = Random.Range(-8, 8);
                highestNumber = 9;
                SetQuestionSub();
                
                isAdditionGame = false;
                isSubtractionGame = true;
                break;

            // Level 7
            case 7:
                if (level == 7.0f)
                {
                    audio.clip = levelUpSound;
                    audio.Play();
                }
                // Assign new numbers to the variables and cubes
                SetNumbers(10);
                ResetGameSub();
                sum = 0;
                while (sum == 0)
                {
                    sum = Random.Range(-17, 17);
                }
  
                highestNumber = 18;
                SetQuestionSub();

                isAdditionGame = false;
                isSubtractionGame = true;
                break;

            // Level 8
            case 8:
                if (level == 8.0f)
                {
                    audio.clip = levelUpSound;
                    audio.Play();
                }
                // Assign new numbers to the variables and cubes
                SetNumbers(20);
                ResetGameSub();
                highestNumber = 28;
                valid = false;

                while (!valid)
                {
                    sum = Random.Range(-27, 27);
                    if ((sum >= 11) || (sum <= 8) && (sum != 0))
                    {
                        valid = true;
                    }
                }
                SetQuestionSub();

                isAdditionGame = false;
                isSubtractionGame = true;
                break;

            // Level 9
            case 9:
                if (level == 9.0f)
                {
                    audio.clip = levelUpSound;
                    audio.Play();
                }
                SetNumbers(1);
                ResetGameMult();
                isSubtractionGame = false;
                isAdditionGame = false;
                isMultiplicationGame = true;
                SetQuestionMult();
                break;

            // Level 10
            case 10:
                if (level == 10.0f)
                {
                    audio.clip = levelUpSound;
                    audio.Play();
                }
                SetNumbers(10);
                ResetGameMult();
                SetQuestionMult();
                break;

            // Level 11
            case 11:
                if (level == 11.0f)
                {
                    audio.clip = levelUpSound;
                    audio.Play();
                }
                SetNumbers(20);
                ResetGameMult();
                SetQuestionMult();
                break;

            // Level 12
            case 12:
                if (level == 12.0f)
                {
                    audio.clip = levelUpSound;
                    audio.Play();
                }
                isSubtractionGame = false;
                isAdditionGame = false;
                isMultiplicationGame = false;
                isDivisionGame = true;
                SetNumbers(10);
                ResetGameDiv();
                SetQuestionDiv();        
                break;

            // Level 13
            case 13:
                if (level == 13.0f)
                {
                    audio.clip = levelUpSound;
                    audio.Play();
                }
                SetNumbers(20);
                ResetGameDiv();
                SetQuestionDiv();
                break;

            // Level 14
            case 14:
                if (level == 14.0f)
                {
                    audio.clip = levelUpSound;
                    audio.Play();
                }
                isSubtractionGame = false;
                isAdditionGame = false;
                isMultiplicationGame = false;
                isDivisionGame = true;
                SetNumbers(60);
                ResetGameDiv();
                SetQuestionDiv();
                break;

            // Level 15
            case 15:
                if (level == 15.0f)
                {
                    audio.clip = levelUpSound;
                    audio.Play();
                }
                ResetGame2Op();
                SetQuestion2Op();
                break;

            // Level 16
            case 16:
                if (level == 16.0f)
                {
                    audio.clip = levelUpSound;
                    audio.Play();
                }
                SetNumbers(10);
                ResetGame2Op();
                SetQuestion2Op();
                break;

            // Complete game sequence
            case 17:
                GameComplete();
                break;
        }
    }

    // Sets the number objects to the tens base passed to the method eg. 10s, 20s, 30s
    private void SetNumbers(int tens)
    {
        if (isDivisionGame)
        {
            if (tens >= 20)
            {
                tens -= 1;
            }
           
            for (int i = 0; i < 9; i++)
            {
               
                cubes[i].num = i + 2;
                cubes[i].numText.text = "" + (i + 2);
                cubes[i + 9].num = i + tens + 1;
                cubes[i + 9].numText.text = "" + (i + tens + 1);
            }
        }
        else
        {
            for (int i = 0; i < 9; i++)
            {
                cubes[i + 9].num = i + tens;
                cubes[i + 9].numText.text = "" + (i + tens);
            }
        }
    }

    // Game over routine
    private void GameOver()
    {
        timerText.text = "Game Over!";
        gameOver = true;
        answerText.text = "";

        accuracy = (amountCorrect / numberOfTries) * 100; // Calculate player's accuracy 
        amountCorrectText.text = "Accuracy: " + Mathf.Floor(accuracy) + "%"; // Display the accuracy feedback 
        numberOfTriesText.text = "";
        audio.Stop();
        audio.clip = timeOutSound;
        audio.Play();
    }

    // Pre start routine
    private void PreStartGame()
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
        }
        preStartTimer -= Time.deltaTime;
    }



    // Complete game routine
    private void GameComplete()
    {
        timerText.text = "Complete !!";
        gameOver = true;
        answerText.text = "";

        accuracy = (amountCorrect / numberOfTries) * 100; // Calculate player's accuracy 
        amountCorrectText.text = "Accuracy: " + Mathf.Floor(accuracy) + "%"; // Display the accuracy feedback 
        numberOfTriesText.text = "";
        audio.Stop();
        audio.clip = crowdSound;
        audio.Play();
        timeLeft = 0;
    }



    // --------------------------- ADDITION GAME METHODS ----------------------------------------------------------- //

    // Reset addition game
    private void ResetGameAdd()
    {
        op1.correctAnswer = false;
        op2.correctAnswer = false;

        timeLeft = startingTime;
        previousTime = startingTime;

        // Allow all numbers to be placed again
        for (int i = 0; i < cubes.Length; i++)
        {
            cubes[i].correctNumber = true; // Reset all correctNumber booleans to true
            cubes[i].hasBeenPlaced1 = false; // Reset placed on 1st wall boolean to false
            cubes[i].hasBeenPlaced2 = false; // Reset placed on 2nd wall boolean to false
            cubes[i].op1Allowed = true;
            cubes[i].op2Allowed = true;
            cubes[i].transform.position = originalPosition[i]; // Reset positions of numbers
            cubes[i].transform.rotation = Quaternion.identity; // Reset rotations of numbers
        }
        levelText.text = "Level: " + (int)level;
        op1.GetComponent<Renderer>().material.color = new Color(0.6f, 0.6f, 0.6f);
        op2.GetComponent<Renderer>().material.color = new Color(0.6f, 0.6f, 0.6f);
    }

    // Set question for addition game
    private void SetQuestionAdd()
    {      
        // Initial checks to see if numbers should be allowed to be placed under certain conditions
        for (int i = 0; i < cubes.Length; i++)
        {
            // Set the numbers to false if they cant possibly add up to the sum
            if ((cubes[i].num + lowestNumber > sum) || (cubes[i].num + highestNumber < sum))
            {
                cubes[i].correctNumber = false;
            }

            if ((sum - cubes[i].num > 9) && (sum - cubes[i].num < (highestNumber - 8)))
            {
                cubes[i].correctNumber = false;
            }
        }
        // If sum is less than 10 then set the number equal to the sum as false
        if (sum < 10)
        {
            cubes[sum - 1].correctNumber = false;
        }
        // For levels 2 and higher
        if (level >= 2)
        {
            // If sum is divisible to 2, set the number that is equal to half the sum to false
            if (sum % 2 == 0)
            {
                cubes[((sum / 2) - (highestNumber - 8)) + 9].correctNumber = false;
            }
        }
              
        answerText.text = sum + "";
        operatorText.text = "+";

    }

    // Check answers for addition game
    private void CheckAnswersAdd()
    {
        if (!gameOver)
        {
            if (op1.correctAnswer)
            {
                for (int i = 0; i < cubes.Length; i++)
                {
                    if (cubes[i].hasBeenPlaced1)
                    {
                        for (int j = 0; j < cubes.Length; j++)
                        {
                            if (j != i)
                            {
                                // If the number placed, plus the remaining number doesnt total the sum,
                                //  set it's correctNumber boolean to false
                                if ((cubes[i].num + cubes[j].num) != sum)
                                {
                                    cubes[j].correctNumber = false;
                                }
                            }
                        }
                        cubes[i].transform.position = new Vector3(op1.transform.position.x,
                            op1.transform.position.y, op1.transform.position.z - 0.1f);
                        cubes[i].transform.rotation = Quaternion.identity;
                    }
                }
            }

            if (op2.correctAnswer)
            {
                for (int i = 0; i < cubes.Length; i++)
                {
                    if (cubes[i].hasBeenPlaced2)
                    {
                        for (int j = 0; j < cubes.Length; j++)
                        {
                            if (j != i)
                            {
                                // If the number placed, plus the remaining number doesnt total the sum,
                                //  set it's correctNumber boolean to false
                                if ((cubes[i].num + cubes[j].num) != sum)
                                {
                                    cubes[j].correctNumber = false;
                                }
                            }
                        }

                        cubes[i].transform.position = new Vector3(op2.transform.position.x,
                      op2.transform.position.y, op2.transform.position.z - 0.1f);
                        cubes[i].transform.rotation = Quaternion.identity;
                    }
                }
            }


            if (op1.correctAnswer && op2.correctAnswer && !winner)
            {
                CorrectRoutine();
            }

            if (winner)
            {
                restartTimer -= Time.deltaTime;
                if (restartTimer < 0f)
                {
                    audio.Stop();
                    LoadLevel();
                    winner = false;
                }
            }
        }
    }

    // ---------------------------------- END OF ADDITION GAME METHODS ---------------------------------------------- //

    // --------------------------- SUBTRACTION GAME METHODS ----------------------------------------------------------- //

    // Reset addition game
    private void ResetGameSub()
    {
        op1.correctAnswer = false;
        op2.correctAnswer = false;

        timeLeft = startingTime;
        previousTime = startingTime;

        // Allow all numbers to be placed again
        for (int i = 0; i < cubes.Length; i++)
        {
            cubes[i].correctNumber = true; // Reset all correctNumber booleans to true
            cubes[i].hasBeenPlaced1 = false; // Reset placed on 1st wall boolean to false
            cubes[i].hasBeenPlaced2 = false; // Reset placed on 2nd wall boolean to false
            cubes[i].op1Allowed = true;
            cubes[i].op2Allowed = true;
            cubes[i].transform.position = originalPosition[i]; // Reset positions of numbers
            cubes[i].transform.rotation = Quaternion.identity; // Reset rotations of numbers
        }
        levelText.text = "Level: " + (int)level;
        op1.GetComponent<Renderer>().material.color = new Color(0.6f, 0.6f, 0.6f);
        op2.GetComponent<Renderer>().material.color = new Color(0.6f, 0.6f, 0.6f);
    }

    // Set question for Subtraction game
    private void SetQuestionSub()
    {
        // Initial checks to see if numbers should be allowed to be placed under certain conditions
        for (int i = 0; i < cubes.Length; i++)
        {
            // Set the numbers to false if they cant possibly subtract to the sum
            if ((cubes[i].num <= sum) || (cubes[i].num - highestNumber > sum))
            {
                cubes[i].op1Allowed = false;
            }

            for (int j = 0; j < cubes.Length; j++)
            {
                if ((highestNumber - cubes[j].num) < sum || (1 - cubes[j].num  > sum))
                {
                    cubes[j].op2Allowed = false;
                }            
            }      
        }
              
        answerText.text = sum + "";
        operatorText.text = "-";

    }

    // Check answers for subtraction game
    private void CheckAnswersSub()
    {
        if (!gameOver)
        {
            if (op1.correctAnswer)
            {
                for (int i = 0; i < cubes.Length; i++)
                {
                    if (cubes[i].hasBeenPlaced1)
                    {
                        for (int j = 0; j < cubes.Length; j++)
                        {
                            if (j != i)
                            {
                                // If the number placed, plus the remaining number doesnt total the sum,
                                //  set it's correctNumber boolean to false
                                if ((cubes[i].num - cubes[j].num) != sum)
                                {
                                    cubes[j].correctNumber = false;
                                }
                            }
                        }
                        cubes[i].transform.position = new Vector3(op1.transform.position.x,
                            op1.transform.position.y, op1.transform.position.z - 0.1f);
                        cubes[i].transform.rotation = Quaternion.identity;
                    }
                }
            }

            if (op2.correctAnswer)
            {
                for (int i = 0; i < cubes.Length; i++)
                {
                    if (cubes[i].hasBeenPlaced2)
                    {
                        for (int j = 0; j < cubes.Length; j++)
                        {
                            if (j != i)
                            {
                                // If the number placed, plus the remaining number doesnt total the sum,
                                //  set it's correctNumber boolean to false
                                if ((cubes[j].num - cubes[i].num) != sum)
                                {
                                    cubes[j].correctNumber = false;
                                }
                            }
                        }

                        cubes[i].transform.position = new Vector3(op2.transform.position.x,
                      op2.transform.position.y, op2.transform.position.z - 0.1f);
                        cubes[i].transform.rotation = Quaternion.identity;
                    }
                }
            }


            if (op1.correctAnswer && op2.correctAnswer && !winner)
            {
                CorrectRoutine();
            }

            if (winner)
            {
                restartTimer -= Time.deltaTime;
                if (restartTimer < 0f)
                {
                    audio.Stop();
                    LoadLevel();
                    winner = false;
                }
            }
        }
    }

    // ---------------------------------- END OF SUBTRACTION GAME METHODS ---------------------------------------------- //


    // ------------------------------------- MULTIPLICATION GAME METHODS --------------------------------------------- //

    // Reset multiplication game
    private void ResetGameMult()
    {
        op1.correctAnswer = false;
        op2.correctAnswer = false;

        timeLeft = startingTime;
        previousTime = startingTime;


        // Allow all numbers to be placed again
        for (int i = 0; i < cubes.Length; i++)
        {
            cubes[i].correctNumber = false; // Reset all correctNumber booleans to false
            cubes[i].hasBeenPlaced1 = false; // Reset placed on 1st wall boolean to false
            cubes[i].hasBeenPlaced2 = false; // Reset placed on 2nd wall boolean to false 
            cubes[i].op1Allowed = false;
            cubes[i].op2Allowed = false;
            cubes[i].transform.position = originalPosition[i]; // Reset positions of numbers
            cubes[i].transform.rotation = Quaternion.identity; // Reset rotations of numbers
        }
        op1.GetComponent<Renderer>().material.color = new Color(0.6f, 0.6f, 0.6f);
        op2.GetComponent<Renderer>().material.color = new Color(0.6f, 0.6f, 0.6f);
    }

    // Set question for multiplication game
    private void SetQuestionMult()
    {
        bool nonDivisible = true;

        while (nonDivisible)
        {
            switch ((int)level)
            {
                case 9:
                    sum = Random.Range(2, 81);
                    break;
                case 10:
                    sum = Random.Range(10, 162);
                    break;
                case 11:
                    sum = Random.Range(20, 252);
                    break;
            }
                
            // Check if the random number generated is divisible by numbers between 1 and 10 
            for (int i = 0; i < cubes.Length; i++)
            {
                if ((sum % cubes[i].num) != 0)
                {
                    nonDivisible = true;
                }
                else 
                {
                    for (int j = 0; j < cubes.Length; j++)
                    {
                        if (cubes[i].num % cubes[j].num == 0)
                        {
                            nonDivisible = false;
                        }
                    }
                    
                }

                if ((cubes[i].num * cubes[i].num) == sum)
                {
                    nonDivisible = true;
                }
            }
        }
     
       // Calculate and set the correct number combinations that can be placed
       for (int i = 0; i < cubes.Length; i++)
       {
            // Multiply the number selected with a number between 1 and 10 to see if the 
            // calculation matches the sum
            for (int j = 0; j < cubes.Length; j++)
            {
                if (i != j) // Cant reuse the same number object 
                {
                    if ((cubes[i].num * cubes[j].num) == sum)
                    {
                        cubes[i].correctNumber = true;
                        cubes[i].op1Allowed = true;
                        cubes[i].op2Allowed = true;
                    }
                }
            }      
       }
        answerText.text = sum + "";
        operatorText.text = "x";
    }


    // Check answers for multiplication game
    private void CheckAnswersMult()
    {
        if (!gameOver)
        {
            if (op1.correctAnswer)
            {
                // Find out which number has been placed on the board
                for (int i = 0; i < cubes.Length; i++)
                {
                    if (cubes[i].hasBeenPlaced1)
                    {
                        for (int j = 0; j < cubes.Length; j++)
                        {
                            if (j != i) // We dont want to check the same number that has been placed
                            {
                                // If the number placed, multiplied by the remaining number doesnt total the sum,
                                //  set it's correctNumber boolean to false
                                if ((cubes[i].num * cubes[j].num) != sum)
                                {
                                    cubes[j].correctNumber = false;
                                }
                            }
                        }
                        // Make the number "stick" to the 1st wall
                        cubes[i].transform.position = new Vector3(op1.transform.position.x,
                            op1.transform.position.y, op1.transform.position.z - 0.1f);
                        cubes[i].transform.rotation = Quaternion.identity;
                    }
                }
            }

            if (op2.correctAnswer)
            {
                for (int i = 0; i < cubes.Length; i++)
                {
                    if (cubes[i].hasBeenPlaced2)
                    {
                        for (int j = 0; j < cubes.Length; j++)
                        {
                            if (j != i)
                            {
                                // If the number placed, multiplied by the remaining number doesnt total the sum,
                                //  set it's correctNumber boolean to false
                                if ((cubes[i].num * cubes[j].num) != sum)
                                {
                                    cubes[j].correctNumber = false;
                                }
                            }
                        }

                        // Make the number "stick" to the 2nd wall
                        cubes[i].transform.position = new Vector3(op2.transform.position.x,
                            op2.transform.position.y, op2.transform.position.z - 0.1f);
                        cubes[i].transform.rotation = Quaternion.identity;
                    }
                }
            }


            if (op1.correctAnswer && op2.correctAnswer && !winner)
            {
                CorrectRoutine();
            }

            if (winner)
            {
                restartTimer -= Time.deltaTime;
                if (restartTimer < 0f)
                {
                    audio.Stop();
                    LoadLevel();
                    winner = false;
                }

            }
        }
    }

    //------------------------------------------ END OF MULTIPICATION GAME METHODS -------------------------------------//

    // --------------------------- DIVISION GAME METHODS ----------------------------------------------------------- //

    // Reset addition game
    private void ResetGameDiv()
    {
        op1.correctAnswer = false;
        op2.correctAnswer = false;

        timeLeft = startingTime;
        previousTime = startingTime;

        // Allow all numbers to be placed again
        for (int i = 0; i < cubes.Length; i++)
        {
            cubes[i].correctNumber = true; // Reset all correctNumber booleans to true
            cubes[i].hasBeenPlaced1 = false; // Reset placed on 1st wall boolean to false
            cubes[i].hasBeenPlaced2 = false; // Reset placed on 2nd wall boolean to false
            cubes[i].op1Allowed = true;
            cubes[i].op2Allowed = true;
            cubes[i].transform.position = originalPosition[i]; // Reset positions of numbers
            cubes[i].transform.rotation = Quaternion.identity; // Reset rotations of numbers
        }
        levelText.text = "Level: " + (int)level;
        op1.GetComponent<Renderer>().material.color = new Color(0.6f, 0.6f, 0.6f);
        op2.GetComponent<Renderer>().material.color = new Color(0.6f, 0.6f, 0.6f);
    }

    // Set question for Subtraction game
    private void SetQuestionDiv()
    {
        switch ((int)level)
        {
            case 12:
                sum = Random.Range(2, 9);
                break;
            case 13:
                sum = Random.Range(2, 14);
                break;
            case 14:
                bool valid = false;
                while (!valid)
                {
                    valid = true;
                    sum = Random.Range(2, 34);
                    if ((sum >= 23 && sum < 30) || (sum < 20 && sum > 13))
                    {
                        valid = false;
                    }
                }
                break;
        }
        // Initial checks to see if numbers should be allowed to be placed under certain conditions
        for (int i = 0; i < cubes.Length; i++)
        {
            // Set the numbers to false if they cant possibly divide to the sum
            if (cubes[i].num <= sum)
            {
                cubes[i].op1Allowed = false;
            }
           
            cubes[i].op1Allowed = false;
            cubes[i].op2Allowed = false;
            for (int j = 0; j < cubes.Length; j++)
            {
                if (((float)cubes[i].num / (float)cubes[j].num) == (float)sum)
                {
                    cubes[i].op1Allowed = true;
                }
                if ((sum * cubes[i].num) == cubes[j].num)
                {
                    cubes[i].op2Allowed = true;
                }
            }          
        }
   
        answerText.text = sum + "";
        operatorText.text = "/";

    }

    // Check answers for division game
    private void CheckAnswersDiv()
    {
        if (!gameOver)
        {
            if (op1.correctAnswer)
            {
                for (int i = 0; i < cubes.Length; i++)
                {
                    if (cubes[i].hasBeenPlaced1)
                    {
                        for (int j = 0; j < cubes.Length; j++)
                        {
                            if (j != i)
                            {
                                // If the number placed, plus the remaining number doesnt total the sum,
                                //  set it's correctNumber boolean to false
                                if ((sum * cubes[j].num) != cubes[i].num)
                                {
                                    cubes[j].correctNumber = false;
                                    cubes[j].op2Allowed = false;
                                }
                            }
                        }
                        cubes[i].transform.position = new Vector3(op1.transform.position.x,
                            op1.transform.position.y, op1.transform.position.z - 0.1f);
                        cubes[i].transform.rotation = Quaternion.identity;
                    }
                }
            }

            if (op2.correctAnswer)
            {
                for (int i = 0; i < cubes.Length; i++)
                {
                    if (cubes[i].hasBeenPlaced2)
                    {
                        for (int j = 0; j < cubes.Length; j++)
                        {
                            if (j != i)
                            {
                                // If the number placed, plus the remaining number doesnt total the sum,
                                //  set it's correctNumber boolean to false
                                if ((cubes[j].num / cubes[i].num) != sum)
                                {
                                    cubes[j].correctNumber = false;
                                    cubes[j].op1Allowed = false;
                                }
                            }
                        }

                        cubes[i].transform.position = new Vector3(op2.transform.position.x,
                      op2.transform.position.y, op2.transform.position.z - 0.1f);
                        cubes[i].transform.rotation = Quaternion.identity;
                    }
                }
            }


            if (op1.correctAnswer && op2.correctAnswer && !winner)
            {
                CorrectRoutine();
            }

            if (winner)
            {
                restartTimer -= Time.deltaTime;
                if (restartTimer < 0f)
                {
                    audio.Stop();
                    if (level == 15.0f)
                    {
                        SceneManager.LoadScene("InteractiveMaths2Operators");
                    }
                    LoadLevel();
                    winner = false;
                }
            }
        }
    }

    // ---------------------------------- END OF DIVISION GAME METHODS ---------------------------------------------- //

    // ---------------------------------- 2 OPERATOR GAME METHODS ------------------------------------------------------//

    // Reset 2 Operator game
    private void ResetGame2Op()
    {
        op1.correctAnswer = false;
        op2.correctAnswer = false;
        op3.correctAnswer = false;

        timeLeft = startingTime;
        previousTime = startingTime;

        // Do not allow any numbers to be placed initially
        for (int i = 0; i < cubes.Length; i++)
        {
            cubes[i].correctNumber = false; // Reset all correctNumber booleans to false for multiplication
            cubes[i].op1Allowed = false; //  Do not allow to be placed on operand 1
            cubes[i].op2Allowed = false; // Do not allow to be placed on operand 2
            cubes[i].correctNumber2Operator = false; // Reset all correctNumber booleans to false for addition
            cubes[i].hasBeenPlaced1 = false; // Reset placed on 1st wall boolean to false
            cubes[i].hasBeenPlaced2 = false; // Reset placed on 2nd wall boolean to false
            cubes[i].hasBeenPlaced3 = false; // Reset placed on 3rd wall boolean to false
            cubes[i].transform.position = originalPosition[i]; // Reset positions of numbers
            cubes[i].transform.rotation = Quaternion.identity; // Reset rotations of numbers
            firstNumberPlaced = 0;
            secondNumberPlaced = 0;
            numbersPlaced = 0;        
        }
        levelText.text = "Level: " + (int)level;
        op1.GetComponent<Renderer>().material.color = new Color(0.6f, 0.6f, 0.6f);
        op2.GetComponent<Renderer>().material.color = new Color(0.6f, 0.6f, 0.6f);
        op3.GetComponent<Renderer>().material.color = new Color(0.6f, 0.6f, 0.6f);
    }

    private void SetQuestion2Op()
    {
        switch ((int)level)
        {
            case 15:
                sum = Random.Range(3, 89);
                break;

            case 16:
                sum = Random.Range(12, 170);
                break;
        }
        
        
        // Calculate and set the correct number combinations that can be placed
        for (int i = 0; i < cubes.Length; i++)
        {
            // Multiply the number selected with a number between 1 and 10 and add a number between 1 and 10 
            // to see if the calculation matches the sum
            for (int j = 0; j < cubes.Length; j++)
            {
                //if ((i + 1) != j)
                //{
                    for (int k = 1; k < 10; k++)
                    {
                            if ((cubes[i].num * cubes[j].num) + k == sum)
                            {
                                cubes[i].correctNumber = true;
                                //cubes[i + 9].correctNumber = true;
                                cubes[i].op1Allowed = cubes[i].op2Allowed = true;
                                //cubes[i + 9].op1Allowed = cubes[i + 9].op2Allowed = true;
                                cubes[j].correctNumber = true;
                               // cubes[j + 9].correctNumber = true;
                                cubes[j].op1Allowed = cubes[j].op2Allowed = true;
                               // cubes[j + 9].op1Allowed = cubes[j + 9].op2Allowed = true;
                    }
   
                    }
                    
                //}
            }
        }
        answerText.text = sum + "";
        operatorText.text = "x";
        operator2Text.text = "+";
    }

    // Check answers for 2 Operator game
    private void CheckAnswers2Op()
    {
        if (!gameOver)
        {
            if (op1.correctAnswer)
            {           
                // Find out which number has been placed on the board
                for (int i = 0; i < cubes.Length; i++)
                {
                    if (cubes[i].hasBeenPlaced1)
                    {
                        numbersPlaced++;
                        firstNumberPlaced = i;
                        for (int j = 0; j < cubes.Length; j++)
                        {
                            //if (j != i) // We dont want to check the same number that has been placed
                            //{
                                // If the number placed, multiplied by the remaining number doesnt total the sum,
                                //  set it's correctNumber boolean to false
                                if ((cubes[i].num * cubes[j].num) >= sum)
                                {
                                    cubes[j].correctNumber = false; // Cannot have the 2nd operand greater or equal to sum
                                }
                                else if (cubes[i].num * cubes[j].num + 9 < sum)
                                {
                                    cubes[j].correctNumber = false;    
                                }
                            //}
                        }
                        // Make the number "stick" to the 1st wall
                        cubes[i].transform.position = new Vector3(op1.transform.position.x,
                            op1.transform.position.y, op1.transform.position.z - 0.1f);
                        cubes[i].transform.rotation = Quaternion.identity;
                    }
                }
            }

            if (op2.correctAnswer)
            {
               
                for (int i = 0; i < cubes.Length; i++)
                {
                    if (cubes[i].hasBeenPlaced2)
                    {
                        numbersPlaced++;
                        secondNumberPlaced = i;
                        for (int j = 0; j < cubes.Length; j++)
                        {
                            //if (j != i) // We dont want to check the same number that has been placed
                           // {
                                // If the number placed, multiplied by the remaining number doesnt total the sum,
                                //  set it's correctNumber boolean to false
                                if ((cubes[i].num * cubes[j].num) >= sum)
                                {
                                    cubes[j].correctNumber = false; // Cannot have 1st operand greater or equal to sum
                                }
                                else if (cubes[i].num * cubes[j].num + 9 < sum)
                                {
                                    cubes[j].correctNumber = false;
                                }
                            // }
                        }
                            // Make the number "stick" to the 2nd wall
                            cubes[i].transform.position = new Vector3(op2.transform.position.x,
                                op2.transform.position.y, op2.transform.position.z - 0.1f);
                            cubes[i].transform.rotation = Quaternion.identity;
                    }
                }
            }

            if (numbersPlaced >= 2)
            {
                for (int i = 0; i < 9; i++)
                {
                    if (((cubes[firstNumberPlaced].num * cubes[secondNumberPlaced].num) + (i+1)) == sum)
                    {
                        cubes[i].correctNumber2Operator = true;
                        cubes[i + 9].correctNumber2Operator = true;
                    }
                }
               
            }


            if (op3.correctAnswer)
            {
                for (int i = 0; i < cubes.Length; i++)
                {
                    if (cubes[i].hasBeenPlaced3)
                    {
                        
                        // Make the number "stick" to the 2nd wall
                        cubes[i].transform.position = new Vector3(op3.transform.position.x,
                            op3.transform.position.y, op3.transform.position.z - 0.1f);
                        cubes[i].transform.rotation = Quaternion.identity;
                    }
                }
            }
            if (op1.correctAnswer && op2.correctAnswer && op3.correctAnswer && !winner)
            {
                CorrectRoutine();
            }

            if (winner)
            {
                restartTimer -= Time.deltaTime;
                if (restartTimer < 0f)
                {
                    audio.Stop();
                    LoadLevel();
                    winner = false;
                }
            }
        }
    }
}

