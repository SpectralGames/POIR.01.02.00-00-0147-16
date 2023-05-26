using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System;
using System.Text;
using System.Security.Cryptography;
using System.Runtime.Serialization;


public class SaveGameController : MonoBehaviour 
{
	private static SaveGameController _instance;
	public static SaveGameController instance 
	{
		get
		{
			// jesli nie zostal zainicjowany
			if(_instance == null)
			{
				_instance = new GameObject("SaveGameController").AddComponent<SaveGameController>();
				_instance.Init();
			}
			
			return _instance;
		}
	}
	
	//#if UNITY_EDITOR
	private static string PasswordHash; // = "SeTaGiE22";
	//#else
	//static readonly string PasswordHash = "SeTaGiE22" + SystemInfo.deviceUniqueIdentifier;
	//#endif
	static readonly string SaltKey = "CrimeGermanii039III";
	static readonly string VIKey = "AloAlooCo0598756";
	private static byte[] keyBytes;
	private SerializableGameData gameData; //ogolne savy



	private void Init()
	{
		DontDestroyOnLoad(_instance.gameObject);
		//CleanupSaves();
		#if UNITY_EDITOR
		PasswordHash = "SeTaGiE22";
		#else
		PasswordHash = "SeTaGiE22" + SaveGameController.GetUniqueID();
		#endif

		keyBytes = new Rfc2898DeriveBytes(PasswordHash, Encoding.ASCII.GetBytes(SaltKey)).GetBytes(256 / 8);

		// // // wczytywanie savow ogolnych // // // // // // // // // // //
		gameData = this.LoadGameData();
		if (gameData == null) {
			gameData = new SerializableGameData ();
			CleanPreferences ();
		}


	}


	public static string GetUniqueID()
	{
		string id = PlayerPrefs.GetString("Crime.CrimeID", "none");
		if(id.CompareTo("none") == 0) //nie byl zapisany, stworz nowy i zapisz
		{
			id = SystemInfo.deviceUniqueIdentifier;
			if (id == SystemInfo.unsupportedIdentifier || id.Length < 5) //niewspierany, generuje losowy id
			{
				id = StringHelper.GetRandomString(10);
			}
			PlayerPrefs.SetString("Crime.CrimeID", id);
			PlayerPrefs.Save();
		}
		return id;
	}

	public static void CleanupSaves()
	{
		//Destroy(_instance);
		//_instance = null;

		if(File.Exists(Application.persistentDataPath + "/gameData.dat"))
			File.Delete(Application.persistentDataPath + "/gameData.dat");

	}

	private void CleanPreferences()
	{		

	}
		
	public void SaveGameData(bool isEngagedByDisable = false)
	{
		//Debug.Log ("save game");
		//if(instance != null && instance.gameObject.activeSelf)
		SetItemLevel("WoodenBolt", 1);
		
		if (isEngagedByDisable)
			Save ();
		else
			StartCoroutine (SaveAfterEndOfFrame ());
	}

	IEnumerator SaveAfterEndOfFrame()
	{
		yield return new WaitForEndOfFrame ();
		Save ();
	}

	private void Save()
	{
		BinaryFormatter bf = new BinaryFormatter();
		try{
			using (FileStream fileStream = File.Create (Application.persistentDataPath + "/gameData.dat")) 
			{
				MemoryStream memoryStream = new MemoryStream ();
				try{
					bf.Serialize (memoryStream, gameData);
				}
				catch(SerializationException exception)
				{
					Debug.Log("ex:" + exception.Message);
				}

				MemoryStream encryptedMemoryStream = new MemoryStream (Encrypt (memoryStream.GetBuffer ())); //stream po zaszyfrowaniu
				encryptedMemoryStream.WriteTo (fileStream); //zapisz na dysku
				fileStream.Close ();
			}
		}
		catch(Exception e) {
            Debug.Log(e.Message);
        }
	}

	public SerializableGameData LoadGameData()
	{
		if(File.Exists(Application.persistentDataPath + "/gameData.dat"))
		{
			BinaryFormatter bf = new BinaryFormatter();
			using (FileStream fileStream = File.Open (Application.persistentDataPath + "/gameData.dat", FileMode.Open)) 
			{
				byte[] encryptedBytes = new byte[fileStream.Length];
				fileStream.Read (encryptedBytes, 0, encryptedBytes.Length); //przepisz filestream do encryptedbytes
				MemoryStream memoryStream = new MemoryStream (Decrypt (encryptedBytes)); //stream po rozszyfrowaniu


				SerializableGameData data = null;
				try{
					data = (SerializableGameData) bf.Deserialize (memoryStream);
					foreach( System.Reflection.FieldInfo fieldInfo in data.GetType().GetFields())
					{
						if(fieldInfo.GetValue(data) == null)
						{
							System.Object newInstance = Activator.CreateInstance(fieldInfo.FieldType);
							fieldInfo.SetValue(data, newInstance);
							Debug.Log("Field is NULL, created new instance of " + fieldInfo.Name + " "+  fieldInfo.FieldType);
						}
					}
				}
				catch(SerializationException exception)
				{
					Debug.Log("ex:" + exception.Message);
				}
				return data;
			}
		}else return null;
	}





	// Tutoriale // // // // // // // // // // // // // // // // // // // // // // // // // // // // // // // // //
	public bool IsTutorialFinished(string tutorialCodeName)
	{
		if(gameData.tutorialFinished.ContainsKey(tutorialCodeName))
		{
			return gameData.tutorialFinished[tutorialCodeName];
		}else
		{
			gameData.tutorialFinished.Add(tutorialCodeName, false);
			return false;
		}
	}

	public void SetTutorialFinished(string tutorialCodeName)
	{
		if(gameData.tutorialFinished.ContainsKey(tutorialCodeName))
		{
			gameData.tutorialFinished[tutorialCodeName] = true;
		}else
		{
			gameData.tutorialFinished.Add(tutorialCodeName, true);
		}
	}

	public int GetItemLevel(string itemCodeName) // -1 locked, 0 unlocked, 1 bought
	{
		
		if(gameData.itemLevels.ContainsKey(itemCodeName))
			return gameData.itemLevels[itemCodeName];
		else
			return -1;
	}

	public void SetItemLevel(string itemCodeName, int newLevel)
	{
		if(gameData.itemLevels.ContainsKey(itemCodeName))
			gameData.itemLevels[itemCodeName] = newLevel;
		else
			gameData.itemLevels.Add(itemCodeName, newLevel);
	}

	public bool IsItemBought(string itemCodeName)
	{
		if(gameData.itemLevels.ContainsKey(itemCodeName))
		{
			return gameData.itemLevels[itemCodeName] > 0;
		}else{
			return false;
		}
	}

	public bool IsItemUnlocked(string itemCodeName)
	{
		if(gameData.itemLevels.ContainsKey(itemCodeName))
		{
			return gameData.itemLevels[itemCodeName] > -1;
		}else{
			return false;
		}
	}

    /*public void SetMainTutorialFinished()
	{
		gameData.isMainTutorialFinished = true;	
		SaveGameData();
	}

	public bool IsMainTutorialFinished()
	{
		return gameData.isMainTutorialFinished;
	}*/
    public void SetVideoSettings(int quality)
    {
        gameData.videoSettings = quality;
    }

    public int GetVideoSettings()
    {
        return gameData.videoSettings;        
    }

    public void SetMusicVolumeSettings(float value)
    {
        gameData.musicVolumeSettings = value;
    }

    public float GetMusicVolumeSettings()
    {
        return gameData.musicVolumeSettings;
    }

    public void SetSFXVolumeSettings(float value)
    {
        gameData.sfxVolumeSettings = value;
    }

    public float GetSFXVolumeSettings()
    {
        return gameData.sfxVolumeSettings;
    }

    public void AddScore(long value)
    {
        gameData.scoreValue += value;
    }

    public long GetScore()
	{
		return gameData.scoreValue;
	}


	public long GetExp()
	{
		return gameData.expValue;
	}

	public void AddExp(long value)
	{
		gameData.expValue += value;

		foreach(KeyValuePair<string, string> level in XMLGameDataReader.GetLevelsExpPoints())
		{
			if (gameData.expValue >= int.Parse (level.Value)) {
				SetPlayerLevel (int.Parse (level.Key));
                
			}
		}
	}

	public long GetGold()
	{
		return gameData.goldValue;
	}

	public void AddGold(long value)
	{
		gameData.goldValue += value;
        CheckGoldAchievement();
	}

	public void SetGold(long value)
	{
		gameData.goldValue = value;
        CheckGoldAchievement();
    }
		
	public int GetPlayerLevel()
	{
		return gameData.currentLevel;
	}

	public void SetPlayerLevel(int level)
	{
		gameData.currentLevel = level;
        /*if (level >= 10)
            SteamStatsAndAchievements.Instance.UnlockAchievement(SteamStatsAndAchievements.ACHIEVEMENT_EXP_LEVEL_10);
        else if(level >= 20)
            SteamStatsAndAchievements.Instance.UnlockAchievement(SteamStatsAndAchievements.ACHIEVEMENT_EXP_LEVEL_20);
        else if (level >= 30)
            SteamStatsAndAchievements.Instance.UnlockAchievement(SteamStatsAndAchievements.ACHIEVEMENT_EXP_LEVEL_30);*/

    }

	public bool WasGameRated()
	{
		return gameData.wasGameRated;
	}

	public void SetGameRated()
	{
		gameData.wasGameRated = true;
	}

	public int GetGameLevelStars(string id)
	{
		if (gameData.gameLevels.ContainsKey (id)) {
			return gameData.gameLevels [id];
		} else {
			gameData.gameLevels.Add (id, -1);
			SaveGameData ();
			return -1;
		}
	}
	public void SetGameLevelStars(string id, int stars)
	{
		if (gameData.gameLevels.ContainsKey (id))
		{
			if(gameData.gameLevels [id] < stars)
				gameData.gameLevels [id] = stars;
		} else {
			gameData.gameLevels.Add (id, stars);
		}
	}

	public bool GetWorldStatusComplete(int id)
	{
		if (gameData.gameWorldsCompleteStatus.ContainsKey (id)) {
			return gameData.gameWorldsCompleteStatus [id];
		} else {
			gameData.gameWorldsCompleteStatus.Add (id, false);
			SaveGameData ();
			return false;
		}
	}
	public void SetWorldStatusComplete(int id, bool unlocked)
	{
		if (gameData.gameWorldsCompleteStatus.ContainsKey (id)) {
			gameData.gameWorldsCompleteStatus [id] = unlocked;
		} else {
			gameData.gameWorldsCompleteStatus.Add (id, unlocked);
		}
	}

	public void AddWorldStatusComplete(int id)
	{
		if (gameData.gameWorldsCompleteStatus.ContainsKey (id) == false) {
			gameData.gameWorldsCompleteStatus [id] = false;
		} 
	}

    private void CheckGoldAchievement()
    {
        /*if (gameData.goldValue >= 10000)
            SteamStatsAndAchievements.Instance.UnlockAchievement(SteamStatsAndAchievements.ACHIEVEMENT_COLLECT_GOLD_10000);
        else if (gameData.goldValue >= 20000)
            SteamStatsAndAchievements.Instance.UnlockAchievement(SteamStatsAndAchievements.ACHIEVEMENT_COLLECT_GOLD_20000);
        else if (gameData.goldValue >= 40000)
            SteamStatsAndAchievements.Instance.UnlockAchievement(SteamStatsAndAchievements.ACHIEVEMENT_COLLECT_GOLD_40000);
        else if (gameData.goldValue >= 80000)
            SteamStatsAndAchievements.Instance.UnlockAchievement(SteamStatsAndAchievements.ACHIEVEMENT_COLLECT_GOLD_80000);
        else if (gameData.goldValue >= 100000)
            SteamStatsAndAchievements.Instance.UnlockAchievement(SteamStatsAndAchievements.ACHIEVEMENT_COLLECT_GOLD_100000);*/
    }
	// // // // // // // // // // // // // // // // // // // // // // // // // // // // // // //
	// // // // // // // // // Krypto // // // // // // // // // // // // // // // // // // // //

	static string GetSha( string text )
	{
		System.Security.Cryptography.SHA1 sha1 = System.Security.Cryptography.SHA1.Create();
		byte[] bytes = System.Text.Encoding.ASCII.GetBytes(text);
		byte[] hash = sha1.ComputeHash(bytes);

		StringBuilder sb = new StringBuilder();
		for (int i = 0; i < hash.Length; i++)
			sb.Append(hash[i].ToString("x2"));

		return sb.ToString();
	}

	public static byte[] Encrypt(byte[] plainTextBytes)
	{
		var symmetricKey = new RijndaelManaged() { Mode = CipherMode.CBC, Padding = PaddingMode.Zeros };
		var encryptor = symmetricKey.CreateEncryptor(keyBytes, Encoding.ASCII.GetBytes(VIKey));

		byte[] cipherTextBytes;

		using (var memoryStream = new MemoryStream())
		{
			using (var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
			{
				cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
				cryptoStream.FlushFinalBlock();
				cipherTextBytes = memoryStream.ToArray();
				cryptoStream.Close();
			}
			memoryStream.Close();
		}
		return cipherTextBytes;
	}

	public static byte[] Decrypt(byte[] cipherTextBytes)
	{
		var symmetricKey = new RijndaelManaged() { Mode = CipherMode.CBC, Padding = PaddingMode.Zeros };
		var decryptor = symmetricKey.CreateDecryptor(keyBytes, Encoding.ASCII.GetBytes(VIKey));

		var memoryStream = new MemoryStream(cipherTextBytes);
		var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);
		byte[] plainTextBytes = new byte[cipherTextBytes.Length];

		int decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);

		memoryStream.Close();
		cryptoStream.Close();
		Array.Resize(ref plainTextBytes, decryptedByteCount);
		return plainTextBytes;
	}
}
	
// // // // // // // // // // // // // // // // // // // // // // // // // // // // // // // // // // // // // // // // // // // // // // // //
[Serializable]
public class SerializableGameData
{
	public long scoreValue;
	public bool wasGameRated = false;
	public Dictionary<string, int> itemLevels;
	public Dictionary<string, bool> tutorialFinished; //klucz - nazwa kodowa etapu tutoriala, wartosc - czy skonczony
	public int currentLevel = 1;
	public long goldValue;
	public long expValue;
	public Dictionary<string, int> gameLevels;
	public Dictionary<int, bool> gameWorldsCompleteStatus;
    public int videoSettings = 2;
    public float musicVolumeSettings= .6f;
    public float sfxVolumeSettings = .6f;
    public SerializableGameData()
	{		
		scoreValue = 0;
		goldValue = XMLGameDataReader.GetStartGoldValue();
		expValue = 0;
		wasGameRated = false;
		tutorialFinished = new Dictionary<string, bool>();
		itemLevels = new Dictionary<string, int>();
        gameLevels = new Dictionary<string, int>();
		gameLevels.Add ("1_1", 0); // odblokuj domyślnie pierwszy level

		gameWorldsCompleteStatus = new Dictionary<int, bool> ();
		gameWorldsCompleteStatus.Add (1, true);// odblokuj domyślnie pierwszy świat
	}
}
