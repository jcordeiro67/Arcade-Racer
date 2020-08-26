using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPointChecker : MonoBehaviour {

	public CarController theCar;
	private int checkPoint;



	private void OnTriggerEnter (Collider other)
	{
		if (other.GetComponent<CheckPoint> () != null) {

			checkPoint = other.GetComponent<CheckPoint> ().checkPointNumber;
			theCar.CheckPointHit (checkPoint);

		}


	}

}
