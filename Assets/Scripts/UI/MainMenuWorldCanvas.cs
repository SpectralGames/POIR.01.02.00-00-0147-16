using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EMainMenuState
{
	MainLayer,
	PlayLayer,
	StageSelectionLayer,
	SummonsLayer,
	SummonsLayerStats,
	SpellsLayer,
	SpellsLayerStats,
	ItemPivot,
	NONE,
  SettingsLayer,
  SettingsLayerStats,
  Leaderboard
}

[System.Serializable]
public struct MenuLayers
{
	public EMainMenuState menuState;
	public GameObject layerObject;
}

public class MainMenuWorldCanvas : MonoBehaviour 
{
	public MenuLayers[] menuLayers;
	public PortalEnemiesController portalEnamiesController;
	public MainMenuPlayerStats playerStats;
	private EMainMenuState currentMenuState, previousMenuState;
	private UpgradesWorldCanvas upgradesCanvas;
	private int lastLandSelected;
	private Animator mainAnimator;

    private RadialMenuController[] radialMenuControllers;

	// Use this for initialization
	void Awake () 
	{
		currentMenuState = previousMenuState = EMainMenuState.NONE;	
		upgradesCanvas = this.GetComponent<UpgradesWorldCanvas>();
		lastLandSelected = -1;
		TryUnlockUpgradableItems();
		mainAnimator = GetComponent<Animator>();


        //SteamLeaderboard.Instance.SendScore((int)SaveGameController.instance.GetExp());

        GameObject loadingCamera = GameObject.FindGameObjectWithTag("LoadingCamera");
        if (loadingCamera != null) Destroy(loadingCamera);
        playerStats.UpdateUI ();
        ObjectPool.Instance.Init();
	}
    private void Start()
    {
        radialMenuControllers = FindObjectsOfType<RadialMenuController>();

    }

    private void TryUnlockUpgradableItems()
	{
		List<string> upgradableItems = new List<string> ();
		upgradableItems.AddRange (XMLItemsReader.GetAllSpells ());
		upgradableItems.AddRange (XMLItemsReader.GetAllSummons ());

		foreach (string itemCode in upgradableItems)
		{
			if (SaveGameController.instance.GetItemLevel (itemCode) == -1) {
				if (SaveGameController.instance.GetPlayerLevel () >= XMLItemsReader.GetItemUnlockExpLevel (itemCode)) {
					SaveGameController.instance.SetItemLevel (itemCode, 0);
				}
			}
		}
		SaveGameController.instance.SaveGameData ();
	}
	// // // // // // // // // Buttony

	public void OnPlayButtonClicked()
	{
		Debug.Log("PlayButtonClicked");
		this.SwitchMenuState(EMainMenuState.PlayLayer);
		upgradesCanvas.OnCloseUpgrade ();
		CloseStats ();
	}

	public void OnLandButtonClicked(int landNumber)
	{
		//Debug.Log("Land number: " + landNumber);
		if(landNumber == lastLandSelected)
		{
			this.SetMenuState(EMainMenuState.StageSelectionLayer, false);
			lastLandSelected = -1;
		}else{
			//ustaw w pozycji guzika
			GameObject stageSelectionLayer = this.FindLayerObjectOfMenuState(EMainMenuState.StageSelectionLayer);
			GameObject playLayer = this.FindLayerObjectOfMenuState(EMainMenuState.PlayLayer);
			Transform selectedLandButton = playLayer.transform.GetChild(landNumber-1);
		//	stageSelectionLayer.transform.position = selectedLandButton.transform.position;

			this.SetMenuState(EMainMenuState.StageSelectionLayer, true);
			lastLandSelected = landNumber;

			StageSelectionMenu stageSelectionMenu = stageSelectionLayer.GetComponent<StageSelectionMenu> ();
			stageSelectionMenu.CreateMenu (lastLandSelected);
		}
	}
		
	public void OnSummonsButtonClicked()
	{
		upgradesCanvas.OnCloseUpgrade ();
		portalEnamiesController.EnableAttack (true);
		this.SwitchMenuState(EMainMenuState.SummonsLayer);

		CloseStats ();
	}

	public void OnSummonsTypeClicked(string summonType)
	{
		if (this.FindLayerObjectOfMenuState(EMainMenuState.SummonsLayerStats).activeInHierarchy == false) {
			this.SetMenuState(EMainMenuState.SummonsLayerStats,true);
		}
		upgradesCanvas.OnOpenUpgradeLayer (false, summonType);
		MainMenuInfoWindow.Instance.HideInfoWindow();
	}

	public void OnSpellsButtonClicked()
	{
		Debug.Log("Spells clicked");
		upgradesCanvas.OnCloseUpgrade ();
		portalEnamiesController.EnableAttack (true);
		this.SwitchMenuState(EMainMenuState.SpellsLayer);
		CloseStats ();
	}

	public void OnSpellsTypeClicked(string spellType)
	{
		if (this.FindLayerObjectOfMenuState(EMainMenuState.SpellsLayerStats).activeInHierarchy == false) {
			this.SetMenuState(EMainMenuState.SpellsLayerStats,true);
		}
		upgradesCanvas.OnOpenUpgradeLayer (true, spellType);
		MainMenuInfoWindow.Instance.HideInfoWindow();
	}

	public void OnButtonUpgradeClicked()
	{
		string currentItemCodeName = upgradesCanvas.GetCurrentSelectedElementCodeName();
		int currentLevel = SaveGameController.instance.GetItemLevel(currentItemCodeName);
		int price = currentLevel == 0 ? int.Parse(XMLItemsReader.GetItemUnlockPrice(currentItemCodeName)[0]) : int.Parse(XMLItemsReader.GetItemUpgradePrice(currentItemCodeName, currentLevel)[0]);

		if(SaveGameController.instance.GetGold() < price)
		{
			MainMenuInfoWindow.Instance.ShowInfoWindow("Not enough coins!", null, null, true, currentMenuState);
		}else{
			upgradesCanvas.SimulateUpgradeItemStats();
			string descriptionText = "Do you want to " + (currentLevel == 0 ? "unlock" : "upgrade") + " the " + currentItemCodeName + " for " + price.ToString() + " coins ?";
			MainMenuInfoWindow.Instance.ShowInfoWindow(descriptionText, this.OnUpgradeOKButtonClicked, this.OnUpgradeCancelButtonClicked, false, currentMenuState);
		}
	}

	private void OnUpgradeOKButtonClicked()
	{
		upgradesCanvas.OnUpgradeSpellButtonClicked ();
		playerStats.UpdateUI ();
		mainAnimator.Play("MainMenuUpgradeAnim");
        // update spell menu on hand after unlock spell
        foreach(var radialMenu in radialMenuControllers)
        {
            radialMenu.UpdateMenu();
        }
	}
	private void OnUpgradeCancelButtonClicked()
	{
		upgradesCanvas.OnUpdateUpgradeLayer(); //przywroc staty
	}

	public void OnBackButtonClicked()
	{
		Debug.Log("BackButtonClicked");
		this.SwitchMenuState(previousMenuState);
	}

	public void CloseStats()
	{
		if (this.FindLayerObjectOfMenuState (EMainMenuState.SpellsLayerStats).activeInHierarchy == true) {
			this.SetMenuState (EMainMenuState.SpellsLayerStats, false);
		}
		if (this.FindLayerObjectOfMenuState(EMainMenuState.SummonsLayerStats).activeInHierarchy == true) {
			this.SetMenuState(EMainMenuState.SummonsLayerStats,false);
		}
    if (this.FindLayerObjectOfMenuState(EMainMenuState.SettingsLayerStats).activeInHierarchy == true)
    {
      this.SetMenuState(EMainMenuState.SettingsLayerStats, false);
    }
  }

	// // // // // // // // // 

	private void SwitchMenuState(EMainMenuState newState)
	{
		previousMenuState = currentMenuState;
		if(currentMenuState != newState)
			currentMenuState = newState;
		else
			currentMenuState = EMainMenuState.NONE;
		
		StartCoroutine(AnimateStateChange(currentMenuState, previousMenuState));

		if(lastLandSelected > -1)
		{
			this.SetMenuState(EMainMenuState.StageSelectionLayer, false);
			lastLandSelected = -1;
		}
		//jesli otwarte okienko, schowaj
		MainMenuInfoWindow.Instance.HideInfoWindow();
	}

	private void SetMenuState(EMainMenuState stateToModify, bool isOpening)
	{
		if(isOpening == true)
			StartCoroutine(AnimateStateChange(stateToModify, EMainMenuState.NONE));
		else
			StartCoroutine(AnimateStateChange(EMainMenuState.NONE, stateToModify));
	}
	

	IEnumerator AnimateStateChange(EMainMenuState stateToShow, EMainMenuState stateToHide)
	{
		
		float currentTime = 0f;
		float animTime = 0f;
		GameObject layerToHide = this.FindLayerObjectOfMenuState(stateToHide);
		GameObject layerToShow = this.FindLayerObjectOfMenuState(stateToShow);

		//SetRigidbodyKinematicOfLayer(layerToShow, true);
		//SetRigidbodyKinematicOfLayer(layerToHide, true);
		layerToShow?.gameObject.SetActive(true);

		while(currentTime <= animTime)
		{
			currentTime += Time.deltaTime;
			float currentFactor = Mathf.Clamp01(currentTime/animTime);
			//if(layerToHide != null) layerToHide.transform.localScale = Vector3.one * 10f * (1f-currentFactor);
			//if(layerToShow != null) layerToShow.transform.localScale = Vector3.one * 10f * currentFactor;

			yield return null;
		}
			
		//SetRigidbodyKinematicOfLayer(layerToShow, false);
		//SetRigidbodyKinematicOfLayer(layerToHide, false);
		layerToHide?.gameObject.SetActive(false);
	}

	private GameObject FindLayerObjectOfMenuState(EMainMenuState menuStateToFind)
	{
		foreach(MenuLayers layer in menuLayers)
		{
			if(layer.menuState == menuStateToFind)
				return layer.layerObject;
		}
		return null;
	}

	private void SetRigidbodyKinematicOfLayer(GameObject layerParent, bool kinematicState)
	{
		if(layerParent == null) return;
		Rigidbody[] childRigidbodies = layerParent.GetComponentsInChildren<Rigidbody>(true);
		foreach(Rigidbody childRigidbody in childRigidbodies)
		{
			childRigidbody.isKinematic = kinematicState;
		}
	}

public void OnSettingsButtonClicked()
{
upgradesCanvas.OnCloseUpgrade();
this.SwitchMenuState(EMainMenuState.SettingsLayer);
CloseStats();
}

public void OnLeaderboardButtonClicked()
{
    upgradesCanvas.OnCloseUpgrade();
    this.SwitchMenuState(EMainMenuState.Leaderboard);
    CloseStats();
}
}
