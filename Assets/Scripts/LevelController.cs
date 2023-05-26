using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.SceneManagement;

[System.Serializable]
public class WaveGroup
{
    //public EStrategyType strategyType;
    public float initialGroupDelay;
    public int enemyCount;
    public GameObject enemyObject;
    [Header("Spawns")]
    public Pathpoint[] meshPoints;
    public BezierPath[] splinePoints;
    public NavigationType navigationType;
    public float minSpawnDelay;
    public float maxSpawnDelay;
    private int id;
    public int groupId
    {
        get { return id; }
        set { id = value; }
    }

}

[System.Serializable]
public class Wave
{
    public float initialDelay;
    public Enemy newEnemyType = null;
    public WaveGroup[] enemyGroups;
}

[System.Serializable]
public class Level
{
    public Wave[] waves;
    public WaveGroup[] triggeredGroup;
	public GameObject[] objectsToActivate;
	public int initTowerNumber;
	public float timeOfDay;
}

public class LevelController : MonoBehaviour
{
    public static bool DEBUG_MODE = false;
    [SerializeField]
    public int debugLevelNumber = 0, debugWaveNumber = 0;

    private List<EnemyGroup> enemyGroups = new List<EnemyGroup>();

    public Level[] levels;
    public GameObject[] objectsToReset;

    public int EnemiesToEnd = 10;



    private float levelTime, waveTime;
    //public static long runtimeGold;
    [HideInInspector] public int nCurrentLevel, nCurrentWave;
    private List<WaveGroup> currentWaveEnemyGroups;

    private GameObject enemiesParent;
    private bool isEveryEnemyReleased;


    public bool isEveryEnemyDead
    {
        get {
            if (nCurrentWave + 1 == levels[nCurrentLevel].waves.Length && isEveryEnemyReleased == true && GetAliveEnemiesCount() == 0)
                return true;
            else
                return false;
        }
    }

    private GameplayController gameplayController;
    private bool isGameplayActive;
    private int generatedId;
    private int triggeredWaveIndex = 0;
    private float delayBetweenWaves = -1f;
    private List<GameObject> waveGroupIndicators;

    void Awake ()
    {
		int selectedStage = GlobalGameSettings.SelectedStageNumber;
		DEBUG_MODE = selectedStage == -1;
        currentWaveEnemyGroups = new List<WaveGroup>();
        
        if (DEBUG_MODE)
        {
            nCurrentLevel = debugLevelNumber;
        }
        else
        {
			nCurrentLevel = selectedStage-1;
        }

        enemiesParent = new GameObject("Enemies");
        //gameplayController = GameObject.Find("GameplayController").GetComponent<GameplayController>();
        levelTime = waveTime = 0f;
        nCurrentWave = DEBUG_MODE == true ? debugWaveNumber - 1 : -1;
        //nCurrentWave = 0;
        isGameplayActive = false;
        isEveryEnemyReleased = false;

		OnPrepareLevel();
    }

    private void Start ()
    {
		//OnPrepareTimeOfDay();
		//OnPrepareLevel(); przniesione do Awake, W OnPrepareLevel odkrywane są teleporty do których musi mieć dostęp playecontroller już w Awake żeby wywołać init teleport number
    }

	public void OnSetGameplayController(GameplayController gameplayController)
	{
		this.gameplayController = gameplayController;
	}

    // Update is called once per frame
    void Update ()
    {

        levelTime += Time.deltaTime;
        if (isGameplayActive == true)
        {
            //UpdateEnemyGroups();

            waveTime += Time.deltaTime;

            // przebieg fali, odpalanie wrogow
            if (nCurrentWave > -1)
            {
                for (int i = currentWaveEnemyGroups.Count - 1; i >= 0; i--)
                {
                    WaveGroup enemyGroup = currentWaveEnemyGroups[i];
                    if (waveTime > enemyGroup.initialGroupDelay) //przekroczylem delay wypuszczenia grupy wrogow
                    {
                        generatedId++;
                        enemyGroup.groupId = generatedId; // potrzebne w ogole?
                        StartCoroutine(StartSpawningEnemyGroup(enemyGroup, currentWaveEnemyGroups.Count - 1));
                        currentWaveEnemyGroups.RemoveAt(i); // usun grupe, juz obsluzona
                    }
                }
            }

            if (levels[nCurrentLevel].waves.Length > nCurrentWave + 1)
            { //czy sa jeszcze fale w kolejce do wypuszczenia
              //automatyczne odpalanie kolejnej fali po czasie albo w sytuacji gdy nie ma zywych wrogow
                if ((isEveryEnemyReleased == true && GetAliveEnemiesCount() == 0))
                {
                    this.OnWaveFinished();
                    //StartCoroutine ("StartNextWave");
                }
            }

        }
        else
        { //isGameplayActive
            if (delayBetweenWaves > 0f)
            {
                delayBetweenWaves -= Time.deltaTime;
                if (delayBetweenWaves <= 0f)
                    this.OnWaveDelayTimePassed();
            }
        }
    }// UPDATEEE

    private void UpdateEnemyGroups ()
    {
        if (enemyGroups.Count < 0)
            return;

        foreach (EnemyGroup enemy in enemyGroups)
        {
            enemy.HoldTactic();
        }
    }

    public void OnSwitchToNextLevel ()
    {
        if (nCurrentLevel < levels.Length - 1) //jesli jest jeszcze level dostepny, przejdz
        {
            nCurrentLevel++;
        }
    }

    public int CheckHasNextLevel ()
    {
        if (nCurrentLevel < levels.Length - 1)
        { //jesli jest jeszcze level dostepny, przejdz
            return nCurrentLevel + 1;
        }
        else
            return -1;
    }

    public bool IsLastLevel ()
    {
        return nCurrentLevel == levels.Length - 1 ? true : false;
    }

	public void OnStartLevelDelayed(float delayTime)
	{
		StartCoroutine(InvokeAfterDelay(delayTime, OnStartLevel));
	}

	private IEnumerator InvokeAfterDelay(float delayTime, System.Action OnDelayFinished = null)
	{
		yield return new WaitForSeconds(delayTime);

		if(OnDelayFinished != null)
			OnDelayFinished.Invoke();
	}

	private void OnPrepareLevel()
	{
		for (int i = 0; i < objectsToReset.Length; i++)
		{
			if (objectsToReset[i] != null)
				objectsToReset[i].SendMessage("OnRestartLevel", SendMessageOptions.DontRequireReceiver);
		}

		for (int i = 0; i < levels[nCurrentLevel].objectsToActivate.Length; i++)
		{
			GameObject objectToActivate = levels[nCurrentLevel].objectsToActivate[i];
			if(objectToActivate != null)
				objectToActivate.SetActive(true);
		}
	}

	private void OnPrepareTimeOfDay()
	{
		GameObject.FindObjectOfType<DayNightCycle>().SetTimeOfDay(levels[nCurrentLevel].timeOfDay);
	}

    public void OnStartLevel ()
    {
        isGameplayActive = true;
        this.OnStartNextWave();
    }

	public int GetCurrentLevelInitTowerNumber()
	{
		return levels[nCurrentLevel].initTowerNumber;
	}

    public void OnStartNextWave ()
    {
        delayBetweenWaves = -1f;
        isGameplayActive = true;
        StartCoroutine("StartNextWave");
        //DestroyCurrentWaveGroupIndicators();
    }

    public void OnClearLevel ()
    {
        StopCoroutine("StartNextWave");
        StopCoroutine("StartSpawningEnemyGroup");

        isGameplayActive = false;
        levelTime = waveTime = 0f;
        delayBetweenWaves = -1f;
        generatedId = 0;
        enemyGroups.Clear(); // new
        nCurrentWave = DEBUG_MODE == true ? debugWaveNumber - 1 : -1;
        triggeredWaveIndex = 0;
        isEveryEnemyReleased = true;
        currentWaveEnemyGroups.Clear();
		ObjectPool.Instance.ClearData ();

        for (int i = 0; i < enemiesParent.transform.childCount; i++)
        {
            Destroy(enemiesParent.transform.GetChild(i).gameObject);
        }
        //CreateNextWaveGroupIndicators();
    }

    private void OnWaveFinished ()
    {
        Debug.Log("OnWaveFinished " + nCurrentWave);
        isGameplayActive = false;
        delayBetweenWaves = Mathf.Floor(levels[nCurrentLevel].waves[nCurrentWave + 1].initialDelay);
        //gameplayController.OnProceedToUpgradesAfterWave(delayBetweenWaves);
        //CreateNextWaveGroupIndicators();
    }

    private void OnWaveDelayTimePassed ()
    {
        Debug.Log("Delay Passed, Start Next Wave !");
        //gameplayController.OnProceedToGameplayAfterWaveDelay();
        this.OnStartNextWave();
    }

    IEnumerator StartNextWave ()
    {
        waveTime = 0f;
        nCurrentWave++;
        isEveryEnemyReleased = false;
        currentWaveEnemyGroups.Clear();

        yield return new WaitForSeconds(1f);
        // // // // // // // // // // // // // // // //
        //	Debug.Log("start new wave!!");
        waveTime = 0f;
        //zbuduj liste grup przeciwnikow, ktore beda wysylane w nowej fali
        for (int i = 0; i < levels[nCurrentLevel].waves[nCurrentWave].enemyGroups.Length; i++)
            currentWaveEnemyGroups.Add(levels[nCurrentLevel].waves[nCurrentWave].enemyGroups[i]);


        Debug.Log("Level " + nCurrentLevel + " Wave " + nCurrentWave + "         !!!!!!!!!!!!!!!!!!!!!!!!");
    }
    int idInc = 0;
    IEnumerator StartSpawningEnemyGroup (WaveGroup enemyGroup, int enemyGroupCount)
    {

        int enemyCount = enemyGroup.enemyCount;
        float spawnDelay = .5f; //Random.Range(enemyGroup.minSpawnDelay, enemyGroup.maxSpawnDelay);

        List<int> spawnNumbersList = new List<int>();

        int spawnMeshNumber = Random.Range(0, enemyGroup.meshPoints.Length); //wylosuj punkt spawna
        int spawnSplieNumber = Random.Range(0, enemyGroup.splinePoints.Length);
        //GameObject enemyIndicator = Instantiate(Resources.Load("EnemyGroupIndicator", typeof(GameObject))) as GameObject;
        //enemyIndicator.SendMessage("OnSpawn", enemyGroup.spawnPoints[spawnNumber].GetComponent<BezierPath>(), SendMessageOptions.DontRequireReceiver);


        yield return new WaitForSeconds(spawnDelay);

        //if (enemyGroup.meshPoints.Count() > 0 || enemyGroup.splinePoints.Count() > 0)
        //{
            while (enemyCount > 0 && isGameplayActive)
            {
                GameObject enemyClone = SpawnEnemy(enemyGroup, enemyGroup.meshPoints[spawnMeshNumber], enemyGroup.splinePoints[spawnSplieNumber]);
                enemyClone.name = (enemyCount + idInc * 10).ToString();
                Enemy e = enemyClone.GetComponent<Enemy>();
				ObjectPool.Instance.enemyList.Add(e); // ### new ###
		
                // jesli base to kazdy enemy jest swoja tak jakby osobna grupka
                enemyGroups.Add(new EnemyGroup(e));

                enemyCount--;
	
                if (spawnNumbersList.Contains(spawnMeshNumber) == false) //jesli nie spawnowalem jeszcze w tym punkcie, postaw indykator i odznacz punkt
                {
                    spawnNumbersList.Add(spawnMeshNumber);
                }

                yield return new WaitForSeconds(Random.Range(enemyGroup.minSpawnDelay, enemyGroup.maxSpawnDelay));
            }

       // }
	
        idInc++;
        if (enemyGroupCount == 0 && isGameplayActive)
        {
            isEveryEnemyReleased = true; //jesli wypuscilem wszystkich przeciwnikow
        }
    }

    GameObject SpawnEnemy (WaveGroup enemyGroup, Pathpoint pathPoint, BezierPath bezierPath)
    {
        //	Debug.Log (enemy + " " + spawnPoint.transform.position + " " + spawnPoint.transform.rotation);

        Vector3 pos = enemyGroup.navigationType == NavigationType.NavMesh ? pathPoint.transform.position : bezierPath.transform.position;
        Quaternion rot = enemyGroup.navigationType == NavigationType.NavMesh ? pathPoint.transform.rotation : bezierPath.transform.rotation;

        GameObject enemyClone = Instantiate(enemyGroup.enemyObject, pos, rot, enemiesParent.transform) as GameObject;
        Enemy e = enemyClone.GetComponent<Enemy>();

        //e.veryFirstEnemyPosition = spawnPoint.transform.position;
        e.Init(pathPoint, bezierPath, enemyGroup.navigationType);
        //enemyClone.transform.SetParent(enemiesParent.transform); //podepnij pod obiekt enemies
        //enemyClone.SendMessage("OnSpawn", spawnPoint.GetComponent<Pathpoint>(), SendMessageOptions.DontRequireReceiver); // wyjebac sendMessage
        //gameplayController.OnEnemySpawned(enemyClone, enemyGroup.strategyType == EStrategyType.Base ? false : true);
        return enemyClone;
    }

    public int GetAliveEnemiesCount ()
    {
        //return enemiesParent.transform.childCount;
		return ObjectPool.Instance.GetAliveEnemyCount();
    }

    public void ShowTriggeredEnemyGroup ()
    {
        StartCoroutine(StartSpawningEnemyGroup(levels[0].triggeredGroup[0], 0));
    }
}

