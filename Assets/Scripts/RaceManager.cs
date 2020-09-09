using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RaceManager : MonoBehaviour {

	public static RaceManager instance;
	[Header ("Race Settings")]
	[Range (1f, 100f)] public int totalLaps;
	public CarController playerCar;
	[Tooltip ("The array of checkPoints on the track")]
	public CheckPoint [] allCheckPoints;
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
	public int countDownCurrent = 3;
	public ParticleSystem raceStartVFX;
	public int playerStartPosition;
	public int aiNumberToSpawn;
	public Transform [] startPositions;
	public List<CarController> carToSpawn = new List<CarController> ();
	public bool raceCompleted;
	private float startCounter;

	private void Awake ()
	{
		instance = this;

	}

	// Start is called before the first frame update
	void Start ()
	{
		totalLaps = RaceInfoManager.instance.noOfLaps;
		aiNumberToSpawn = RaceInfoManager.instance.noOfAi;

		//Assign each checkpoint it's number from the array index
		for (int i = 0; i < allCheckPoints.Length; i++) {
			allCheckPoints [i].checkPointNumber = i;
		}

		isStarting = true;
		//Ensure Race Results panel is off
		UIManager.instance.raceResultsPanel.SetActive (false);
		//Start countdown timer to start number
		startCounter = timeBetweenStartCount;
		//Enable Start Counter text and set to current countdown from Editor
		UIManager.instance.countdownText.gameObject.SetActive (true);
		UIManager.instance.countdownText.text = countDownCurrent + "!";

		//Spawn Player at random Start Position
		playerStartPosition = Random.Range (0, aiNumberToSpawn + 1);

		//Spawn player car at the playerStartPosition
		playerCar = Instantiate (RaceInfoManager.instance.racerCar, startPositions [playerStartPosition].position, startPositions [playerStartPosition].rotation);
		//Set playerCar ai to false
		playerCar.isAI = false;
		//enable audioListner for playerCar
		playerCar.GetComponent<AudioListener> ().enabled = true;

		//Set Camera target
		CameraSwitcher.instance.SetTarget (playerCar);
		playerCar.gameObject.name = "The Player";
		playerCar.gameObject.tag = "Player";
		//Set player position in UI
		UIManager.instance.playerPosText.text = (playerStartPosition + 1) + "/" + (aiNumberToSpawn + 1);

		//playerCar.transform.position = startPositions [playerStartPosition].position;
		//playerCar.theRB.transform.position = startPositions [playerStartPosition].position;

		/////////////////////Rubber Banding Experimantal Code//////////////////////////////////////////
		//add half the difference between the players max speed and the aiDefaultSpeed
		if (playerDefaultSpeed != playerCar.maxSpeed) {
			//Set the playerDefault Speed
			playerDefaultSpeed = playerCar.maxSpeed;
		}

		//if (aiDefaultSpeed <= playerCar.maxSpeed) {
		//	//Set the aiDefaultSpeed
		//	aiDefaultSpeed += Mathf.Abs (playerCar.maxSpeed - aiDefaultSpeed) * .5f;
		//}
		//////////////////////////////////////END CODE/////////////////////////////////////////////////

		for (int i = 0; i < aiNumberToSpawn + 1; i++) {
			if (i != playerStartPosition) {
				int selectedCar = Random.Range (0, carToSpawn.Count);
				allAICars.Add (Instantiate (carToSpawn [selectedCar], startPositions [i].position, startPositions [i].rotation));

				if (carToSpawn.Count > aiNumberToSpawn - i) {
					carToSpawn.RemoveAt (selectedCar);
				}
			}
		}
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

				if (allAICars.Count > 0) {

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
	public void FinishRace ()
	{
		raceCompleted = true;
		switch (playerPos) {

		case 1:
			UIManager.instance.raceResultText.text = "1st";
			UIManager.instance.trophyImage.gameObject.SetActive (true);
			break;

		case 2:
			UIManager.instance.raceResultText.text = "2nd";
			UIManager.instance.trophyImage.gameObject.SetActive (true);
			break;

		case 3:
			UIManager.instance.raceResultText.text = "3rd";
			UIManager.instance.trophyImage.gameObject.SetActive (true);
			break;

		default:
			UIManager.instance.raceResultText.text = playerPos + "th";

			break;
		}

		if (UIManager.instance.raceResultsPanel != null) {

			var ts = System.TimeSpan.FromSeconds (playerCar.bestLapTime);
			UIManager.instance.raceEndBestLapText.text = string.Format ("{0:00}m{1:00}.{2:000}s", ts.Minutes, ts.Seconds, ts.Milliseconds);
			UIManager.instance.raceResultsPanel.SetActive (true);

		}
	}

	public void ExitRace (string scene)
	{
		Time.timeScale = 1f;
		SceneManager.LoadScene (scene);
	}
}



