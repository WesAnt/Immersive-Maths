using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicHandler : MonoBehaviour {

    public AudioSource music;
	// Use this for initialization
	void Start () {
		
	}

    void Awake()
    {
        music = GetComponent<AudioSource>();
        music.Play();
    }

   // Update is called once per frame
    void Update () {
		
	}
}
