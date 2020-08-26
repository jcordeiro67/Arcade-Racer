using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour {

	public Camera mainCamers;

	// Start is called before the first frame update
	void Start ()
	{
		if (mainCamers == null) {
			mainCamers = Camera.main;
		}
	}

	// Update is called once per frame
	void Update ()
	{
		if (mainCamers.name != Camera.main.name) {
			mainCamers = Camera.main;
		}

		transform.LookAt (mainCamers.transform);
	}
}
