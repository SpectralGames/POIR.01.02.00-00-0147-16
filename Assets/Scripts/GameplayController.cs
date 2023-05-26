using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum EGameState
{
	Upgrades, Gameplay
}

public class GameplayController : MonoBehaviour 
{
	public FloatingMenuCanvas floatingMenu;
	
	private PlayerController playerController;
	private LevelController levelController;
	private GameObject worldCanvas;

	private bool isPaused, isGameplayActive;
	private long enemiesGoldBonus = 0;
	private int enemiesExpBonus = 0;
	private int enemiesKilled = 0;
	// Use this for initialization
	void Awake () 
	{
		isPaused = isGameplayActive = false;
		floatingMenu.OnSetGameplayController (this);
		playerController = GameObject.FindObjectOfType<PlayerController>();
		playerController.OnPlayerDied += OnPlayerDied;

		levelController = GameObject.FindObjectOfType<LevelController>();
		levelController.OnSetGameplayController (this);

		worldCanvas = GameObject.Find ("WorldCanvas");
        ObjectPool.Instance.Init();

        GameObject loadingCamera = GameObject.FindGameObjectWithTag("LoadingCamera");
        if (loadingCamera != null) Destroy(loadingCamera);

    }

	void Start()
	{
		isGameplayActive = true;
		levelController.OnStartLevelDelayed(2f);
		playerController.OnTeleportPlayerToTeleportNumber(levelController.GetCurrentLevelInitTowerNumber());
	}

		
	// Update is called once per frame
	void Update () 
	{
		#if UNITY_EDITOR
		if (Input.GetKeyDown (KeyCode.F)) {
			Time.timeScale = 50f;
		} else if (Input.GetKeyUp(KeyCode.F)) {
			if(isPaused == false)
				Time.timeScale = 1f;
		}
		else if (Input.GetKeyUp(KeyCode.R)) {
			RestartGame();
		}
		else if (Input.GetKeyUp(KeyCode.Escape)) {

			isGameplayActive = !floatingMenu.TogglePauseMenu();
		}
		#endif

		if(isGameplayActive)
		{
			if(levelController.isEveryEnemyDead)
				this.OnLevelFinished();
		}

		worldCanvas.SetActive (isGameplayActive);
	}
		
		
	private void OnPlayerDied()
	{
		if(isGameplayActive == false) return;
		isGameplayActive = false;
	
		floatingMenu.ShowGameOverMenu(enemiesExpBonus, enemiesGoldBonus, enemiesKilled);
	}

	private void OnLevelFinished()
	{
		if(isGameplayActive == false) return;
		isGameplayActive = false;
		floatingMenu.ShowLevelCompleteMenu(enemiesExpBonus, enemiesKilled, enemiesGoldBonus, ObjectPool.Instance.GetAliveVirginCount());
	}

	public void RestartGame()
	{
		enemiesGoldBonus = 0;
		enemiesExpBonus = 0;
		enemiesKilled = 0;
        ObjectPool.Instance.ClearData();
        //SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().name);
        GlobalHelpers.LoadLevel(GlobalGameSettings.SelectedLandNumber, "Loading...");

    }

	public void OnEnemyDie(Enemy enemy)
	{
		enemiesGoldBonus += enemy.GetKillGoldBonus();
		enemiesExpBonus += enemy.GetKillExpBonus();
		enemiesKilled++;
	}

	public void OnVirginLost()
	{
		if (ObjectPool.Instance.GetAliveVirginCount() == 0) 
		{
			if(isGameplayActive == false) return;
			isGameplayActive = false;

			floatingMenu.ShowGameOverMenu(enemiesExpBonus, enemiesGoldBonus, enemiesKilled);
		}
	}

	public bool IsGameplayActive()
	{
		return isGameplayActive;
	}

	public void OnGameResume()
	{
		isGameplayActive = true;
	}
}
