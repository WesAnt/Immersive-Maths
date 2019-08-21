using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RestartButton : MonoBehaviour {

    public bool algebraGame;

    private void OnTriggerEnter(Collider other)
    {
        if (algebraGame)
        {
            SceneManager.LoadScene("InteractiveMathsAlgebra");
        }
        else
        {
            SceneManager.LoadScene("InteractiveMaths");
        }
    }
        

}
