using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RespondToGaze : MonoBehaviour {

    public SteamVR_TrackedObject rightHand;
    public SteamVR_TrackedObject leftHand;

    private SteamVR_Controller.Device leftHandController;
    private SteamVR_Controller.Device rightHandController;

    public bool highlight = true;
    public bool soundTriggered = false;
    public GameObject cameraRig;
    private Button button;
    private bool isSelected;
    public AudioSource audio;
    public AudioClip selectSound;

	// Use this for initialization
	void Start () {
        button = GetComponent<Button>();
        audio = GetComponent<AudioSource>();
        audio.clip = selectSound;
    }

	
	// Update is called once per frame
	void Update () {
        leftHandController = SteamVR_Controller.Input((int)leftHand.index);
        rightHandController = SteamVR_Controller.Input((int)rightHand.index);
        isSelected = false;
        Transform camera = cameraRig.transform;
        Ray ray = new Ray(camera.position, camera.rotation * Vector3.forward);
        RaycastHit hit;
        if (Physics.Raycast (ray, out hit) &&
            (hit.transform.parent != null) &&
            (hit.transform.parent.gameObject == gameObject))
        {
            
            isSelected = true;
        }

        if (isSelected)
        {
            if (highlight)
            {
                
                button.Select();
                if (!soundTriggered)
                {
                    audio.Play();
                    soundTriggered = true;
                }

            }

            if ((leftHandController != null && leftHandController.GetPressDown(SteamVR_Controller.ButtonMask.Trigger)) || 
                    (rightHandController != null && rightHandController.GetPressDown(SteamVR_Controller.ButtonMask.Trigger)))
            {
                button.onClick.Invoke();
            }
        }
        else
        {
            audio.Stop();
            soundTriggered = false;
            UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(null);
        }  
	}
}
