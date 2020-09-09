using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RaceInfoManager : MonoBehaviour {

	public static RaceInfoManager instance;

	public string trackToLoad;
	public CarController racerCar;
	public int noOfAi;
	public int noOfLaps;


	private void Awake ()
	{
		//Singleton
		if (instance == null) {
			instance = this;
			DontDestroyOnLoad (gameObject);
		} else {
			Destroy (gameObject);
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
