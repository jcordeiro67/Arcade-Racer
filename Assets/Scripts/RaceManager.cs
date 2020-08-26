using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaceManager : MonoBehaviour {

	public static RaceManager instance;
	[Header ("Race Settings")]
	[Range (1f, 100f)] public int totalLaps;
	public CarController playerCar;
	[Tooltip ("The array of checkPoints on the track")]
	public CheckPoint [] allCheckPoints;
	[Tooltip ("The List of AI Cars")]
	public List<CarController> allAICars = new List<CarController> ();
	public int playerPos;
	[Tooltip ("The time between position checks")]
	public float timeBetweenPosCheck = .2f;
	public float aiDefaultSpeed = 25f;
	public float playerDefaultSpeed = 25f;
	public float rubberBandSpeed = 3.5f;
	public float rubberBandAccel = .5f;
	private float posCheckCounter;
	public bool isStarting;
	public float timeBetweenStartCount = 1f;
	private float startCounter;
	public int countDownCurrent = 3;
	public ParticleSystem raceStartVFX;


	private void Awake ()
	{
		instance = this;

		//Assign each checkpoint it's number from the array index
		for (int i = 0; i < allCheckPoints.Length; i++) {
			allCheckPoints [i].checkPointNumber = i;
		}
	}

	// Start is called before the first frame update
	void Start ()
	{
		isStarting = true;
		startCounter = timeBetweenStartCount;
		//Enable Start Counter text and set to current countdown from Editor
		UIManager.instance.countdownText.gameObject.SetActive (true);
		UIManager.instance.countdownText.text = countDownCurrent + "!";
	}

	// Update is called once per frame
	void Update ()
	{
		if (isStarting) {

			startCounter -= Time.deltaTime;

			if (startCounter <= 0) {
				countDownCurrent--;
				startCounter = timeBetweenStartCount;
				//Update Start Counter
				UIManager.instance.countdownText.text = countDownCurrent + "!";
				//Play race countdown sound


				if (countDownCurrent == 0) {
					//Disable Start Counter
					UIManager.instance.countdownText.gameObject.SetActive (false);
					//Enable Race Start Text
					UIManager.instance.raceStartText.gameObject.SetActive (true);

					//Play race start vfx
					if (raceStartVFX != null) {
						raceStartVFX.Play ();

					}
					isStarting = false;
				}

			}

		} else {

			//Player position check timer
			posCheckCounter -= Time.deltaTime;

			if (posCheckCounter <= 0) {
				playerPos = 1;

				if (allAICars.Count >= 1) {

					//Check player position
					foreach (CarController aiCar in allAICars) {
						//Lap Ahead
						if (aiCar.currentLap > playerCar.currentLap) {
							playerPos++;
							//Same Lap
						} else if (aiCar.currentLap == playerCar.currentLap) {
							if (aiCar.nextCheckPoint > playerCar.nextCheckPoint) {
								playerPos++;
								//Same Checkpoint
							} else if (aiCar.nextCheckPoint == playerCar.nextCheckPoint) {
								if (Vector3.Distance (aiCar.transform.position, allCheckPoints [aiCar.nextCheckPoint].transform.position) < Vector3.Distance (playerCar.transform.position, allCheckPoints [aiCar.nextCheckPoint].transform.position)) {
									playerPos++;
								}
							}
						}
					}

				}
				//Reset posCheckCounter
				posCheckCounter = timeBetweenPosCheck;
				//Update Player position UI
				UIManager.instance.playerPosText.text = playerPos + "/" + (allAICars.Count + 1);
			}

			//Manage Rubber Banding
			if (playerPos == 1) {

				//If player is in First Place Speed up AI cars and slow down player car
				foreach (CarController aiCar in allAICars) {
					aiCar.maxSpeed = Mathf.MoveTowards (aiCar.maxSpeed, aiDefaultSpeed + rubberBandSpeed, rubberBandAccel * Time.deltaTime);
				}
				playerCar.maxSpeed = Mathf.MoveTowards (playerCar.maxSpeed, playerDefaultSpeed - rubberBandSpeed, rubberBandAccel * Time.deltaTime);
			} else {

				//Slow down AI cars depending on position to player
				foreach (CarController aiCar in allAICars) {
					aiCar.maxSpeed = Mathf.MoveTowards (aiCar.maxSpeed, aiDefaultSpeed - (rubberBandSpeed * ((float)playerPos / ((float)allAICars.Count + 1))), rubberBandAccel * Time.deltaTime);
				}

				//Speed up player car depending on player position
				playerCar.maxSpeed = Mathf.MoveTowards (playerCar.maxSpeed, playerDefaultSpeed + (rubberBandSpeed * ((float)playerPos / ((float)allAICars.Count + 1))), rubberBandAccel * Time.deltaTime);
			}
		}
	}
}



