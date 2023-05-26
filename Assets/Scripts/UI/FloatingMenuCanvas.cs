using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FloatingMenuCanvas : MonoBehaviour 
{
	public GameObject pauseMenu;
	public GameObject gameoverMenu;
	public LevelCompleteController levelCompleteMenu;
	public GameObject playerStatsPanel;
	public GameObject mainMenu;
	public Quaternion playerRotation;
	private GameplayController gameplayController;
	private Transform pivotTransform;
	private bool isActive = false;

	
	// Use this for initialization

	void Start () {
		pivotTransform = GameObject.FindGameObjectWithTag ("FloatingMenuPivot").transform;
	}
	
	// Update is called once per frame
	void LateUpdate () 
	{
		//this.SnapMenuCanvasToPivot();
	}

	public void OnSetGameplayController(GameplayController gameplayController)
	{
		this.gameplayController = gameplayController;
	}

	public bool TogglePauseMenu()
	{
		bool isVisible = false;
		if (gameoverMenu.activeSelf == false && levelCompleteMenu.gameObject.activeSelf == false) {
			if (pauseMenu.activeSelf) {
				isVisible = false;
				isActive = false;
				pauseMenu.SetActive (false);
				playerStatsPanel.gameObject.SetActive (false);
				this.SetTimeScale(1f);
			} else {
				isVisible = true;
				this.ActivateMenuAndSnapToPivot();
				playerStatsPanel.GetComponentInChildren<MainMenuPlayerStats> ().UpdateUI ();
				pauseMenu.SetActive (true);
				this.SetTimeScale(0.01f);
			}
		}
		return isVisible;
	}

	private void SnapMenuCanvasToPivot()
	{
		if (isActive) 
		{
			Vector3 pivotNormal = new Vector3(pivotTransform.forward.x, 0f, pivotTransform.forward.z).normalized;
			this.transform.position = Camera.main.transform.position + pivotNormal*0.6f;
			this.transform.LookAt (new Vector3 (Camera.main.transform.position.x, this.transform.position.y, Camera.main.transform.position.z));
			this.transform.rotation *= Quaternion.Euler (new Vector3 (0f, 180f, 0f));
		}
	}
	private void SnapMenuCanvasToPivot(Transform gameobject)
	{
		if (isActive) 
		{
			Vector3 pivotNormal = new Vector3(pivotTransform.forward.x, 0f, pivotTransform.forward.z).normalized;
			gameobject.position = Camera.main.transform.position + pivotNormal*0.6f;
			gameobject.LookAt (new Vector3 (Camera.main.transform.position.x, this.transform.position.y, Camera.main.transform.position.z));
			gameobject.rotation *= Quaternion.Euler (new Vector3 (0f, 180f, 0f));
		}
	}
	public void ShowGameOverMenu(int enemiesExpBonus, long enemiesGoldValue, int enemiesKilled)
	{
		this.ActivateMenuAndSnapToPivot();

		gameoverMenu.SetActive (true);
		int selectedStage = GlobalGameSettings.SelectedStageNumber;
		if (selectedStage == -1)
			selectedStage = 1;

		gameoverMenu.transform.Find ("ExpBonusTf").GetComponent<Text> ().text = enemiesExpBonus.ToString () + " XP";
		gameoverMenu.transform.Find ("GoldBonusTf").GetComponent<Text> ().text = enemiesGoldValue.ToString ();
		gameoverMenu.transform.Find ("LevelNrTf").GetComponent<Text> ().text = selectedStage.ToString ();
		gameoverMenu.transform.Find ("EnemyTf").GetComponent<Text> ().text = enemiesKilled.ToString ();
		SaveGameController.instance.AddGold (enemiesGoldValue);
		SaveGameController.instance.SaveGameData ();

		playerStatsPanel.GetComponentInChildren<MainMenuPlayerStats> ().SetData ( enemiesExpBonus);
		//.SetProgressExpBar (bonusExp);
	}

	public void ShowLevelCompleteMenu(int enemiesExpBonus, int enemiesKilled, long enemiesGoldValue, int virginsLeft)
	{
		this.ActivateMenuAndSnapToPivot();
		//SnapMenuCanvasToPivot(mainMenu.transform);
		levelCompleteMenu.gameObject.SetActive (true);
		levelCompleteMenu.SetData(enemiesExpBonus, enemiesKilled, enemiesGoldValue, virginsLeft);
	}

	private void ActivateMenuAndSnapToPivot()
	{
		isActive = true;
		this.SnapMenuCanvasToPivot();
		playerStatsPanel.gameObject.SetActive (true);
	}
		
	public void OnButtonContinue()
	{
		this.SetTimeScale(1f);
		isActive = false;
        //UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName:"BruteVsMaidenVR_MainMenu");
        ObjectPool.Instance.ClearData();
        //SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().name);
        GlobalHelpers.LoadLevel("BruteVsMaidenVR_MainMenu", "Loading...");
    }

	public void OnButtonRestart()
	{
		this.SetTimeScale(1f);
		isActive = false;
		gameplayController.RestartGame ();
	}

	public void OnButtonResume()
	{
		this.SetTimeScale(1f);
		isActive = false;
		pauseMenu.SetActive (false);
		playerStatsPanel.gameObject.SetActive (false);
		gameplayController.OnGameResume ();
	}
		
	private void SetTimeScale (float timeScale)
	{
		Time.timeScale = timeScale;
		Time.maximumDeltaTime = timeScale * 0.333f;
		Time.fixedDeltaTime = timeScale * 0.02f;
	}

}
