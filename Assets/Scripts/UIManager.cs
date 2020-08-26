using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour {

	public static UIManager instance;

	public TMP_Text lapCounterText;
	public TMP_Text bestLapTimeText;
	public TMP_Text currentLapTimeText;
	public TMP_Text playerPosText;
	public TMP_Text countdownText;
	public TMP_Text raceStartText;


	// Start is called before the first frame update
	void Awake ()
	{
		instance = this;

	}

	// Update is called once per frame
	void Update ()
	{

	}

}
