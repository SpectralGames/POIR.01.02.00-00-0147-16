using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class StageSelectionMenu : MonoBehaviour {

	public GameObject buttonPrefab;
	public Transform buttonsParent;

	private UnityIntEvent buttonClickEvent;

	private int selectedWorld;

	// Use this for initialization
	void Start () {
		
	}
	// Update is called once per frame
	void Update () {
		
	}

	public void CreateMenu(int worldId)
	{
		foreach (Transform child in buttonsParent)
			Destroy (child.gameObject);

		selectedWorld = worldId;
		buttonClickEvent = new UnityIntEvent ();
		buttonClickEvent.AddListener (OnButtonClick);

		int MAX_COLUMNS = 5;
		float START_POS_X = -.4f;
		float MARGIN_X = .2f;
		float MAGRIN_Y = .15f;

		float posX = START_POS_X;
		float posY = .63f;

		int levelsInWorld = XMLGameDataReader.GetWorldByID (worldId).levels;


		for(int i = 0; i < levelsInWorld;i++)
		{
			if (i % MAX_COLUMNS == 0 && i != 0) {
				posX = START_POS_X;
				posY -= MAGRIN_Y;
			}

			GameObject button = Instantiate (buttonPrefab, buttonsParent, false);
			button.name = "ButtonStage" + i;
			button.transform.localPosition = new Vector3 (posX, posY, 0);
			button.transform.Find ("ButtonBody/Text").GetComponent<Text> ().text = (i + 1).ToString();

			button.GetComponent<Button3d> ().SetClickEvent (buttonClickEvent, i + 1);

			string levelId = worldId + "_" + (i + 1);
			SetStarsForLevelButton (button, levelId);

			posX += MARGIN_X;
		}
	}

	private void OnButtonClick(int stageId)
	{
		GlobalGameSettings.SelectedLandNumber = selectedWorld;
		GlobalGameSettings.SelectedStageNumber = stageId;
        ObjectPool.Instance.ClearData();
		GlobalHelpers.LoadLevel(GlobalGameSettings.SelectedLandNumber, "Loading...");
	}

	private void SetStarsForLevelButton(GameObject button, string levelId)
	{
		int levelStars = SaveGameController.instance.GetGameLevelStars (levelId);
		Transform starsContainer = button.transform.Find ("Stars");
		if (levelStars == 3) {
			starsContainer.Find ("star1").gameObject.SetActive (true);
			starsContainer.Find ("star2").gameObject.SetActive (true);
			starsContainer.Find ("star3").gameObject.SetActive (true);

		} else if (levelStars == 2) {
			starsContainer.Find ("star1").gameObject.SetActive (true);
			starsContainer.Find ("star2").gameObject.SetActive (true);
			starsContainer.Find ("star3").gameObject.SetActive (false);
		} else if (levelStars == 1) {
			starsContainer.Find ("star1").gameObject.SetActive (true);
			starsContainer.Find ("star2").gameObject.SetActive (false);
			starsContainer.Find ("star3").gameObject.SetActive (false);
		} else {
			starsContainer.Find ("star1").gameObject.SetActive (false);
			starsContainer.Find ("star2").gameObject.SetActive (false);
			starsContainer.Find ("star3").gameObject.SetActive (false);
		}
			
		if (levelStars >= 0)
			button.transform.Find ("lock").gameObject.SetActive (false);
		else {
			button.transform.Find ("lock").gameObject.SetActive (true);
			button.GetComponent<Button3d> ().EnableButton (false);
		}
	}
}
