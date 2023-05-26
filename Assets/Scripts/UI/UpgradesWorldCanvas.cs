using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpgradesWorldCanvas : MonoBehaviour 
{
	private bool isUpgradingSpells = false;
//	public GameObject spellUpgradeButton;
//	public GameObject summonUpgradeButton;
	public GameObject spawnUpgradablePivot;
	public MaidenSpawnTower spawnTower;
	public AttacksController attackController;
	public UpgradeStatsController summonStatsLayer, spellsStatsLayer;
	public PortalEnemiesController enemiesPortal;

	public List<AttackItemHolder> lockedAttackModelsList;
	public GameObject lockedSpellSpawnPoint;

	private List<string> currentUpgradableList;
	private int currentElementSelected;
	private GameObject currentSummon;

	// Use this for initialization
	void Awake () 
	{
		currentUpgradableList = new List<string>();
		//Debug.Log(SaveGameController.instance.GetScore() + " score");
		//Debug.Log(XMLItemsReader.GetInstantDamageValue("FireBall").ToString() + " bool");
	}

	public void OnOpenUpgradeLayer(bool spellsLayer, string upgradableOnStartCodeName = null)
	{
		isUpgradingSpells = spellsLayer;
		//RemoveSummonModel ();
		//RemoveLockedSpell ();
		currentElementSelected = 0;
		currentUpgradableList = isUpgradingSpells ? XMLItemsReader.GetAllSpells() : XMLItemsReader.GetAllSummons();
		if (upgradableOnStartCodeName != null) {
			currentElementSelected = currentUpgradableList.IndexOf (upgradableOnStartCodeName);
        }
		this.OnUpdateUpgradeLayer(false);
	}

	public void OnCloseUpgrade()
	{
		RemoveSummonModel ();
		RemoveLockedSpell ();
		CancelInvoke ();
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(Input.GetKeyDown(KeyCode.G))
		{
			SaveGameController.instance.AddGold(5000);
			this.GetComponent<MainMenuWorldCanvas>().playerStats.UpdateUI();
			SaveGameController.instance.SaveGameData();
		}else if(Input.GetKeyDown(KeyCode.E)){
			SaveGameController.instance.AddExp(5000);
			this.GetComponent<MainMenuWorldCanvas>().playerStats.UpdateUI();
			SaveGameController.instance.SaveGameData();
		}
	}
		
	public void OnUpgradeSpellButtonClicked()
	{
		string currentItemCodeName = currentUpgradableList[currentElementSelected];
		int currentLevel = SaveGameController.instance.GetItemLevel(currentItemCodeName);

		int price = 0;
		if (currentLevel == 0) 
		{
			price = int.Parse( XMLItemsReader.GetItemUnlockPrice (currentItemCodeName) [0]);
		}else{
			price = int.Parse( XMLItemsReader.GetItemUpgradePrice (currentItemCodeName, currentLevel) [0]);
		}
			
		SaveGameController.instance.SetItemLevel(currentItemCodeName, currentLevel+1);
		SaveGameController.instance.SetGold (SaveGameController.instance.GetGold () - price);
		SaveGameController.instance.SaveGameData ();

		if (isUpgradingSpells)
			this.OnUpdateSpellsLayer (false);
		else
			this.OnUpdateSummonsLayer (false);

        CheckAllUnlocked(isUpgradingSpells);
		SaveGameController.instance.SaveGameData ();
	}

    private void CheckAllUnlocked(bool spells)
    {
        bool allUnlocked = true;
        foreach(var spell in spells ? XMLItemsReader.GetAllSpells(): XMLItemsReader.GetAllSummons())
        {
            if(SaveGameController.instance.GetItemLevel(spell) <= 0)
            {
                allUnlocked = false;
            }
        }
        if(allUnlocked)
        {
            //SteamStatsAndAchievements.Instance.UnlockAchievement(spells ? SteamStatsAndAchievements.ACHIEVEMENT_UNLOCK_ALL_SPELLS: SteamStatsAndAchievements.ACHIEVEMENT_UNLOCK_ALL_SUMMONS);
        }
    }

    public void OnPreviousItemButtonClicked()
	{
		currentElementSelected = currentElementSelected-1 < 0 ? currentUpgradableList.Count-1 : currentElementSelected-1;
		this.OnUpdateUpgradeLayer();
	}

	public void OnNextItemButtonClicked()
	{
		currentElementSelected = (currentElementSelected+1) % currentUpgradableList.Count;
		this.OnUpdateUpgradeLayer();
	}

	public string GetCurrentSelectedElementCodeName()
	{
		return currentUpgradableList[currentElementSelected];
	}

	public void SimulateUpgradeItemStats()
	{
		string currentItemCodeName = GetCurrentSelectedElementCodeName();
		int currentLevel = SaveGameController.instance.GetItemLevel(currentItemCodeName);

		this.OnUpdateUpgradeLayer(true, currentLevel, true);
	}



	public void OnUpdateUpgradeLayer(bool animateValues = true, int levelToSelect = -1, bool isSimulated = false)
	{
		if (isUpgradingSpells) 
			this.OnUpdateSpellsLayer (animateValues, levelToSelect, isSimulated);
		else
			this.OnUpdateSummonsLayer(animateValues, levelToSelect, isSimulated);
	}

	private void OnUpdateSpellsLayer(bool animateValues = true, int levelToSelect = -1, bool isSimulated = false)
	{
		string currentItemCodeName = GetCurrentSelectedElementCodeName();
		int currentLevel = levelToSelect == -1 ? SaveGameController.instance.GetItemLevel(currentItemCodeName) : levelToSelect;

		spellsStatsLayer.ShowStats (currentItemCodeName, currentLevel, animateValues, false, isSimulated);
		if (animateValues == false) {
			CancelInvoke ();
			RemoveLockedSpell ();
			if (currentLevel < 1) {
				Instantiate (GetLockedPrefab (currentItemCodeName), lockedSpellSpawnPoint.transform.position, lockedSpellSpawnPoint.transform.rotation, lockedSpellSpawnPoint.transform);

			} else {
			//	CreateSpellAttack ();
			}
		}
        ObjectPool.Instance.player.attackController.DestroyTempAttack();


    }

	private void CreateSpellAttack()
	{
        attackController.CreateMenuAttack(spawnUpgradablePivot.transform, GetCurrentSelectedElementCodeName(), enemiesPortal.GetCurrentEnemyPosition()); 
		int currentLevel = SaveGameController.instance.GetItemLevel(GetCurrentSelectedElementCodeName());

		bool isInstantAttack = XMLItemsReader.GetInstantDamageValue (GetCurrentSelectedElementCodeName ());
		CancelInvoke ();
		if (isInstantAttack) 
			Invoke ("CreateSpellAttack",  5);	
		else 
		{
			if (currentLevel < 1)
				currentLevel = 1;
			
			Invoke ("CreateSpellAttack",  XMLItemsReader.GetDurationValue (GetCurrentSelectedElementCodeName (), currentLevel) + 5);
		}
	}

	private void OnUpdateSummonsLayer(bool animateValues = true, int levelToSelect = -1, bool isSimulated = false)
	{
		string currentItemCodeName = GetCurrentSelectedElementCodeName();
		int currentLevel = levelToSelect == -1 ? SaveGameController.instance.GetItemLevel(currentItemCodeName) : levelToSelect;

		if (animateValues == false)
		{
			bool summonExist = spawnTower.IsTowerPlaced ();
			Debug.Log ("summonexist:" + summonExist);
			RemoveSummonModel ();
			CancelInvoke ();
			if (summonExist)
				Invoke ("CreateSummonAttack", .5f);
			else
				CreateSummonAttack ();
		}
		 
		summonStatsLayer.ShowStats (currentItemCodeName, currentLevel, animateValues, true, isSimulated);
	}

	private void CreateSummonAttack()
	{
		string currentItemCodeName = GetCurrentSelectedElementCodeName();
		int currentLevel = SaveGameController.instance.GetItemLevel (currentItemCodeName);
		if (currentLevel < 1) {
			GameObject maiden = Instantiate (GetLockedPrefab (currentItemCodeName), spawnTower.GetSpawnPoint (), Quaternion.identity, attackController.GetTowersParent ());
			if (maiden.GetComponent<TowerBase> () != null) {
				//	maiden.GetComponent<TowerBase> ().OnSetValues (GetCurrentAttackLevel (attackType));
				spawnTower.OnTowerSpawned (maiden.GetComponent<TowerBase> ());
			}
		} else {
			attackController.CreateMenuTower (spawnTower, currentItemCodeName);
		}
	}

	private GameObject GetLockedPrefab(string itemCodeName)
	{
		foreach (AttackItemHolder item in lockedAttackModelsList) {
			if (item.type.ToString().Equals (itemCodeName)) {
				return item.attackPrefabs [0];
			}
		}
		return null;
	}

	private void CheckMaxUpgrade()
	{
		string currentItemCodeName = currentUpgradableList[currentElementSelected];
		int currentLevel = SaveGameController.instance.GetItemLevel(currentItemCodeName);
	//	if(currentLevel > 
	}

	private void RemoveSummonModel()
	{
		spawnTower.RemoveTower ();
	}

	private void RemoveLockedSpell()
	{
		foreach (Transform child in lockedSpellSpawnPoint.transform)
			Destroy (child.gameObject);
	}


}
