using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoleBashSound : MonoBehaviour {
    public AudioSource hitSound;

	// Use this for initialization
	void Start () {
        hitSound = GetComponent<AudioSource>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerEnter(Collider other)
    {
        hitSound.Play();
    }
}
