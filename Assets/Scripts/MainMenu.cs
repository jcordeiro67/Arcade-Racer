using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour {

	public static MainMenu instance;

	public GameObject raceSetupPanel, trackSelectPanel, racerSelectPanel;
	public Image trackSelectImage;
	public Image racerSelectImage;
	public Slider noOfLaps, noOfAI;



	private void Awake ()
	{
		instance = this;
	}

	private void Start ()
	{
		if (RaceInfoManager.instance.enteredRace) {
			trackSelectImage.sprite = RaceInfoManager.instance.trackSprite;
			racerSelectImage.sprite = RaceInfoManager.instance.racerSprite;
			noOfLaps.value = RaceInfoManager.instance.noOfLaps;
			noOfAI.value = RaceInfoManager.instance.noOfAi;
			OpenRaceSetup ();
		}

		if (!PlayerPrefs.HasKey (RaceInfoManager.instance.trackToLoad + "_unlocked")) {
			PlayerPrefs.SetInt (RaceInfoManager.instance.trackToLoad + "_unlocked", 1);
		}

		noOfLaps.value = RaceInfoManager.instance.noOfLaps;
		noOfAI.value = RaceInfoManager.instance.noOfAi;

	}

	public void StartRace ()
	{
		RaceInfoManager.instance.enteredRace = true;
		SceneManager.LoadScene (RaceInfoManager.instance.trackToLoad);
	}

	public void QuitGame ()
	{
		Application.Quit ();
		Debug.Log ("Quit GAme");
	}

	public void OpenRaceSetup ()
	{
		raceSetupPanel.SetActive (true);
	}

	public void CloseRaceSetup ()
	{
		raceSetupPanel.SetActive (false);
		RaceInfoManager.instance.enteredRace = false;
	}

	public void OpenTrackSelect ()
	{
		CloseRaceSetup ();
		trackSelectPanel.SetActive (true);
	}

	public void CloseTrackSelect ()
	{
		trackSelectPanel.SetActive (false);
		OpenRaceSetup ();
	}

	public void OpenRacerSelect ()
	{
		CloseRaceSetup ();
		racerSelectPanel.SetActive (true);
	}

	public void CloseRacerSelect ()
	{
		racerSelectPanel.SetActive (false);
		OpenRaceSetup ();
	}

	public void SetNoOfLaps ()
	{
		RaceInfoManager.instance.noOfLaps = (int)noOfLaps.value;
	}

	public void SetNoOfAI ()
	{
		RaceInfoManager.instance.noOfAi = (int)noOfAI.value;
	}

}
