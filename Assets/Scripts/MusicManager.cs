using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (AudioSource))]
public class MusicManager : MonoBehaviour {

	public AudioClip [] UIBackgroundMusic;
	AudioSource audioSource;

	// Start is called before the first frame update
	void Start ()
	{
		audioSource = GetComponent<AudioSource> ();
		int audioClip = Random.Range (0, UIBackgroundMusic.Length);
		audioSource.clip = UIBackgroundMusic [audioClip];
		if (audioSource.loop == false) {
			audioSource.loop = true;
		}
		audioSource.Play ();
	}

}
