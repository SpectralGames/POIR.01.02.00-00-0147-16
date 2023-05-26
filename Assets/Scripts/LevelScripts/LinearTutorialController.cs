using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.EventSystems;
using System.Text;

[System.Serializable]
public class TutorialStageInfo
{
    public string tutorialCodeName;
    //[TextArea(0, 2)]
    public string tutorialHintText;
}

public class LinearTutorialController : MonoBehaviour
{
    public GameObject tutorialPanel;
    public TutorialStageInfo[] tutorialStages;
    public bool DEBUG_MODE = true;
    //public bool isItPartOfMainTutorial = false;
    private int currentTutorialIndex;
    private Transform currentActiveInfoWindow;
    private Transform centerWindow, leftWindow, rightWindow, lawrenceWindow, emilyWindow, ruleWindow, ululaWindow;
    private Transform pointer, arrow;
    private bool isPointerAnimating;
    private string currentWaitingForWarriorType;

    private Button[] currentBlockedButtonList = null;
    private List<Button> waitForButtonList;
    private List<Button3d> waitFor3dButtonList;
    private EventTrigger.Entry currentWaitForTriggerEntry;
    private string sceneName;
    private Coroutine currentWindowAnimation;
    private Transform currentWindowAnimating;


    // Use this for initialization
    public virtual void Awake()
    {
#if !UNITY_EDITOR //na urzadzeniu nigdy nie ustawiaj debug mode
		DEBUG_MODE = false;
#endif
        waitForButtonList = new List<Button>();
        waitFor3dButtonList = new List<Button3d>();
        pointer = tutorialPanel.transform.Find("Pointer");
        arrow = tutorialPanel.transform.Find("Arrow");
        centerWindow = tutorialPanel.transform.Find("CenterWindow");
        leftWindow = tutorialPanel.transform.Find("ALeftWindow");
        rightWindow = tutorialPanel.transform.Find("BRightWindow");
        ruleWindow = tutorialPanel.transform.Find("RuleWindow");
        sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
    }

    public virtual void Start()
    {
        //if(isItPartOfMainTutorial && SaveGameController.instance.IsMainTutorialFinished()) return; //wyjdz, jesli jest czescia glownego tuta (cheat)
        this.OnInit();
        this.OnMoveToTheNextTutorialStage(0.05f);
    }

    protected virtual void OnInit()
    {
        currentTutorialIndex = -1;
        isPointerAnimating = false;
    }

    private bool OnFindCurrentTutorialIndex()
    {
        for (int i = 0; i < tutorialStages.Length; i++)
        {
            if (SaveGameController.instance.IsTutorialFinished(tutorialStages[i].tutorialCodeName + sceneName + this.gameObject.name + i.ToString()) == false)
            {
                Debug.Log("znalazlem tutorial nieskonczony: " + i.ToString());
                currentTutorialIndex = i;
                return true;
            }
        }
        return false;
    }

    IEnumerator OnStartTutorialStageDelayed(float delay, TutorialStageInfo stage)
    {
        tutorialPanel.transform.Find("BlockRaycast").gameObject.SetActive(true); //zablokuj klikniecia pomiedzy kolejnymi krokami tutoriala
        yield return null; //odczekaj jedna klatke na wszelki wypadek
        if (delay > 0f) yield return new WaitForSeconds(delay);
        this.OnStartTutorialStage(stage);
    }

    protected virtual void OnStartTutorialStage(TutorialStageInfo stage)
    {
        Debug.Log("Start Tutorial Stage: " + currentTutorialIndex + " codeName: " + stage.tutorialCodeName);
        string[] codeNameExploded = stage.tutorialCodeName.Split("_"[0]);

        this.OnPrepareTutorialStage(codeNameExploded, stage.tutorialHintText);
    }

    protected virtual void OnFinishCurrentTutorialStage(bool savePreviousStages = false, bool hideInfoWindow = true, int saveNextStages = -1)
    {
        if (savePreviousStages == true && DEBUG_MODE == false)
        {
            Debug.Log("Saving tutorial stages...");
            if (saveNextStages < 0) //nie zapisuje do konkretnego indeksu
            {
                for (int i = 0; i <= currentTutorialIndex; i++)
                    SaveGameController.instance.SetTutorialFinished(tutorialStages[i].tutorialCodeName + sceneName + this.gameObject.name + i.ToString());
            }
            else
            { //zapisz do konkretnego indeksu ignorujac aktualny
                for (int i = 0; i <= currentTutorialIndex + saveNextStages; i++)
                    SaveGameController.instance.SetTutorialFinished(tutorialStages[i].tutorialCodeName + sceneName + this.gameObject.name + i.ToString());
            }
            SaveGameController.instance.SaveGameData();
        }
        if (hideInfoWindow == true)
        {
            AnimateInfoWindow(currentActiveInfoWindow, -1f);
        }
        isPointerAnimating = false;
    }

    protected virtual void OnPrepareTutorialStage(string[] codeNameExploded, string hintText)
    {

        if (codeNameExploded[codeNameExploded.Length - 1][0] == 'C')
            currentActiveInfoWindow = centerWindow;
        else if (codeNameExploded[codeNameExploded.Length - 1][0] == 'A')
            currentActiveInfoWindow = leftWindow;
        else if (codeNameExploded[codeNameExploded.Length - 1][0] == 'B')
            currentActiveInfoWindow = rightWindow;
        else if (codeNameExploded[codeNameExploded.Length - 1][0] == 'R')
            currentActiveInfoWindow = ruleWindow;
        else
            currentActiveInfoWindow = null;

        pointer.gameObject.SetActive(false); arrow.gameObject.SetActive(false);
        tutorialPanel.transform.Find("BlockRaycast").gameObject.SetActive(false);
        if (currentActiveInfoWindow != null) currentActiveInfoWindow.transform.Find("OkButton").gameObject.SetActive(false);
        isPointerAnimating = false;

        switch (codeNameExploded[0]) //sprawdz typ etapu
        {
            case "info":
                tutorialPanel.transform.Find("BlockRaycast").gameObject.SetActive(true);
                if (codeNameExploded.Length > 2) this.OnSetGamePaused(true);
                currentActiveInfoWindow.transform.Find("OkButton").gameObject.SetActive(true);
                currentActiveInfoWindow.transform.Find("OkButton").GetComponent<Button3d>().clickEvent.AddListener(() => OnOkButtonClicked(this));
                AnimateInfoWindow(currentActiveInfoWindow, 1f);
                break;
            case "info2":
                tutorialPanel.transform.Find("BlockRaycast").gameObject.SetActive(true);
                if (codeNameExploded.Length > 2) this.OnSetGamePaused(false);
                AnimateInfoWindow(currentActiveInfoWindow, 1f);
                break;
            case "save": //koniec transakcji
                if (codeNameExploded.Length > 2)
                    this.OnFinishCurrentTutorialStage(true, false, int.Parse(codeNameExploded[1]));
                else
                    this.OnFinishCurrentTutorialStage(true, false);
                this.OnMoveToTheNextTutorialStage(0.05f);
                break;
            case "stop": //zatrzymaj tutorial
                break;
            case "jump": //przeskocz o x krokow tutoriala
                this.OnJumpToTutorialStage(int.Parse(codeNameExploded[1]));
                break;
            case "hide":
                string[] objectsToHide = new string[codeNameExploded.Length - 2];
                System.Array.Copy(codeNameExploded, 1, objectsToHide, 0, codeNameExploded.Length - 2); //skopiuj wszystkie obiekty, pomin 0 i ostatni element tablicy
                this.HideOrShowSomeObjects(objectsToHide, false);
                this.OnMoveToTheNextTutorialStage(0.05f);
                break;
            case "show":
                string[] objectsToShow = new string[codeNameExploded.Length - 2];
                System.Array.Copy(codeNameExploded, 1, objectsToShow, 0, codeNameExploded.Length - 2); //skopiuj wszystkie obiekty, pomin 0 i ostatni element tablicy
                this.HideOrShowSomeObjects(objectsToShow, true);
                this.OnMoveToTheNextTutorialStage(0.05f);
                break;
            case "playanimation":
                this.PlayAnimationOnSomeObject(codeNameExploded[1], codeNameExploded[2]);   //obiekt z animatorem, nazwa animacji
                this.OnMoveToTheNextTutorialStage(0.05f);
                break;
            case "waitforbuttonworld":
                string[] waitForButtonWorldNamesList = new string[codeNameExploded.Length - 2];
                System.Array.Copy(codeNameExploded, 1, waitForButtonWorldNamesList, 0, codeNameExploded.Length - 2); //skopiuj wszystkie obiekty, pomin 0 i ostatni element tablicy
                this.OnSetInteractableOnButtons(false, waitForButtonWorldNamesList);
                this.OnWaitForSomeButtons(waitForButtonWorldNamesList);
                AnimateInfoWindow(currentActiveInfoWindow, 1f);
                this.OnSetGamePaused(true);
                break;
            case "waitforbutton":
                string[] waitForButtonNamesList = new string[codeNameExploded.Length - 2];
                System.Array.Copy(codeNameExploded, 1, waitForButtonNamesList, 0, codeNameExploded.Length - 2); //skopiuj wszystkie obiekty, pomin 0 i ostatni element tablicy
                this.OnSetInteractableOnButtons(false, waitForButtonNamesList);
                this.OnWaitForSomeButtons(waitForButtonNamesList);
                tutorialPanel.transform.Find("BlockLevelRaycast").gameObject.SetActive(true);
                AnimateInfoWindow(currentActiveInfoWindow, 1f);
                this.OnSetGamePaused(true);
                break;
            case "waitfor3dbutton":
                string[] waitFor3dButtonNamesList = new string[codeNameExploded.Length - 2];
                System.Array.Copy(codeNameExploded, 1, waitFor3dButtonNamesList, 0, codeNameExploded.Length - 2); //skopiuj wszystkie obiekty, pomin 0 i ostatni element tablicy
                                                                                                                  //this.OnSetInteractableOnButtons(false, waitFor3dButtonNamesList);
                currentActiveInfoWindow.transform.Find("OkButton").gameObject.SetActive(waitFor3dButtonNamesList[0] == "OkButton");
                StartCoroutine(OnWaitForSome3dButtons(waitFor3dButtonNamesList));
                AnimateInfoWindow(currentActiveInfoWindow, 1f);
                //this.OnSetGamePaused(true);
                break;
            case "waitforeventtrigger":
                this.OnSetInteractableOnButtons(false);
                this.OnWaitForTriggerEvent(codeNameExploded[1]);
                tutorialPanel.transform.Find("BlockLevelRaycast").gameObject.SetActive(true);
                AnimateInfoWindow(currentActiveInfoWindow, 1f);
                this.OnSetGamePaused(true);
                break;
            case "waitforcurrentenemydeath":
                //this.OnSetInteractableOnButtons(false);
                this.OnWaitForCurrentEnemyDeath();
                //AnimateInfoWindow(currentActiveInfoWindow, 1f);
                //this.OnSetGamePaused(true);
                AnimateInfoWindow(currentActiveInfoWindow, 1f);
                break;
            case "waitforsummoningwarrior":
                this.OnWaitForSummoningWarrior(codeNameExploded[1]);
                //this.OnPointAtSomething("MaidenSpawnPivot");
                AnimateInfoWindow(currentActiveInfoWindow, 1f);
                break;
            case "point":
                tutorialPanel.transform.Find("BlockRaycast").gameObject.SetActive(true);
                this.OnPointAtSomething(codeNameExploded[1]);
                currentActiveInfoWindow.transform.Find("OkButton").gameObject.SetActive(true);
                currentActiveInfoWindow.transform.Find("OkButton").GetComponent<Button3d>().clickEvent.AddListener(() => OnOkButtonClicked(this));
                AnimateInfoWindow(currentActiveInfoWindow, 1f);
                this.OnSetGamePaused(true);
                break;
            case "delay":
                this.OnMoveToTheNextTutorialStage(float.Parse(codeNameExploded[1]));
                break;
            case "addgold":
                this.AddGold(int.Parse(codeNameExploded[1]));
                this.OnMoveToTheNextTutorialStage(0.05f);
                break;
            case "sendevent":
                if (codeNameExploded.Length > 3)
                    this.SendEventToSomeObject(codeNameExploded[1], codeNameExploded[2], codeNameExploded[3]);
                else
                    this.SendEventToSomeObject(codeNameExploded[1], codeNameExploded[2]);
                this.OnMoveToTheNextTutorialStage(0.05f);
                break;
            case "switchlevel":
                this.OnSwitchLevel(codeNameExploded[1]);
                this.OnMoveToTheNextTutorialStage(0.05f);
                break;
            case "unparent":
                this.OnUnparentSomeObject(codeNameExploded[1]);
                this.OnMoveToTheNextTutorialStage(0.05f);
                break;
            case "buttonclick":
                this.OnClickSomeButton(codeNameExploded[1]);
                this.OnMoveToTheNextTutorialStage(0.05f);
                break;
            case "destroysummon":
                this.GetComponent<MainMenuTutorialController>().DestroyCurrentTower();
                this.OnMoveToTheNextTutorialStage(0.05f);
                break;
            case "waitforteleport":
                var tower =  GameObject.Find(codeNameExploded[1]);
                var towerController = tower.GetComponent<TowerTeleport>();
                StartCoroutine(WaitForTeleport(towerController));
                break;
            default:
                pointer.gameObject.SetActive(true);
                if (currentActiveInfoWindow != null) currentActiveInfoWindow.transform.Find("OkButton").gameObject.SetActive(false);
                AnimateInfoWindow(currentActiveInfoWindow, 1f);
                break;
        }
        //if(currentActiveInfoWindow != null) Debug.Log("aktywne okno jest " + currentTutorialIndex + " hint " + hintText);
        if (currentActiveInfoWindow != null) currentActiveInfoWindow.transform.Find("TutorialText").GetComponent<Text>().text = hintText.Replace("<br>", "\n");// LanguageManager.Instance.GetTextValue(hintText);
    }

    private IEnumerator WaitForTeleport(TowerTeleport towerController)
    {
        if(towerController != null)
        {
            bool isTeleported = false;
            towerController.OnTeleport.AddListener(() => isTeleported = true);
            yield return new WaitUntil(() => isTeleported);
            towerController.OnTeleport.RemoveAllListeners();
            OnMoveToTheNextTutorialStage(0.05f);
        }
        else
            yield return null;
    }

    private void HideOrShowSomeObjects(string[] objectNames, bool bShow)
    {
        /*if(bShow == false)
		{
			for(int i=0; i<objectNames.Length; i++)
			{
				GameObject sceneGO = GameObject.Find(objectNames[i]);
				if(sceneGO != null) sceneGO.SetActive(bShow);
			}
		}else{ */


        //  GameObject[] rootObjects = UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects();
        List<GameObject> rootObjects = new List<GameObject>();
        for (int i =0;i < UnityEngine.SceneManagement.SceneManager.sceneCount;i++ )
        {
            rootObjects.AddRange(UnityEngine.SceneManagement.SceneManager.GetSceneAt(i).GetRootGameObjects());
        }

        for (int i = 0; i < rootObjects.Count; i++)
        {
            for (int j = 0; j < objectNames.Length; j++)
            {
                if (string.Compare(rootObjects[i].name, objectNames[j]) == 0)
                    rootObjects[i].SetActive(bShow);
            }
            if (rootObjects[i].transform.childCount > 0)// && rootObjects[i].activeSelf == true)       
                this.ActivateTransformInChild(rootObjects[i].transform, objectNames, bShow);
            
        }
        //}
    }

    private void HideSomeObject(string objectName)
    {
        this.HideOrShowSomeObjects(new string[] { objectName }, false);
    }

    private void ActivateTransformInChild(Transform rootTransform, string[] objectNames, bool bShow)
    {
        for (int i = 0; i < rootTransform.childCount; i++)
        {
            for (int j = 0; j < objectNames.Length; j++)
            {
                if (string.Compare(rootTransform.GetChild(i).name, objectNames[j]) == 0)
                    rootTransform.GetChild(i).gameObject.SetActive(bShow);
            }
            if (rootTransform.GetChild(i).childCount > 0)// && rootTransform.GetChild(i).gameObject.activeSelf == true)
                ActivateTransformInChild(rootTransform.GetChild(i), objectNames, bShow); //uruchom rekurencje na wszystkich childach
        }
    }

    private void PlayAnimationOnSomeObject(string objectName, string animName)
    {
        GameObject sceneGO = GameObject.Find(objectName);
        if (sceneGO != null)
            sceneGO.GetComponent<Animator>().Play(animName);
    }

    private void SendEventToSomeObject(string objectName, string eventName, string param = "")
    {
        GameObject sceneGO = GameObject.Find(objectName);
        if (sceneGO != null)
            sceneGO.SendMessage(eventName, param);
    }

    private void AddGold(int count)
    {
        //SaveGameController.instance.SaveGoldValue(SaveGameController.instance.GetGoldValue() + count);
    }

    private void OnUnparentSomeObject(string pathToTheObject)
    {
        GameObject[] rootObjects = UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects();
        for (int i = 0; i < rootObjects.Length; i++)
        {
            string[] pathSplit = pathToTheObject.Split("/"[0]);
            if (string.Compare(rootObjects[i].gameObject.name, pathSplit[0]) == 0)
            {
                rootObjects[i].transform.Find(pathToTheObject.Replace(pathSplit[0] + "/", "")).SetParent(null);
            }
        }
    }

    private void OnSetInteractableOnButtons(bool isInteractable, string[] buttonsForExclusion = null)
    {
        if (currentBlockedButtonList == null)
            currentBlockedButtonList = GameObject.FindObjectsOfType<Button>();
        try
        {
            //Button[] allButtons = GameObject.FindObjectsOfType<Button>();
            for (int i = 0; i < currentBlockedButtonList.Length; i++)
            {
                if (buttonsForExclusion != null)
                {
                    bool currentButtonIsInTheExclusionArray = false;
                    for (int j = 0; j < buttonsForExclusion.Length; j++)
                    {
                        if (string.Compare(currentBlockedButtonList[i].gameObject.name, buttonsForExclusion[j]) == 0) //to jest guzik, ktorego nie chcemy ruszac
                        {
                            currentButtonIsInTheExclusionArray = true;
                            break;
                        }
                    }
                    // zmien stan tylko wtedy, gdy aktualny guzik nie nalezy do tablicy buttonsForExclusion
                    if (currentButtonIsInTheExclusionArray == false) { currentBlockedButtonList[i].interactable = isInteractable; }
                }
                else
                {
                    currentBlockedButtonList[i].interactable = isInteractable;
                }
            }
            if (isInteractable == true) currentBlockedButtonList = null;
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
        }
    }

    private void OnWaitForSummoningWarrior(string warriorType)
    {
        currentWaitingForWarriorType = warriorType;
        this.GetComponent<MainMenuTutorialController>().OnSummoningWarriorCompleted += OnSummoningWarriorCompleted;
    }
    private void OnSummoningWarriorCompleted(string warriorType)
    {
        if (warriorType == currentWaitingForWarriorType)
        {
            this.GetComponent<MainMenuTutorialController>().OnSummoningWarriorCompleted -= OnSummoningWarriorCompleted;
            this.OnFinishCurrentTutorialStage();
            this.OnMoveToTheNextTutorialStage(0.05f);
        }
    }


    private void OnWaitForCurrentEnemyDeath()
    {
        this.GetComponent<MainMenuTutorialController>().OnCurrentEnemyDiedCompleted += OnCurrentEnemyDiedCompleted;
    }

    private void OnCurrentEnemyDiedCompleted()
    {
        //this.OnSetGamePaused(false);
        //this.OnSetInteractableOnButtons(true);
        this.GetComponent<MainMenuTutorialController>().OnCurrentEnemyDiedCompleted -= OnCurrentEnemyDiedCompleted;
        this.OnFinishCurrentTutorialStage();
        this.OnMoveToTheNextTutorialStage(0.05f);
    }


    private void OnWaitForSomeButtons(string[] buttonNames)
    {
        waitForButtonList.Clear();
        Button[] allButtons = GameObject.FindObjectsOfType<Button>();
        for (int i = 0; i < allButtons.Length; i++)
        {
            for (int j = 0; j < buttonNames.Length; j++)
            {
                if (string.Compare(allButtons[i].gameObject.name, buttonNames[j]) == 0)
                {
                    allButtons[i].onClick.AddListener(OnSomeButtonClicked);
                    waitForButtonList.Add(allButtons[i]);
                    if (buttonNames.Length == 1) //dla jednego, odpal animacje pointera
                    {
                        StartCoroutine(OnAnimatePointer(allButtons[i].gameObject, pointer));
                        return;
                    }
                    break;
                }
            }
        }
    }

    private IEnumerator OnWaitForSome3dButtons(string[] buttonNames)
    {
        yield return null;
        waitFor3dButtonList.Clear();
        Button3d[] allButtons = GameObject.FindObjectsOfType<Button3d>();
        for (int i = 0; i < allButtons.Length; i++)
        {
            for (int j = 0; j < buttonNames.Length; j++)
            {
                if (allButtons[i].gameObject.name == buttonNames[j])
                {
                    allButtons[i].clickEvent.AddListener(OnSomeButton3dClicked);
                    waitFor3dButtonList.Add(allButtons[i]);
                    if (buttonNames.Length == 1) //dla jednego, odpal animacje pointera
                    {
                        StartCoroutine(OnAnimatePointer(allButtons[i].gameObject, pointer));
                    }
                    break;
                }
            }
        }
    }


    private void OnWaitForTriggerEvent(string triggerName)
    {
        EventTrigger[] allTriggers = GameObject.FindObjectsOfType<EventTrigger>();
        for (int i = 0; i < allTriggers.Length; i++)
        {
            if (string.Compare(allTriggers[i].gameObject.name, triggerName) == 0)
            {
                for (int j = 0; j < allTriggers[i].triggers.Count; j++)
                {
                    EventTrigger.Entry entry = allTriggers[i].triggers[j];
                    if (entry.eventID == EventTriggerType.PointerDown) //znalazlem triggera z eventem PointerDown, podepnij sie
                    {
                        currentWaitForTriggerEntry = entry; //zapamietaj event
                        entry.callback.AddListener(OnSomeTriggerClicked);
                        StartCoroutine(OnAnimatePointer(allTriggers[i].gameObject, pointer));
                    }
                }
                return;
            }
        }
    }

    private void OnPointAtSomething(string elementName)
    {
        GameObject elementGO = GameObject.Find(elementName);
        if (elementGO != null)
        {
            Debug.Log("OnPointAtSomething: object found");
            StartCoroutine(OnAnimatePointer(elementGO, arrow, true));
        }
    }

    private void OnSwitchLevel(string levelName)
    {
        if (string.Compare(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name, "MainMenu") == 0) //jesli jestem w menu, uzyj mechanizmu
        {
            PlayerPrefs.SetString("ProjectV.GameLevel", "00Tutorial");
        }
        else
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(levelName);
        }
    }

    IEnumerator OnAnimatePointer(GameObject pointerTarget, Transform currentPointer, bool pulsingAnimation = true)
    {
        yield return null;
        isPointerAnimating = true;
        currentPointer.gameObject.SetActive(true);

        RectTransform pointerRect = currentPointer.GetComponent<RectTransform>();
        pointerRect.anchoredPosition = Vector2.zero;
        RectTransform tutorialCanvasRect = tutorialPanel.transform.GetComponent<RectTransform>();
        MeshRenderer pointerTargetMesh = pointerTarget.GetComponent<MeshRenderer>();
        if (pointerTargetMesh == null) pointerTargetMesh = pointerTarget.GetComponentInChildren<MeshRenderer>();
        RectTransform targetRect = pointerTarget.GetComponent<RectTransform>();
        if (targetRect != null && targetRect.GetComponentInParent<Canvas>().renderMode == RenderMode.WorldSpace) //jesli worldspace, traktuj jak normalny obiekt na scenie
            targetRect = null;
        Camera canvasCamera = Camera.main; //tutorialCanvasRect.GetComponent<Canvas>().worldCamera;

        Vector2 initScreenPosition = targetRect != null ? (Vector2)targetRect.position : Vector2.one;
        Vector2 initViewportPosition = targetRect != null ? new Vector2(initScreenPosition.x / tutorialCanvasRect.sizeDelta.x, initScreenPosition.y / tutorialCanvasRect.sizeDelta.y) :
            (Vector2)canvasCamera.WorldToViewportPoint(pointerTargetMesh != null ? pointerTargetMesh.bounds.center : pointerTarget.transform.position);
        float initAngle = 60f; //initViewportPosition.x < 0.5f ? 60f : 0f;
        if (initViewportPosition.y < 0.5f) initAngle = -initAngle - 75f;
        //pointerRect.localEulerAngles = new Vector3(0f, 0f, initAngle); //ustal kat

        float xFactor = initViewportPosition.x < 0.5f ? 7f : -7f; //parametry, ktore kontroluja kierunek animacji pointera w zaleznosci od pozycji na ekranie
        float yFactor = initViewportPosition.y < 0.5f ? 7f : -7f;

        if (pulsingAnimation == false) xFactor = yFactor = 0f;

        while (isPointerAnimating)
        {
            //jesli nie widac targetu, schowaj pointer
            /*Vector3 viewportTargetPosition = Camera.main.WorldToViewportPoint(pointerTarget.transform.position);
			currentPointer.gameObject.SetActive(viewportTargetPosition.x > 0f && viewportTargetPosition.x < 1f && viewportTargetPosition.y > 0f && 
				viewportTargetPosition.y < 1f && viewportTargetPosition.z > 0f); */
            currentPointer.gameObject.SetActive(pointerTarget.gameObject.activeInHierarchy);
            //ustaw pozycje
            if (targetRect != null)
            {
                pointerRect.position = targetRect.position;
                pointerRect.anchoredPosition -= new Vector2(pointerRect.transform.up.x, pointerRect.transform.up.y) * Mathf.Sin(Time.realtimeSinceStartup * xFactor) * 5f;//new Vector2(Mathf.Sin(Time.realtimeSinceStartup*xFactor), Mathf.Sin(Time.realtimeSinceStartup*yFactor))*4f;
            }
            else
            {
                Vector3 worldPosition = pointerTargetMesh != null ? pointerTargetMesh.bounds.center : pointerTarget.transform.position;
                currentPointer.position = worldPosition - currentPointer.transform.forward * 0.1f + currentPointer.transform.forward * Mathf.Sin(Time.realtimeSinceStartup * xFactor) * 0.05f;
            }
            //currentPointer.transform.position = pointerTarget.transform.position;
            yield return null;
        }
        pointerRect.anchoredPosition = new Vector2(-1000f, 0f);
    }

    private void OnClickSomeButton(string buttonName)
    {
        Button[] buttons = GameObject.FindObjectsOfType<Button>();
        for (int i = 0; i < buttons.Length; i++)
        {
            if (buttons[i].name.CompareTo(buttonName) == 0)
            {
                buttons[i].onClick.Invoke();
                return;
            }
        }
        //GameObject sceneButtonGO = GameObject.Find(buttonName);
        //if(sceneButtonGO != null)
        //	sceneButtonGO.GetComponent<Button>().onClick.Invoke();
    }

    private void AnimateInfoWindow(Transform windowToAnimate, float direction)
    {
        if (currentWindowAnimating == windowToAnimate)
            StopCoroutine(currentWindowAnimation);
        currentWindowAnimation = StartCoroutine(OnAnimateInfoWindow(windowToAnimate, direction));
    }

    IEnumerator OnAnimateInfoWindow(Transform windowToAnimate, float direction)
    {
        currentWindowAnimating = windowToAnimate;
        windowToAnimate.gameObject.SetActive(true);
        CanvasGroup activeWindowCanvasGroup = windowToAnimate.GetComponent<CanvasGroup>();
        //float currentTime = 0f, animTime = 0.2f;
        float initXValue = direction > 0f ? 0f : 1f;
        float finalXValue = direction > 0f ? 1f : 0f;
        /*
		do
		{
			currentTime += Time.unscaledDeltaTime;
			float percent = Mathf.Clamp01(currentTime/animTime);
			activeWindowCanvasGroup.alpha = Mathf.Lerp(initXValue, finalXValue, percent);
			//currentActiveInfoWindow.anchoredPosition = new Vector2(Mathf.Lerp(initXValue, finalXValue, percent*percent), currentActiveInfoWindow.anchoredPosition.y);
			yield return null;
		}while(currentTime < animTime);
		*/
        //jesli wyjezdzam z oknem poza ekran, ukryj okno
        if (direction < 0f) windowToAnimate.gameObject.SetActive(false);
        currentWindowAnimating = null;
        yield return null;
    }

    private void OnJumpToTutorialStage(int tutorialSteps)
    {
        currentTutorialIndex += tutorialSteps;
        StartCoroutine(OnStartTutorialStageDelayed(0.05f, tutorialStages[currentTutorialIndex]));
    }

    private void OnMoveToTheNextTutorialStage(float delay = 0.5f)
    {
        if (currentTutorialIndex < 0)
        {
            if (this.OnFindCurrentTutorialIndex()) StartCoroutine(OnStartTutorialStageDelayed(delay, tutorialStages[currentTutorialIndex]));
        }
        else if (currentTutorialIndex < tutorialStages.Length - 1)
        {
            currentTutorialIndex++;
            StartCoroutine(OnStartTutorialStageDelayed(delay, tutorialStages[currentTutorialIndex]));
        }
        else
        {
            // tutorial complete, unlock achievement
            //SteamStatsAndAchievements.Instance.UnlockAchievement(SteamStatsAndAchievements.COMPLETE_TUTORIAL);
        }
    }

    private void OnOkButtonClicked(LinearTutorialController controllerReference)
    {
        if (controllerReference != this) return; //jesli nie jestem odbiorca, nie obsluguj
        currentActiveInfoWindow.transform.Find("OkButton").GetComponent<Button3d>().clickEvent.RemoveAllListeners();
        //    SoundManager.instance.PlayUISoundEffect("button");
        tutorialPanel.transform.Find("BlockRaycast").gameObject.SetActive(false);
        this.OnSetGamePaused(false);
        this.OnFinishCurrentTutorialStage();
        this.OnMoveToTheNextTutorialStage(0.05f);
        //SoundManager.instance.PlayClick0();
    }

    private void OnSetGamePaused(bool bPaused)
    {
        if (bPaused)
            Time.timeScale = 0f;
        else
            Time.timeScale = 1f;
    }

    private void OnSomeButtonClicked()
    {
        this.OnSetGamePaused(false);
        //odepnij sie od listenera
        for (int i = 0; i < waitForButtonList.Count; i++)
        {
            waitForButtonList[i].onClick.RemoveListener(OnSomeButtonClicked);
        }
        waitForButtonList.Clear();
        tutorialPanel.transform.Find("BlockLevelRaycast").gameObject.SetActive(false);
        this.OnSetInteractableOnButtons(true);
        this.OnFinishCurrentTutorialStage();
        this.OnMoveToTheNextTutorialStage(0.05f);
    }

    private void OnSomeButton3dClicked()
    {
        //this.OnSetGamePaused(false);
        //odepnij sie od listenera
        for (int i = 0; i < waitFor3dButtonList.Count; i++)
        {
            waitFor3dButtonList[i].clickEvent.RemoveListener(OnSomeButton3dClicked);
        }
        waitFor3dButtonList.Clear();
        this.OnSetInteractableOnButtons(true);
        this.OnFinishCurrentTutorialStage();
        this.OnMoveToTheNextTutorialStage(0.05f);
    }

    private void OnSomeTriggerClicked(BaseEventData data)
    {
        //odepnij sie od listenera
        currentWaitForTriggerEntry.callback.RemoveListener(OnSomeTriggerClicked);
        tutorialPanel.transform.Find("BlockLevelRaycast").gameObject.SetActive(false);
        this.OnSetGamePaused(false);
        this.OnSetInteractableOnButtons(true);
        this.OnFinishCurrentTutorialStage();
        this.OnMoveToTheNextTutorialStage(0.05f);
    }

    static string GetSha(string text)
    {
        System.Security.Cryptography.SHA1 sha1 = System.Security.Cryptography.SHA1.Create();
        byte[] bytes = System.Text.Encoding.ASCII.GetBytes(text);
        byte[] hash = sha1.ComputeHash(bytes);

        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < hash.Length; i++)
            sb.Append(hash[i].ToString("X2"));

        return sb.ToString();
    }
}
