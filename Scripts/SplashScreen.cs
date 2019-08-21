using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

// The following code was adapted from a youtube tutorial by Mark Phillip of Studica.com
// -------------------------------------------------------------------------------------

public class SplashScreen : MonoBehaviour
{
    Image uiImage;
    Canvas parentCanvas;

    [SerializeField] Sprite[] images; // Pictures we want to cycle through

    [SerializeField] bool clickToProceed;

    [SerializeField] float fadeTime; // Amount of time it takes to fade an image

    [SerializeField] float displayTime;

    [SerializeField] float transparentTime; //  Amount of time an image stays transparent before fading in


    void Start () {
        parentCanvas = GetComponent<Canvas>();

        if (parentCanvas.worldCamera != Camera.main)
        {
            parentCanvas.worldCamera = Camera.main;

        }

        DontDestroyOnLoad(gameObject);

        uiImage = GetComponentInChildren<Image>();
        uiImage.sprite = images[0];

        StartCoroutine(CycleImages());
	}


    IEnumerator CycleImages()
    {
        if (!clickToProceed)
        {
            for (int i = 0; i < images.Length; i++)
            {
                uiImage.sprite = images[i];
                uiImage.color = new Color(uiImage.color.r, uiImage.color.g, uiImage.color.b, 0);

                yield return new WaitForSeconds(transparentTime);

                // fade in for loop
                for (float alpha = 0; alpha < 1; alpha += Time.deltaTime / fadeTime)
                {
                    uiImage.color = new Color(uiImage.color.r, uiImage.color.g, uiImage.color.b, alpha);
                    yield return null; // wait for a frame and then return to execution
                }

                yield return new WaitForSeconds(displayTime);

                // Fade out for loop
                for (float alpha = 1; alpha > 0; alpha -= Time.deltaTime / fadeTime)
                {
                    uiImage.color = new Color(uiImage.color.r, uiImage.color.g, uiImage.color.b, alpha);
                    yield return null; // wait for a frame and then return to execution
                }

            }
            yield return new WaitForSeconds(3);
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }
}
