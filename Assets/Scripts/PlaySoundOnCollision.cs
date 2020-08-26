using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaySoundOnCollision : MonoBehaviour {

	public AudioSource CarHitSound;
	public int groundLayerIndex = 8;
	public Vector2 hitSoundVarianceRange = new Vector2 (0.6f, 1.2f);

	private void OnCollisionEnter (Collision other)
	{
		if (CarHitSound != null && other.gameObject.layer != groundLayerIndex) {
			CarHitSound.Stop ();
			CarHitSound.pitch = Random.Range (0.6f, 1.2f);
			CarHitSound.Play ();
		}


	}

	// Start is called before the first frame update
	void Start ()
	{

	}

	// Update is called once per frame
	void Update ()
	{

	}
}
