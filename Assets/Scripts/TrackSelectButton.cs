using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TrackSelectButton : MonoBehaviour {

	public string trackSceneName;
	public Image trackImage;
	public int raceLaps = 4;
	public GameObject lockedText;
	public string trackToUnlockOnWin;

	private bool isLocked;

	// Start is called before the first frame update
	void Start ()
	{
		if (!PlayerPrefs.HasKey (trackSceneName + "_unlocked")) {
			isLocked = true;
			trackImage.color = Color.gray;
			lockedText.SetActive (true);
			GetComponent<Button> ().interactable = false;
		}
	}

	// Update is called once per frame
	void Update ()
	{

	}

	public void SelectTrack ()
	{
		if (!isLocked) {

			RaceInfoManager.instance.trackToLoad = trackSceneName;
			RaceInfoManager.instance.noOfLaps = raceLaps;
			RaceInfoManager.instance.trackSprite = trackImage.sprite;

			MainMenu.instance.trackSelectImage.sprite = trackImage.sprite;

			MainMenu.instance.CloseTrackSelect ();
			RaceInfoManager.instance.trackToUnlock = trackToUnlockOnWin;
		}

	}
}
