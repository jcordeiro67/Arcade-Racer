using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RacerSelectButton : MonoBehaviour {

	public string racerName;
	public Image racerImage;
	public CarController racerToSet;
	public TMP_Text racerButtonLabel;

	private void Start ()
	{
		racerButtonLabel.text = racerName;
	}

	public void SelectRacer ()
	{
		RaceInfoManager.instance.racerCar = racerToSet;
		RaceInfoManager.instance.racerSprite = racerImage.sprite;

		MainMenu.instance.racerSelectImage.sprite = racerImage.sprite;

		MainMenu.instance.CloseRacerSelect ();

	}
}
