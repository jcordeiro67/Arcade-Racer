using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour {

	public Rigidbody theRB;
	[Header ("Car Setup")]
	public bool isAI;
	public float maxSpeed;
	public float forwardAccel = 8f;
	public float reverseAccel = 4f;
	public float turnStrength = 180f;
	public float maxWheelTurn = 10f;
	public AudioSource engineSound;
	public AudioSource skidSound;
	public float skidFadeSpeed;
	public int currentLap = 1;
	public int nextCheckPoint;
	public float resetCoolDown = 2f;
	private float resetCounter;

	[Header ("Car Parts")]
	public Transform leftFrontWheel;
	public Transform rightFrontWheel;
	public Transform groundRayPointFront;
	public Transform groundRayPointRear;

	[Header ("Physics Settings")]
	public LayerMask whatIsGround;
	public float groundRayLength = 0.75f;
	public float gravityMod = 10f;
	public float gravityModMultiple = 500f;

	[Header ("Dust Trails")]
	public ParticleSystem [] dustVFX;
	public float maxEmmission = 25f;
	public float emmissionFadeSpeed = 20f;
	public float turningThreshold = 0.5f;
	public float speedThreshold = 0.5f;

	private float emmissionRate;
	private float speedInput;
	private float turnInput;
	private bool grounded;
	private float dragOnGround;



	public float lapTime;
	public float bestLapTime;

	[Header ("AI Settings")]
	public int currentTarget;
	private Vector3 targetPoint;
	public float aiAccelerationSpeed = 1;
	public float aiTurnSpeed = 0.8f;
	public float aiReachPointRange = 5f;
	public float aiPointVariance = 3f;
	public float aiMaxTurn = 15f;
	private float aiSpeedInput;
	private float aiSpeedMod;

	// Start is called before the first frame update
	void Start ()
	{
		theRB.transform.parent = null;
		dragOnGround = theRB.drag;
		//emmissionRate = 0f;

		if (isAI) {
			targetPoint = RaceManager.instance.allCheckPoints [currentTarget].transform.position;
			RandomizeAITarget ();

			aiSpeedMod = Random.Range (0.7f, 1.1f);
		}

		if (!isAI) {
			//Reset Lap Counter Display
			UIManager.instance.lapCounterText.text = currentLap + "/" + RaceManager.instance.totalLaps;
		}

		resetCounter = resetCoolDown;
	}

	// Update is called once per frame
	void Update ()
	{
		//Start race when isStarting countDown reaches 0
		if (!RaceManager.instance.isStarting) {

			//Lap Counter
			lapTime += Time.deltaTime;

			if (!isAI) {
				//Update Lap Time UI
				var ts = System.TimeSpan.FromSeconds (lapTime);
				UIManager.instance.currentLapTimeText.text = string.Format ("{0:00}m{1:00}.{2:000}s", ts.Minutes, ts.Seconds, ts.Milliseconds);


				//DRIVE INPUT
				speedInput = 0f;
				if (Input.GetAxis ("Vertical") > 0) {
					speedInput = Input.GetAxis ("Vertical") * forwardAccel;
				} else if (Input.GetAxis ("Vertical") < 0) {
					speedInput = Input.GetAxis ("Vertical") * reverseAccel;
				}

				//TURNING CAR
				turnInput = Input.GetAxis ("Horizontal");

				//RESET CAR TO TRACK
				if (resetCounter > 0) {

					resetCounter -= Time.deltaTime;
				}

				if (Input.GetKeyDown (KeyCode.R) && resetCoolDown <= 0) {
					ResetToTrack ();
				}


			} else {
				//Set targetPoint y to match car's y
				targetPoint.y = transform.position.y;

				if (Vector3.Distance (transform.position, targetPoint) < aiReachPointRange) {
					SetNextAITarget ();
				}

				//Ai Turning
				Vector3 targetDir = targetPoint - transform.position;
				float angle = Vector3.Angle (targetDir, transform.forward);

				Vector3 localPosition = transform.InverseTransformPoint (targetPoint);
				if (localPosition.x < 0f) {
					angle = -angle;
				}

				turnInput = Mathf.Clamp (angle / aiMaxTurn, -1f, 1f);

				//AI Speed Input
				if (Mathf.Abs (angle) < aiMaxTurn) {
					aiSpeedInput = Mathf.MoveTowards (aiSpeedInput, 1f, aiAccelerationSpeed);
				} else {
					aiSpeedInput = Mathf.MoveTowards (aiSpeedInput, aiTurnSpeed, aiAccelerationSpeed);
				}
				//AI Randomize speed each lap
				aiSpeedMod = Random.Range (0.7f, 1.1f);
				speedInput = aiSpeedInput * forwardAccel * aiSpeedMod;
			}

			//TURNING WHEELS
			leftFrontWheel.localRotation = Quaternion.Euler (leftFrontWheel.localRotation.eulerAngles.x, (turnInput * maxWheelTurn) - 180, leftFrontWheel.localRotation.eulerAngles.z);
			rightFrontWheel.localRotation = Quaternion.Euler (rightFrontWheel.localRotation.eulerAngles.x, turnInput * maxWheelTurn, rightFrontWheel.localRotation.eulerAngles.z);

			//Set Cars Position to the RigidBody
			//transform.position = theRB.transform.position;

			//Dust Trail Emission
			emmissionRate = Mathf.MoveTowards (emmissionRate, 0f, emmissionFadeSpeed * Time.deltaTime);

			//Emmit if turning or spped is lees then 1/2 masSpeed and not 0
			if (grounded && (Mathf.Abs (turnInput) > turningThreshold || (theRB.velocity.magnitude < maxSpeed * speedThreshold && theRB.velocity.magnitude != 0))) {
				emmissionRate = maxEmmission;
			}

			if (theRB.velocity.magnitude <= .5f) {
				emmissionRate = 0;
			}

			//Fade Dust Trails
			for (int i = 0; i < dustVFX.Length; i++) {
				var emissionModule = dustVFX [i].emission;
				emissionModule.rateOverTime = emmissionRate;

			}

			//Engine Sound
			if (engineSound != null) {
				engineSound.pitch = 1f + (theRB.velocity.magnitude / maxSpeed) * 2f;
			}

			//Skid Sound
			if (skidSound != null) {
				if (Mathf.Abs (turnInput) > 0.5f) {
					skidSound.volume = 1;

				} else {
					skidSound.volume = Mathf.MoveTowards (skidSound.volume, 0f, skidFadeSpeed * Time.deltaTime);
				}
			}
		}
	}

	void FixedUpdate ()
	{
		grounded = false;
		RaycastHit hit;
		Vector3 normalTarget = Vector3.zero;

		if (Physics.Raycast (groundRayPointFront.position, -transform.up, out hit, groundRayLength, whatIsGround)) {
			grounded = true;
			//Finds surface normal
			normalTarget = hit.normal;
		}
		//RayCast to determing if grounded
		if (Physics.Raycast (groundRayPointRear.position, -transform.up, out hit, groundRayLength, whatIsGround)) {
			grounded = true;
			normalTarget = (normalTarget + hit.normal) / 2f;
		}
		//When On Ground rotate car to surface normal
		if (grounded) {
			transform.rotation = Quaternion.FromToRotation (transform.up, normalTarget) * transform.rotation;
		}

		//Acceleratiom
		if (grounded) {

			theRB.drag = dragOnGround;
			theRB.AddForce (transform.forward * speedInput * 1000f);

		} else {
			emmissionRate = 0f;
			theRB.drag = 0.1f;
			theRB.AddForce (-Vector3.up * gravityMod * 500f);
		}

		//Normalize Acceleration Set MaxSpeed
		if (theRB.velocity.magnitude > maxSpeed) {
			theRB.velocity = theRB.velocity.normalized * maxSpeed;
		}

		transform.position = theRB.transform.position;

		if (grounded && theRB.velocity.magnitude > 0.02f) {
			transform.rotation = Quaternion.Euler (transform.rotation.eulerAngles +
				new Vector3 (0f, turnInput * turnStrength * Time.deltaTime * Mathf.Sign (speedInput) * (theRB.velocity.magnitude / maxSpeed), 0f));

		}
		//Debug.Log (theRB.velocity.magnitude);
	}


	public void CheckPointHit (int checkPoint)
	{
		if (checkPoint == nextCheckPoint) {
			nextCheckPoint++;

			if (nextCheckPoint == RaceManager.instance.allCheckPoints.Length) {
				nextCheckPoint = 0;
				LapCompleted ();
			}
		}

		if (isAI) {
			if (checkPoint == currentTarget) {
				SetNextAITarget ();
			}
		}
	}

	public void SetNextAITarget ()
	{
		currentTarget++;
		if (currentTarget >= RaceManager.instance.allCheckPoints.Length) {
			currentTarget = 0;

		}

		targetPoint = RaceManager.instance.allCheckPoints [currentTarget].transform.position;
		RandomizeAITarget ();
	}

	public void LapCompleted ()
	{
		//Add a Lap
		currentLap++;
		//Best lap time check
		if (lapTime < bestLapTime || bestLapTime == 0) {
			bestLapTime = lapTime;
		}

		lapTime = 0f;

		if (!isAI) {
			//Set bestLapTime UI Text
			var ts = System.TimeSpan.FromSeconds (bestLapTime);
			UIManager.instance.bestLapTimeText.text = string.Format ("{0:00}m{1:00}.{2:000}s", ts.Minutes, ts.Seconds, ts.Milliseconds);

			UIManager.instance.lapCounterText.text = currentLap + "/" + RaceManager.instance.totalLaps;
		}
	}

	public void RandomizeAITarget ()
	{
		targetPoint += new Vector3 (Random.Range (-aiPointVariance, aiPointVariance), 0f, Random.Range (-aiPointVariance, aiPointVariance));
	}

	void ResetToTrack ()
	{
		int pointToGoTo = nextCheckPoint - 1;
		if (pointToGoTo < 0) {
			pointToGoTo = RaceManager.instance.allCheckPoints.Length - 1;
		}
		//reset the car and the rigidboxy position to the lastcheck point
		speedInput = 0f;
		turnInput = 0f;
		transform.position = RaceManager.instance.allCheckPoints [pointToGoTo].transform.position;
		theRB.transform.position = transform.position;
		theRB.velocity = Vector3.zero;

		//reset the car direction to look at next checkpoint
		transform.LookAt (RaceManager.instance.allCheckPoints [nextCheckPoint].transform.position);

		resetCounter = resetCoolDown;
	}

}
