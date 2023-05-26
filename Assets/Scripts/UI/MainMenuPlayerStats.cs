using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuPlayerStats : MonoBehaviour {

    public Text tfUserName;
	public Text tfExp;
	public Text tfGold;
	public Text tfLevel;
	public Image expBar;
	public GameObject nextLevelParticles;

	public UIImageFillAmountAnimator imageFillAnimator;
	public UIAnimatorSequence progressBarSequenceAnimator;

	private float progressBarValueAfterLevelUp;

	/*
    public void Start()
    {
	    /*
        if (SteamManager.Initialized)
        {
            if(tfUserName != null)
                tfUserName.text = SteamStatsAndAchievements.Instance.GetNickname();
		
          

          //  SteamStatsAndAchievements.Instance.UnlockAchievement(SteamStatsAndAchievements.ACHIEVEMENT_1);
        }

    }
    */
	    
    public void UpdateUI()
	{
		int playerLevel = SaveGameController.instance.GetPlayerLevel ();
		tfLevel.text = playerLevel.ToString();

		tfExp.text = SaveGameController.instance.GetExp ().ToString () + "/" + XMLGameDataReader.GetNextLevelExpPoints(playerLevel);
		tfGold.text = SaveGameController.instance.GetGold ().ToString ();

		int nextLevelPoints = XMLGameDataReader.GetNextLevelExpPoints (playerLevel);
		int prevLevelPoints = XMLGameDataReader.GetNextLevelExpPoints (playerLevel - 1);

	//	Debug.Log("exp: "+ SaveGameController.instance.GetExp () + " "+ XMLGameDataReader.GetNextLevelExpPoints (playerLevel) + " " + (SaveGameController.instance.GetExp () / XMLGameDataReader.GetNextLevelExpPoints (playerLevel)));
		expBar.fillAmount = 1f * (SaveGameController.instance.GetExp () - prevLevelPoints)  / (nextLevelPoints - prevLevelPoints);
	}

	public void SetData(long bonusExp)
	{
		UpdateUI ();

		int playerLevel = SaveGameController.instance.GetPlayerLevel ();

		int nextLevelPoints = XMLGameDataReader.GetNextLevelExpPoints (playerLevel);
		int prevLevelPoints = XMLGameDataReader.GetNextLevelExpPoints (playerLevel - 1);
	

		float currentProgressValue = 1f * ( (SaveGameController.instance.GetExp()))  / (nextLevelPoints - prevLevelPoints);
		if (currentProgressValue < 0) // pierwszy level
			currentProgressValue = 0;
		
		float afterBonusPointsProgressValue = 1f * (SaveGameController.instance.GetExp() + bonusExp - prevLevelPoints)  / (nextLevelPoints - prevLevelPoints);


		imageFillAnimator.valueFrom = currentProgressValue;
		imageFillAnimator.valueTo = afterBonusPointsProgressValue;
		imageFillAnimator.StartAnimation ();

		float animationTime = 2 * Mathf.Clamp01 (afterBonusPointsProgressValue);
		imageFillAnimator.timeInSeconds = animationTime;
		progressBarValueAfterLevelUp = afterBonusPointsProgressValue - 1;
		if (afterBonusPointsProgressValue < 1) 
		{
			imageFillAnimator.StartAnimation ();
		} 
		else 
		{
			imageFillAnimator.valueTo = 1;
			progressBarSequenceAnimator.StartSequence ();
			progressBarSequenceAnimator.SetCompleteAnimationEvent (gameObject, "OnProgressBarAnimationComplete");
		}
			
		Invoke ("UpdateUIAfterFirstAnimationFinish", animationTime);

		SaveGameController.instance.AddExp (bonusExp);
        //SteamLeaderboard.Instance.SendScore((int)SaveGameController.instance.GetExp());

    }

	private void UpdateUIAfterFirstAnimationFinish()
	{

		nextLevelParticles.SetActive (true);
		int playerLevelAfterBonusExp = SaveGameController.instance.GetPlayerLevel ();
		tfExp.text = (SaveGameController.instance.GetExp()).ToString ()  + "/" + XMLGameDataReader.GetNextLevelExpPoints(playerLevelAfterBonusExp);
		tfLevel.text = playerLevelAfterBonusExp.ToString();
	}

	private void OnProgressBarAnimationComplete()
	{
		imageFillAnimator.valueFrom = 0;
		imageFillAnimator.valueTo = progressBarValueAfterLevelUp;
		imageFillAnimator.StartAnimation ();
	}


}
