using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

	public CarController target;
	public float minDistance, maxDistance;

	public Transform startPositionOffset;

	private float activeDistance;
	private Vector3 offsetDir;

	// Start is called before the first frame update
	void Start ()
	{
		offsetDir = transform.position - startPositionOffset.position;
		activeDistance = minDistance;

		offsetDir.Normalize ();
	}

	// Update is called once per frame
	void Update ()
	{

		activeDistance = minDistance + ((maxDistance - minDistance) * (target.theRB.velocity.magnitude * .5f / target.maxSpeed));
		transform.position = target.transform.position + (offsetDir * activeDistance);
	}
}
