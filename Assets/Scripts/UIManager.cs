using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {

	public static UIManager instance;

	public TMP_Text lapCounterText;
	public TMP_Text bestLapTimeText;
	public TMP_Text currentLapTimeText;
	public TMP_Text playerPosText;
	public TMP_Text countdownText;
	public TMP_Text raceStartText;
	public TMP_Text raceResultText;
	public Image trophyImage;
	public TMP_Text raceEndBestLapText;
	public GameObject raceResultsPanel;
	public GameObject trackUnlockedText;
	public GameObject pauseScrene;

	public bool isPaused;
	private Scene scene;
	private int sceneCount;

	void Awake ()
	{
		instance = this;

	}

	private void Start ()
	{
		//Get the (Scene)scene and (int)total scenes in build settings
		sceneCount = SceneManager.sceneCountInBuildSettings;
		scene = SceneManager.GetActiveScene ();
	}

	// Update is called once per frame
	void Update ()
	{
		if (Input.GetKeyDown (KeyCode.Escape)) {
			PauseUnpause ();
		}

	}

	//Exit race to main menu
	public void ExitRace ()
	{
		Time.timeScale = 1f;
		RaceInfoManager.instance.enteredRace = true;
		RaceManager.instance.ExitRace ("MainMenu");

	}

	//Restart the last race
	public void ReTryRace ()
	{
		Time.timeScale = 1;
		SceneManager.LoadScene (scene.buildIndex);
	}

	public void NextRace ()
	{
		if (scene.buildIndex < sceneCount - 1) {
			SceneManager.LoadScene (scene.buildIndex + 1);
		} else {
			SceneManager.LoadScene (0);
		}

	}

	//Pause and Unpause the game
	public void PauseUnpause ()
	{
		isPaused = !isPaused;
		pauseScrene.SetActive (isPaused);
		if (isPaused) {
			Time.timeScale = 0f;
		} else {
			Time.timeScale = 1f;
		}
	}
	//Quit the game
	public void QuitGame ()
	{
		Application.Quit ();
	}

}
