using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelCompleteController : MonoBehaviour 
{
	public GameObject stageBonusContainer;
	public Text stageGoldTf;
	public Text stageExpTf;

	public Text virginSafeTf;
	public Text virginGoldTf;
	public Text virginExpTf;

	public Text enemyKiledTf;
	public Text enemyGoldTf;
	public Text enemyExpTf;

	public Text totalExpTf;
	public Text totalGoldTf;
	public Text LevelNrTf;
//	public Text expTf;

//	public Image progressBar;

	public GameObject star1;
	public GameObject star2;
	public GameObject star3;

	public MainMenuPlayerStats playerStatsController;

	private UITranslationAnimator3d starTranslationAnim1;
	private UITranslationAnimator3d starTranslationAnim2;
	private UITranslationAnimator3d starTranslationAnim3;


	public void SetData(int enemiesExpBonus, int enemiesKilled, long enemiesGoldValue, int virginsLeft)
	{
		int totalGoldBonus = 0;
		int totalExpBonus = 0;

		int selectedStage = GlobalGameSettings.SelectedStageNumber;
		if (selectedStage == -1)
			selectedStage = 1;

		LevelNrTf.text = selectedStage.ToString ();
		string levelId = GlobalGameSettings.SelectedLandNumber + "_" + GlobalGameSettings.SelectedStageNumber;

		// bonus za ukonczenie levelu ----------------------------------------------------------------------------
		if (SaveGameController.instance.GetGameLevelStars (levelId) < 1) {
			int bonusGold = XMLGameDataReader.GetGoldForLevelCompleteValue () * selectedStage;
			int bonusExp = XMLGameDataReader.GetExpForLevelCompleteValue () * selectedStage;

			totalGoldBonus += bonusGold;
			totalExpBonus += bonusExp;

			stageExpTf.text = bonusExp.ToString () + " XP";
			stageGoldTf.text = bonusGold.ToString ();

			stageBonusContainer.SetActive (true);
		} else {
			stageBonusContainer.SetActive (false);
		}
			
		// bonus za dziewice -----------------------------------------------------------------------
		int bonusGoldForVirgins = XMLGameDataReader.GetGoldForVirginValue ()  * virginsLeft;
		int bonusExpForVirgins = XMLGameDataReader.GetExpForVirginValue () *  virginsLeft;

		virginSafeTf.text = virginsLeft + "/3";
		virginExpTf.text = bonusExpForVirgins + " XP";
		virginGoldTf.text = bonusGoldForVirgins.ToString();

		totalGoldBonus += bonusGoldForVirgins;
		totalExpBonus += bonusExpForVirgins;

		// bonus za enemies -------------------------------------------------------------------------
		enemyKiledTf.text = enemiesKilled.ToString ();
		enemyExpTf.text = enemiesExpBonus + " XP";
		enemyGoldTf.text = enemiesGoldValue.ToString();

		totalExpBonus += enemiesExpBonus;
		totalGoldBonus += (int) enemiesGoldValue;

	
		// total bonus ---------------------------------------------------------------------
		totalGoldTf.text = totalGoldBonus.ToString ();
		totalExpTf.text = totalExpBonus.ToString ()+ " XP";

		SaveGameController.instance.AddGold (totalGoldBonus);
		SaveGameController.instance.SetGameLevelStars (levelId, virginsLeft);
		//SaveGameController.instance.AddExp (totalExpBonus);

		// dodaj przelicz i wyświetl statystyki gracza
		playerStatsController.SetData (totalExpBonus);
		if (GlobalGameSettings.SelectedLandNumber > 0) 
		{
			if (virginsLeft > 0) {
				if (GlobalGameSettings.SelectedStageNumber < XMLGameDataReader.GetWorldByID (GlobalGameSettings.SelectedLandNumber).levels) {// jesli jest kolejny level do odblokowania
					SaveGameController.instance.SetGameLevelStars (GlobalGameSettings.SelectedLandNumber + "_" + (GlobalGameSettings.SelectedStageNumber + 1), 0);
				}
            
				if (XMLGameDataReader.GetWorlds ().Count > GlobalGameSettings.SelectedLandNumber) { // sprawdż czy jest kolejny świat do odblokowania
                    // calc all stars for levels in world
                    int starsInWorld = 0;
                    for (int i = 1; i <= XMLGameDataReader.GetWorldByID(GlobalGameSettings.SelectedLandNumber).levels; i++)
                    {
                        string id = GlobalGameSettings.SelectedLandNumber + "_" + i;
                        int stars = SaveGameController.instance.GetGameLevelStars(id);
                        if(stars>0)
                             starsInWorld += SaveGameController.instance.GetGameLevelStars(id);
                    }
                 
                    int newLevel = GlobalGameSettings.SelectedLandNumber + 1;
                    //check if wasnt unlocked and enough stars to unlock new world
                    if (SaveGameController.instance.GetWorldStatusComplete(newLevel) == false && starsInWorld >= XMLGameDataReader.GetWorldByID(GlobalGameSettings.SelectedLandNumber).startToCompleteWorld)
                    {
                        SaveGameController.instance.SetWorldStatusComplete(newLevel, true);
                        SaveGameController.instance.SetGameLevelStars(newLevel + "_1", 0);
                    }
                    /*
                    if (newLevel == 2)
                        SteamStatsAndAchievements.Instance.UnlockAchievement(SteamStatsAndAchievements.ACHIEVEMENT_UNLOCK_LEVEL_2);
                    if (newLevel == 3)
                        SteamStatsAndAchievements.Instance.UnlockAchievement(SteamStatsAndAchievements.ACHIEVEMENT_UNLOCK_LEVEL_3);*/
                    
                }
				
			}
		}
			
		SaveGameController.instance.SaveGameData ();

		starTranslationAnim1 = star1.GetComponent<UITranslationAnimator3d> ();
		starTranslationAnim2 = star2.GetComponent<UITranslationAnimator3d> ();
		starTranslationAnim3 = star3.GetComponent<UITranslationAnimator3d> ();

		if (virginsLeft == 3) {
			starTranslationAnim1.StartAnimation ();
			starTranslationAnim2.StartAnimation ();
			starTranslationAnim3.StartAnimation ();
		}
		else if (virginsLeft == 2) {
			starTranslationAnim1.StartAnimation ();
			starTranslationAnim2.StartAnimation ();
			star3.SetActive (false);
		}
		else if (virginsLeft == 1) {
			starTranslationAnim1.StartAnimation ();
			star2.SetActive (false);
			star3.SetActive (false);
		} 
		else if(virginsLeft == 0){
			star1.SetActive (false);
			star2.SetActive (false);
			star3.SetActive (false);
		}
	}
}
