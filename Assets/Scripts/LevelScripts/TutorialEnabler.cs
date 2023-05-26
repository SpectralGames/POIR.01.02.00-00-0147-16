using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialEnabler : MonoBehaviour 
{
	public GameObject upgradesTutorialObject, gameplayTutorialObject;

	public void CheckIfCriteriaAreMet(int nCurrentLevel, int nCurrentWave, EGameState currentGameState)
	{
		if(currentGameState == EGameState.Upgrades)
		{
			if(nCurrentLevel == 0 && nCurrentWave == 1) //odpal pierwszy tutorial upgradow
			{
				#if UNITY_EDITOR
				if(upgradesTutorialObject.GetComponent<LinearTutorialController>().DEBUG_MODE == false)
					upgradesTutorialObject.SetActive(true);
				#else
				upgradesTutorialObject.SetActive(true);
				#endif
			}
		}else if(currentGameState == EGameState.Gameplay)
		{
			if(nCurrentLevel == 0 && nCurrentWave == 0) //odpal pierwszy tutorial gameplaya
			{
				#if UNITY_EDITOR
				if(gameplayTutorialObject.GetComponent<LinearTutorialController>().DEBUG_MODE == false)
					gameplayTutorialObject.SetActive(true);
				#else
				gameplayTutorialObject.SetActive(true);
				#endif
			}
		}
	}
}
