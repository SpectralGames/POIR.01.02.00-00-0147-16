using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeStatsController : MonoBehaviour {

	public Text upgradableNameTf;
	public Text descriptionTf;
	public Text levelTf;
	public Text currentLevelTf;
	public Text costTf;

	public Image upgradeImage;
	public Image buyImage;

	public GameObject upgradeButton;
	public GameObject lockItem;

	public ChartStatsController attackDamageStats;
	public ChartStatsController attackRadiusStats;
	public ChartStatsController manaCostStats;
	public ChartStatsController attackRateStats;
	public ChartStatsController lifeTimeStats;
	public ChartStatsController healthStats;

	public Transform headerIconsParent, gestureSymbolsParent;

	public Text damageValue, radiusValue, healthValue, costValue, rateValue, activeTimeValue;

	public void ShowStats (string currentItemCodeName, int currentLevel, bool animateValues, bool isSummonLayer, bool isSimulated = false)
	{
		upgradableNameTf.text = currentItemCodeName;
		levelTf.text = "UPGRADE ";   // + currentLevel.ToString()
		currentLevelTf.text = "LVL" + currentLevel.ToString();
		descriptionTf.text = XMLItemsReader.GetItemDescription(currentItemCodeName);
		//ikonka w headerze
		for(int i=0; i<headerIconsParent.childCount; i++)
			headerIconsParent.GetChild(i).gameObject.SetActive(headerIconsParent.GetChild(i).name == currentItemCodeName);
		//gest w opisie
		for(int i=0; i<gestureSymbolsParent.childCount; i++)
			gestureSymbolsParent.GetChild(i).gameObject.SetActive(gestureSymbolsParent.GetChild(i).name == currentItemCodeName);


		if(currentLevel == -1)
		{
			upgradeButton.SetActive(false);
			lockItem.SetActive (true);
			levelTf.text = "level "+ XMLItemsReader.GetItemUnlockExpLevel(currentItemCodeName).ToString() +" required";
		}else if(currentLevel == 0)
		{
			int upgradePrice = int.Parse(XMLItemsReader.GetItemUnlockPrice (currentItemCodeName) [0]);
			levelTf.text = "Unlock :";
			upgradeButton.SetActive(true);
			lockItem.SetActive (true);
			costTf.text =upgradePrice.ToString();
			upgradeImage.gameObject.SetActive(false);
			buyImage.gameObject.SetActive(true);
		}
		else
		{
			if(XMLItemsReader.IsSpellNextLevelAvailable(currentItemCodeName, currentLevel))
			{
				int upgradePrice = int.Parse(XMLItemsReader.GetItemUpgradePrice (currentItemCodeName, currentLevel) [0]);
				upgradeButton.SetActive(true);
				lockItem.SetActive (false);
				costTf.text = upgradePrice.ToString();
				upgradeImage.gameObject.SetActive(true);
				buyImage.gameObject.SetActive(false);
			}else{
				upgradeButton.SetActive(false);
				lockItem.SetActive (false);
				levelTf.text = "MAX LVL REACHED";
			}
		}

		//dla statow
		if (currentLevel < 1) {
			currentLevel = 1;
			isSimulated = false;
		}
			
		if (isSummonLayer) {
			string summonAttackWeaponCodeName = XMLItemsReader.GetWeaponCodeName (currentItemCodeName);
			if (animateValues) {
				attackDamageStats.SetDataAndAnimate (60f, XMLItemsReader.GetDamageValue (summonAttackWeaponCodeName, currentLevel), XMLItemsReader.GetDamageValue (summonAttackWeaponCodeName, currentLevel+1), isSimulated);
				attackRadiusStats.SetDataAndAnimate (15f, XMLItemsReader.GetRadiusValue (currentItemCodeName, currentLevel), XMLItemsReader.GetRadiusValue (currentItemCodeName, currentLevel+1), isSimulated);
				manaCostStats.SetDataAndAnimate (50f, XMLItemsReader.GetManaCost (currentItemCodeName, currentLevel), XMLItemsReader.GetManaCost (currentItemCodeName, currentLevel+1), isSimulated);
				attackRateStats.SetDataAndAnimate (60f, XMLItemsReader.GetAttackRateValue (currentItemCodeName, currentLevel), XMLItemsReader.GetAttackRateValue (currentItemCodeName, currentLevel+1), isSimulated);
				lifeTimeStats.SetDataAndAnimate (60f, XMLItemsReader.GetLifeTimeValue (currentItemCodeName, currentLevel), XMLItemsReader.GetLifeTimeValue (currentItemCodeName, currentLevel+1), isSimulated);
				healthStats.SetDataAndAnimate (200f, XMLItemsReader.GetHealthValue (currentItemCodeName, currentLevel), XMLItemsReader.GetHealthValue (currentItemCodeName, currentLevel+1), isSimulated); 
			} else {
				attackDamageStats.SetDataAndAnimate (60f, XMLItemsReader.GetDamageValue (summonAttackWeaponCodeName, currentLevel), XMLItemsReader.GetDamageValue (summonAttackWeaponCodeName, currentLevel+1), isSimulated);
				attackRadiusStats.SetData (15f, XMLItemsReader.GetRadiusValue (currentItemCodeName, currentLevel), XMLItemsReader.GetRadiusValue (currentItemCodeName, currentLevel+1), isSimulated);
				manaCostStats.SetData (50f, XMLItemsReader.GetManaCost (currentItemCodeName, currentLevel), XMLItemsReader.GetManaCost (currentItemCodeName, currentLevel+1), isSimulated);
				attackRateStats.SetData (60f, XMLItemsReader.GetAttackRateValue (currentItemCodeName, currentLevel), XMLItemsReader.GetAttackRateValue (currentItemCodeName, currentLevel+1), isSimulated);
				lifeTimeStats.SetData (60f, XMLItemsReader.GetLifeTimeValue (currentItemCodeName, currentLevel), XMLItemsReader.GetLifeTimeValue (currentItemCodeName, currentLevel+1), isSimulated);
				healthStats.SetData (200f, XMLItemsReader.GetHealthValue (currentItemCodeName, currentLevel), XMLItemsReader.GetHealthValue (currentItemCodeName, currentLevel+1), isSimulated);
			}
			damageValue.text = XMLItemsReader.GetDamageValue (summonAttackWeaponCodeName, currentLevel).ToString() + (isSimulated ? 
				("+" + (XMLItemsReader.GetDamageValue (summonAttackWeaponCodeName, currentLevel+1) - XMLItemsReader.GetDamageValue (summonAttackWeaponCodeName, currentLevel)).ToString()) : "");
			healthValue.text = XMLItemsReader.GetHealthValue (currentItemCodeName, currentLevel).ToString() + (isSimulated ? 
				("+" + (XMLItemsReader.GetHealthValue (currentItemCodeName, currentLevel+1) - XMLItemsReader.GetHealthValue (currentItemCodeName, currentLevel)).ToString()) : "");

			activeTimeValue.text = XMLItemsReader.GetLifeTimeValue (currentItemCodeName, currentLevel).ToString() + (isSimulated ? 
				("+" + (XMLItemsReader.GetLifeTimeValue (currentItemCodeName, currentLevel + 1) - XMLItemsReader.GetLifeTimeValue (currentItemCodeName, currentLevel))) + " s" : " s");
			
		
		} else {
			if(animateValues)
			{
				attackDamageStats.SetDataAndAnimate(100f, XMLItemsReader.GetDamageValue(currentItemCodeName, currentLevel), XMLItemsReader.GetDamageValue(currentItemCodeName, currentLevel+1), isSimulated);
				attackRadiusStats.SetDataAndAnimate(15f, XMLItemsReader.GetRadiusValue(currentItemCodeName, currentLevel), XMLItemsReader.GetRadiusValue(currentItemCodeName, currentLevel+1), isSimulated);
				manaCostStats.SetDataAndAnimate(50f, XMLItemsReader.GetManaCost(currentItemCodeName, currentLevel), XMLItemsReader.GetManaCost(currentItemCodeName, currentLevel+1), isSimulated);
			}else{
				attackDamageStats.SetData(100f, XMLItemsReader.GetDamageValue(currentItemCodeName, currentLevel), XMLItemsReader.GetDamageValue(currentItemCodeName, currentLevel+1), isSimulated);
				attackRadiusStats.SetData(15f, XMLItemsReader.GetRadiusValue(currentItemCodeName, currentLevel), XMLItemsReader.GetRadiusValue(currentItemCodeName, currentLevel+1), isSimulated);
				manaCostStats.SetData(50f, XMLItemsReader.GetManaCost(currentItemCodeName, currentLevel), XMLItemsReader.GetManaCost(currentItemCodeName, currentLevel+1), isSimulated);
			}
			damageValue.text = XMLItemsReader.GetDamageValue (currentItemCodeName, currentLevel).ToString() + (isSimulated ? 
				("+" + (XMLItemsReader.GetDamageValue (currentItemCodeName, currentLevel+1) - XMLItemsReader.GetDamageValue (currentItemCodeName, currentLevel)).ToString()) : "");

			if (XMLItemsReader.GetDurationValue (currentItemCodeName, currentLevel) == -1) {
				activeTimeValue.gameObject.transform.parent.gameObject.SetActive (false);
			} else {
				activeTimeValue.gameObject.transform.parent.gameObject.SetActive (true);
				activeTimeValue.text = XMLItemsReader.GetDurationValue (currentItemCodeName, currentLevel).ToString() + (isSimulated ? 
					("+" + (XMLItemsReader.GetDurationValue (currentItemCodeName, currentLevel + 1) - XMLItemsReader.GetDurationValue (currentItemCodeName, currentLevel))) + " s" : " s");
			}
		}

		radiusValue.text = XMLItemsReader.GetRadiusValue (currentItemCodeName, currentLevel).ToString() + (isSimulated ? 
			("+" + (XMLItemsReader.GetRadiusValue (currentItemCodeName, currentLevel+1) - XMLItemsReader.GetRadiusValue (currentItemCodeName, currentLevel)).ToString()) : "");

		costValue.text = XMLItemsReader.GetManaCost (currentItemCodeName, currentLevel).ToString() + (isSimulated ? 
			("+" + (XMLItemsReader.GetManaCost (currentItemCodeName, currentLevel + 1) - XMLItemsReader.GetManaCost (currentItemCodeName, currentLevel))) : "");
	
		rateValue.text = XMLItemsReader.GetAttackRateValue (currentItemCodeName, currentLevel).ToString() + (isSimulated ? 
			("+" + (XMLItemsReader.GetAttackRateValue (currentItemCodeName, currentLevel + 1) - XMLItemsReader.GetAttackRateValue (currentItemCodeName, currentLevel))) : "");
		
	//	rateValue.text = XMLItemsReader.GetAttackRateValue (currentItemCodeName, currentLevel).ToString() + " s";
	}
}
