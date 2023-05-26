using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System.IO;
using System.Linq;

public class XMLGameDataReader : MonoBehaviour 
{
	private static XmlElement root = null;

	private static Dictionary<string, string> playerExpLevels;
	private static List<WorldItem> gameWorlds;
	private static void InitXml()
	{
		TextAsset xmlText = Resources.Load("GameData") as TextAsset;
		if (xmlText != null) {
			XmlDocument xml = new XmlDocument ();
			xml.LoadXml (xmlText.ToString ());
			root = xml.DocumentElement;
		}
	}
		
	public static int GetNextLevelExpPoints(int currentLevel)
	{
		if (root == null)
			InitXml ();

		XmlNode playerLevelsList = root.SelectSingleNode("PlayerLevels");
		XmlNodeList levelsListItems = playerLevelsList.ChildNodes;
		for (int i = 0; i < levelsListItems.Count; i++)
		{
			if (int.Parse (levelsListItems.Item (i).Attributes ["id"].Value) == currentLevel + 1)
				return int.Parse (levelsListItems.Item (i).Attributes ["expPoints"].Value);
		}
		return -1;
	}

	public static Dictionary<string, string> GetLevelsExpPoints()
	{
		if (root == null)
			InitXml ();

		if (playerExpLevels == null) 
		{
			playerExpLevels = new Dictionary<string, string> ();
			XmlNode playerLevelsList = root.SelectSingleNode ("PlayerLevels");
			XmlNodeList levelsListItems = playerLevelsList.ChildNodes;
			for (int i = 0; i < levelsListItems.Count; i++) {
				playerExpLevels.Add (levelsListItems.Item (i).Attributes ["id"].Value, levelsListItems.Item (i).Attributes ["expPoints"].Value);
			}
		} 

		return playerExpLevels;
	}

	private static void InitWorldsData()
	{
		if (root == null)
			InitXml ();

		if (gameWorlds == null)
		{
			gameWorlds = new List<WorldItem> ();
			XmlNode playerLevelsList = root.SelectSingleNode ("Worlds");
			XmlNodeList levelsListItems = playerLevelsList.ChildNodes;
			for (int i = 0; i < levelsListItems.Count; i++) {
				WorldItem world = new WorldItem ();
				world.id = int.Parse (levelsListItems.Item (i).Attributes ["id"].Value);
				world.levels = int.Parse (levelsListItems.Item (i).Attributes ["levels"].Value);
				world.name = levelsListItems.Item (i).Attributes ["name"].Value;
                world.startToCompleteWorld = int.Parse(levelsListItems.Item(i).Attributes["starsToCompleteWorld"].Value);
                // set levels

                world.levelList = new List<LevelItem>();
                XmlNodeList levels = levelsListItems[i].ChildNodes;
                for (int j = 0; j < levels.Count; j++)
                {
                    LevelItem level = new LevelItem();
                    level.id = int.Parse(levels[j].Attributes["id"].Value);
                    level.type = levels[j].Attributes["type"].Value;
                    world.levelList.Add(level);
                }
                  
                gameWorlds.Add (world);
				SaveGameController.instance.AddWorldStatusComplete (world.id);
			}
		}
		SaveGameController.instance.SaveGameData ();
	}

	public static WorldItem GetWorldByID(int worldId)
	{
		if (root == null)
			InitXml ();

		if (gameWorlds == null)
			InitWorldsData ();
		
		foreach (WorldItem item in gameWorlds) 
		{
			if (item.id == worldId)
				return item;
		}
		return null;
	}
    /*
    public static string GetLevelTypeByID(int worldId, int levelId)
    {
        if (root == null)
            InitXml();

        if (gameWorlds == null)
            InitWorldsData();

        foreach (WorldItem item in gameWorlds)
        {
            if (item.id == worldId)
                return item;
        }
        return null;
    }
    */

    public static List<WorldItem> GetWorlds()
	{
		if (gameWorlds == null)
			InitWorldsData ();

		return gameWorlds;
	}
	public static int GetStartGoldValue()
	{
		if (root == null)
			InitXml ();
		
		return int.Parse (root.SelectSingleNode ("GoldStartValue").FirstChild.Value);
	}

	public static int GetExpForVirginValue()
	{
		if (root == null)
			InitXml ();

		return int.Parse (root.SelectSingleNode ("ExpForVirgin").FirstChild.Value);
	}

	public static int GetGoldForVirginValue()
	{
		if (root == null)
			InitXml ();

		return int.Parse (root.SelectSingleNode ("GoldForVirgin").FirstChild.Value);
	}

	public static int GetGoldForLevelCompleteValue()
	{
		if (root == null)
			InitXml ();

		return int.Parse (root.SelectSingleNode ("GoldForLevelComplete").FirstChild.Value);
	}

	public static int GetExpForLevelCompleteValue()
	{
		if (root == null)
			InitXml ();

		return int.Parse (root.SelectSingleNode ("EpxForLevelComplete").FirstChild.Value);
	}
}
public class WorldItem
{
	public int id;
	public string name;
	public int levels;
    public List<LevelItem> levelList;
    public int startToCompleteWorld;
}
public class LevelItem
{
    public int id;
    public string type;
}
