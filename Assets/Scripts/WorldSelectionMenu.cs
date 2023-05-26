using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WorldSelectionMenu : MonoBehaviour {

	public List<WorldSelectionButton> worldButtons;


	// Use this for initialization
	void Start () 
	{
		foreach(WorldSelectionButton worldButton in worldButtons)
		{
			WorldItem worldData = XMLGameDataReader.GetWorldByID (worldButton.id);
			int stars = 0;
			for (int i = 0; i < worldData.levels; i++) {

				int levelStars = SaveGameController.instance.GetGameLevelStars (worldData.id + "_" + (i + 1)); // dodaj nowy id levelu jesli nie istniał, jeśli istnieje zwróć ilość gwiazdek// e.g 1_3 , 1_35, 2_4
				if (levelStars > 0)
					stars += levelStars;
			}

			worldButton.button.transform.Find ("ButtonBody").Find ("Text").GetComponent<Text> ().text = worldData.name;
			worldButton.button.transform.Find ("ButtonBody").Find ("TextStars").GetComponent<Text> ().text = stars +"/"+worldData.levels*3;
			if (SaveGameController.instance.GetWorldStatusComplete (worldData.id)) {
				worldButton.button.transform.Find ("ButtonBody").Find ("lock").gameObject.SetActive (false);
			} else {
				worldButton.button.GetComponent<Button3d> ().EnableButton (false);
			}

		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}

}
[System.Serializable]
public class WorldSelectionButton
{
	public GameObject button;
	public int id;
}
