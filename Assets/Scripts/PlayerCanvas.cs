using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCanvas : MonoBehaviour {

	public DamageIndicator damageDirectionIndicator;
	// Use this for initialization
	void Start () {
	}

	public void ShowDamageAnimation(GameObject enemy)
	{
		damageDirectionIndicator.ShowPointer (enemy.GetComponent<Enemy>().hitPivot);
	}
		
}
