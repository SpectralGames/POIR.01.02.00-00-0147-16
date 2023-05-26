using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CalculationHelper {

	public static Transform FindNearestObject(IEnumerable<AI> objectsList, Vector3 positionToCheck )
	{
		float length = 50f;
		//int index = -1;
		Transform trans = null;
		foreach (AI objectToCheck in objectsList) 
		{
			float tempLength = Vector3.Distance (objectToCheck.transform.position, positionToCheck);

			if (tempLength < length) {
				//Vector3 direction = positionToCheck - objectToCheck.transform.position;
				length = tempLength;
				trans = objectToCheck.transform;
			}
		}			
		return trans;
		
	}
}
