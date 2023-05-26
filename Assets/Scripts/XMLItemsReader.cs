using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System.IO;
using System.Linq;

public class XMLItemsReader : MonoBehaviour 
{
	private static Dictionary<string, string> allItems;
	private static XmlElement root = null;

	private static void InitXml()
	{
		TextAsset xmlText = Resources.Load("Items") as TextAsset;
		if (xmlText != null) {
			XmlDocument xml = new XmlDocument ();
			xml.LoadXml (xmlText.ToString ());
			root = xml.DocumentElement;
		}
	}
		
	public static Dictionary<string, string> GetAllItemsByType(string type)
	{
		if (allItems == null)
			AddAllItemsToDictionary ();

		return allItems.Where(p => p.Value.Equals(type)).ToDictionary(p => p.Key, p => p.Value);
	}
		
	public static Dictionary<string, string> GetAllItems()
	{
		if (allItems == null)
			AddAllItemsToDictionary ();

		return allItems;
	}
	public static string GetItemType(string itemCodeName)
	{
		if (allItems == null)
			AddAllItemsToDictionary ();

		return allItems [itemCodeName];
	}
		
	public static List<string> GetAllSpells()
	{
		return XMLItemsReader.GetAllItemsByType("Spell").Keys.ToList();
	}

	public static List<string> GetAllSummons()
	{
		return XMLItemsReader.GetAllItemsByType("Summon").Keys.ToList();
	}

	// Stats // // //
	public static int GetManaCost(string itemCodeName, int currentLevel)
	{
		return (int)XMLItemsReader.GetSpecificFloatStat("mana", itemCodeName, currentLevel);
	}

	public static int GetDamageValue(string itemCodeName, int currentLevel)
	{
		return (int)XMLItemsReader.GetSpecificFloatStat("damage", itemCodeName, currentLevel);
	}

	public static float GetThrowBackValue(string itemCodeName, int currentLevel)
	{
		return XMLItemsReader.GetSpecificFloatStat("throwback", itemCodeName, currentLevel);
	}
	public static float GetUpwardDriftValue(string itemCodeName, int currentLevel)
	{
		return XMLItemsReader.GetSpecificFloatStat("upward", itemCodeName, currentLevel);
	}
	public static float GetCritChanceValue(string itemCodeName, int currentLevel)
	{
		return XMLItemsReader.GetSpecificFloatStat("crit", itemCodeName, currentLevel);
	}
	public static float GetRadiusValue(string itemCodeName, int currentLevel)
	{
		return XMLItemsReader.GetSpecificFloatStat("radius", itemCodeName, currentLevel);
	}
		
	public static float GetDurationValue(string itemCodeName, int currentLevel)
	{
		return XMLItemsReader.GetSpecificFloatStat("duration", itemCodeName, currentLevel);
	}

	public static float GetAttackRateValue(string itemCodeName, int currentLevel)
	{
		return XMLItemsReader.GetSpecificFloatStat("attackRate", itemCodeName, currentLevel);
	}

	public static int GetHealthValue(string itemCodeName, int currentLevel)
	{
		return (int) XMLItemsReader.GetSpecificFloatStat("health", itemCodeName, currentLevel);
	}

	public static int GetLifeTimeValue(string itemCodeName, int currentLevel)
	{
		return (int) XMLItemsReader.GetSpecificFloatStat("lifeTime", itemCodeName, currentLevel);
	}

	public static float GetSpeedValue(string itemCodeName, int currentLevel)
	{
		return XMLItemsReader.GetSpecificFloatStat("speed", itemCodeName, currentLevel);
	}

	public static float GetSideEffectDamage(string itemCodeName, int currentLevel)
	{
		return XMLItemsReader.GetSpecificFloatStat("side_effect_damage", itemCodeName, currentLevel);
	}

	public static int GetSideEffectDamageCount(string itemCodeName, int currentLevel)
	{
		return (int)XMLItemsReader.GetSpecificFloatStat("side_effect_damage_count", itemCodeName, currentLevel);
	}

	public static float GetSideEffectRateTime(string itemCodeName, int currentLevel)
	{
		return XMLItemsReader.GetSpecificFloatStat("side_effect_rate_time", itemCodeName, currentLevel);
	}

	public static float GetCustomStatValue(string itemCodeName, int currentLevel, string statName)
	{
		return XMLItemsReader.GetSpecificFloatStat(statName, itemCodeName, currentLevel);
	}


	public static string GetWeaponCodeName(string itemCodeName)
	{
		return XMLItemsReader.GetSpecificStringParam("weapon", itemCodeName);
	}


	public static bool IsSpellNextLevelAvailable(string itemCodeName, int currentLevel)
	{
		if(currentLevel > 0)
		{
			if (root == null)
				InitXml ();

			for(int i=0; i<root.ChildNodes.Count; i++)
			{
				if (root.ChildNodes.Item (i).Attributes ["itemCodeName"].Value.Equals (itemCodeName)) 
				{
					XmlNodeList elementList = root.ChildNodes.Item(i).SelectNodes("stats");
					Debug.Log ("stats: " + elementList.Count + " " + currentLevel);
					return elementList.Count > currentLevel ? true : false;
				}
			}
			return false;
		}else{
			return true;
		}
	}

	// Params // // //
	public static float GetActivationDelayValue(string itemCodeName)
	{
		return XMLItemsReader.GetSpecificFloatParam("activation_delay", itemCodeName);
	}

	public static bool GetInstantDamageValue(string itemCodeName)
	{
		return System.Convert.ToBoolean((int)XMLItemsReader.GetSpecificFloatParam("instant_damage", itemCodeName), System.Globalization.CultureInfo.InvariantCulture);
	}

	public static float GetDestroyTimeValue(string itemCodeName)
	{
		return XMLItemsReader.GetSpecificFloatParam("destroy_time", itemCodeName);
	}

	public static float GetDamageStepValue(string itemCodeName)
	{
		return XMLItemsReader.GetSpecificFloatParam("damage_step", itemCodeName);
	}
		

	// // // // // // // // // // // // // // //

	private static float GetSpecificFloatStat(string statName, string itemCodeName, int currentLevel)
	{
		
		string stringStat = GetSpecificStringStat(statName, itemCodeName, currentLevel);
		if(stringStat != null)
			return float.Parse(stringStat, System.Globalization.CultureInfo.InvariantCulture);
		else
			return -1f;
	}

	private static string GetSpecificStringStat(string statName, string itemCodeName, int currentLevel)
	{
		if (root == null)
			InitXml ();
		for(int i=0; i<root.ChildNodes.Count; i++)
		{
			if (root.ChildNodes.Item (i).Attributes ["itemCodeName"].Value.Equals (itemCodeName)) 
			{
				XmlNodeList elementList = root.ChildNodes.Item(i).SelectNodes("stats");
				for(int j=0; j<elementList.Count; j++)
				{
					if (int.Parse (elementList.Item (j).Attributes ["level"].Value) == currentLevel) 
					{
						XmlNodeList statsList = elementList.Item(j).ChildNodes;

						for(int k=0; k<statsList.Count; k++)
						{
							if(statsList.Item(k).Name == statName)
							{
								return statsList.Item (k).Attributes ["value"].Value;
							}
						}
					}
				}
			}
		}
		return null;
	}
		

	private static float GetSpecificFloatParam(string paramName, string itemCodeName)
	{
		string stringParam = GetSpecificStringParam(paramName, itemCodeName);
		if(stringParam != null)
			return float.Parse(stringParam, System.Globalization.CultureInfo.InvariantCulture);
		else 
			return -1f;
	}

	private static string GetSpecificStringParam(string paramName, string itemCodeName)
	{
		if (root == null)
			InitXml ();

		for(int i=0; i<root.ChildNodes.Count; i++)
		{

			if (root.ChildNodes.Item (i).Attributes ["itemCodeName"].Value.Equals (itemCodeName)) 
			{
				XmlNodeList paramsList = root.ChildNodes.Item(i).SelectSingleNode("params").ChildNodes;

				for(int j=0; j<paramsList.Count; j++)
				{
					if(paramsList.Item(j).Name == paramName)
					{
						return paramsList.Item(j).Attributes["value"].Value;
					}
				}
			}
		}
		return null;
	}


	public static string GetItemDescription(string itemCodeName)
	{		
		if (root == null)
			InitXml ();

		for(int i=0; i<root.ChildNodes.Count; i++)
		{
			if (root.ChildNodes.Item (i).Attributes ["itemCodeName"].Value.Equals (itemCodeName)) 
			{
				return root.ChildNodes.Item(i).SelectSingleNode("description").Attributes["value"].Value;
			}
		}
		return "none";
	}

	public static string[] GetItemUpgradePrice(string itemCodeName, int currentLevel)
	{		
		if (root == null)
			InitXml ();

		//int price = -1;
		for(int i=0; i<root.ChildNodes.Count; i++)
		{
			if (root.ChildNodes.Item (i).Attributes ["itemCodeName"].Value.Equals (itemCodeName)) {
				XmlNodeList upgrades = root.ChildNodes.Item(i).SelectNodes("upgradePrice");
				for (int j = 0; j < upgrades.Count; j++) {
					if (int.Parse (upgrades.Item (j).Attributes ["level"].Value) == currentLevel) 
					{
						Debug.Log ("upgrades.Item (j): " + upgrades.Item (j));
						string[] price = new string[2];
						price[0] = upgrades.Item (j).Attributes ["price"].Value;
						price[1] = upgrades.Item (j).Attributes ["currency"].Value;
						return price;
						//return int.Parse (fusions.Item (j).Attributes ["upgradePrice"].Value);
					}
				}
			}
		}
		return new string[]{"-1", "none"};
	}

	public static string[] GetItemUnlockPrice(string itemCodeName)
	{		
		if (root == null)
			InitXml ();

		for(int i=0; i<root.ChildNodes.Count; i++)
		{
			if (root.ChildNodes.Item (i).Attributes ["itemCodeName"].Value.Equals (itemCodeName)) 
			{
				XmlNode buyNode = root.ChildNodes.Item(i).SelectSingleNode("buyPrice");
				string[] price = new string[2];
				price[0] = buyNode.Attributes ["price"].Value;
				price[1] = buyNode.Attributes ["currency"].Value;
				return price;
			}
		}
		return new string[]{"-1", "none"};
	}

	public static int GetItemUnlockExpLevel(string itemCodeName)
	{		
		if (root == null)
			InitXml ();

		for(int i=0; i<root.ChildNodes.Count; i++)
		{
			if (root.ChildNodes.Item (i).Attributes ["itemCodeName"].Value.Equals (itemCodeName)) 
			{
				XmlNode unlockExpLevelNode = root.ChildNodes.Item(i).SelectSingleNode("levelExpNeed");
				return int.Parse(unlockExpLevelNode.Attributes ["level"].Value);
			}
		}
		return -1;
	}

	/*public static int GetFusionUpgradeItemCount(string codeName, int currentFusion)
	{
		if (root == null)
			InitXml ();
		
		int upgradeItemCount = -1;
		for(int i=0; i<root.ChildNodes.Count; i++)
		{
			if (root.ChildNodes.Item (i).Attributes ["itemCodeName"].Value.Equals (codeName)) {
				XmlNodeList fusions = root.ChildNodes.Item(i).SelectNodes("fusion");
				for (int j = 0; j < fusions.Count; j++) {
					if (int.Parse (fusions.Item (j).Attributes ["level"].Value) == currentFusion + 1) {
						return int.Parse (fusions.Item (j).Attributes ["upgradeItemCount"].Value);
					}
				}
			}
		}
		return upgradeItemCount;
	}
		
	public static int GetMaxFusion(string itemCodeName)
	{		
		if (root == null)
			InitXml ();
		int maxFusion = 0;
		for(int i=0; i<root.ChildNodes.Count; i++)
		{
			if (root.ChildNodes.Item (i).Attributes ["itemCodeName"].Value.Equals (itemCodeName)) {
				maxFusion = root.ChildNodes.Item(i).SelectNodes("fusion").Count;
				return maxFusion;
			}
		}
		return maxFusion;
	}*/
		

	private static void AddAllItemsToDictionary () 
	{
		if (root == null)
			InitXml ();

		allItems = new Dictionary<string, string> ();
		for(int i=0; i<root.ChildNodes.Count; i++)
		{
			allItems.Add (root.ChildNodes.Item (i).Attributes ["itemCodeName"].Value, 
				root.ChildNodes.Item (i).Attributes ["type"].Value);
		}
	}

	public static void ReadXML () 
	{
		if (root == null)
			InitXml ();

		for(int i=0; i<root.ChildNodes.Count; i++)
		{
			Debug.Log(root.ChildNodes.Item(i).Attributes["itemCodeName"].Value);
			/*XmlNodeList fusions = root.ChildNodes.Item(i).SelectNodes("fusion");
			for(int j=0; j<fusions.Count; j++)
			{
				Debug.Log("Fusion " + j + " upgrade price:" + fusions.Item(j).Attributes["upgradePrice"].Value);
			}
			XmlNodeList evolutions = root.ChildNodes.Item(i).SelectNodes("evolution");
			for(int j=0; j<evolutions.Count; j++)
			{
				Debug.Log("Evolution level: " + evolutions.Item(j).Attributes["level"].Value);
				XmlNodeList evolutionRequirements = evolutions.Item(j).ChildNodes;
				for(int k=0; k<evolutionRequirements.Count; k++)
				{
					Debug.Log("Evolution currency Value: " + evolutionRequirements.Item(k).Attributes["value"].Value);
				}
			}*/
		}
	}
}
