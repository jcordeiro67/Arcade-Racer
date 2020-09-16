using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour {

	public AudioClip [] UIBackgroundMusic;
	AudioSource audioSource;

	// Start is called before the first frame update
	void Start ()
	{
		audioSource = GetComponent<AudioSource> ();
		int audioClip = Random.Range (0, UIBackgroundMusic.Length);
		audioSource.clip = UIBackgroundMusic [audioClip];
		audioSource.Play ();
	}

}
