using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour {

    public Button interactiveMathsButton;

    private float timer = 5.0f;

	
	// Update is called once per frame
	void Update () {
        
	}

    public void LoadInteractiveMathsScene()
    {
        // Load interactive maths scene
        SceneManager.LoadScene("InteractiveMaths");
    }

    public void LoadMolesGame()
    {
        // Load moles game scene
        SceneManager.LoadScene("Moles");

    }

    public void LoadAlgebraGame()
    {
        // Load algebra game scene
        SceneManager.LoadScene("InteractiveMathsAlgebra");
    }

    public void QuitGame()
    {
        // Quit the game
        Application.Quit();
    }
}
