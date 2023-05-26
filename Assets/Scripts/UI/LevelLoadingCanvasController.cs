using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class LevelLoadingCanvasController : MonoBehaviour 
{
	[SerializeField]
	private Text loadingText;
	[SerializeField]
	private UIAlphaAnimator alphaAnimator;
	[SerializeField]
	private Image loadingBar;

    private string currentLoadingText;
	private int currentLevelIndexToLoad;
	private string currentLevelNameToLoad;

	private AsyncOperation loadingOperation = null;
    private AsyncOperation loadingOperationSecond = null;
    private string previousSceneName;

    private GameObject loadingCamera;

    // Use this for initialization

    void Awake () 
	{
		DontDestroyOnLoad(this.gameObject);
        
		Application.backgroundLoadingPriority = ThreadPriority.High;
	}

	void LateUpdate()
	{
		if(loadingOperation != null)
		{
			loadingBar.fillAmount = loadingOperation.progress;
		}
        if (Camera.main != null)
        {
            this.transform.position = Camera.main.transform.position + Camera.main.transform.forward * 2f;
            this.transform.forward = Camera.main.transform.forward;
        }
	}

	public void InitLevelLoading(int levelIndexToLoad = -1, string levelNameToLoad = "", string loadingText = "Loading...")
	{
		this.currentLevelIndexToLoad = levelIndexToLoad;
		this.currentLevelNameToLoad = levelNameToLoad;
		this.currentLoadingText = loadingText;

		StartCoroutine(FadeInAnimation(SwitchToLoadScene));
	}

	private IEnumerator FadeInAnimation(System.Action OnFadeFinished = null)
	{
		alphaAnimator.StopAnimation();
		alphaAnimator.valueFrom = 0f;
		alphaAnimator.valueTo = 1f;
		alphaAnimator.PrepareAnimation();
		alphaAnimator.StartAnimation();

		float currentTime = 0f;
		float animTime = alphaAnimator.timeInSeconds;
		int charCount = currentLoadingText.Length;
		float charTime = animTime/(float)charCount;

		while(alphaAnimator.IsPlaying())
		{
			float animFactor = currentTime/animTime;

			int currentCharCount = Mathf.RoundToInt(currentTime/charTime);
			loadingText.text = "- " + currentLoadingText.Substring(0, Mathf.Min(currentCharCount, currentLoadingText.Length)) + " -";

			currentTime += Time.deltaTime;
			yield return null;
		}

		if(OnFadeFinished != null)
			OnFadeFinished.Invoke();
		//StartCoroutine(LoadLoadingScene());
	}

	private void SwitchToLoadScene()
	{
        StartCoroutine(LoadLoadingScene());
    }

	private IEnumerator LoadLoadingScene()
	{
		previousSceneName = SceneManager.GetActiveScene().name;

        this.loadingOperation = SceneManager.LoadSceneAsync("BruteVsMaidenVR_Loading");
        loadingOperation.allowSceneActivation = true;
    

		while(SceneManager.GetActiveScene().name == previousSceneName)
		{
			yield return null;
		}

        //    StartCoroutine(FadeOutAnimation(SwitchToTargetScene));
        yield return new WaitForSeconds(2);
        StartCoroutine(LoadTargetScene());
    }

	private void SwitchToTargetScene()
	{
		StartCoroutine(LoadTargetScene());
	}
	private IEnumerator LoadTargetScene()
	{
		previousSceneName = SceneManager.GetActiveScene().name;

        if (currentLevelIndexToLoad > -1)
        {
            // get day time of loading level
            string levelDayTime = XMLGameDataReader.GetWorldByID(this.currentLevelIndexToLoad).levelList[GlobalGameSettings.SelectedStageNumber - 1].type;
            string sceneGraphics = "BruteVsMaidenVR_level" + this.currentLevelIndexToLoad + "_" + levelDayTime;
            string sceneLogics = "BruteVsMaidenVR_level" + this.currentLevelIndexToLoad + "Main";

            loadingCamera = Instantiate((Resources.Load("LoadingCameraPrefab") as GameObject));
            // load graphics scene
            loadingOperation = SceneManager.LoadSceneAsync(sceneGraphics);
            loadingOperation.allowSceneActivation = false;
           

            // load scene with game logic
            loadingOperationSecond = SceneManager.LoadSceneAsync(sceneLogics, LoadSceneMode.Additive);
            loadingOperationSecond.allowSceneActivation = false;
        }
        else
        {
	        this.loadingOperation = SceneManager.LoadSceneAsync(this.currentLevelNameToLoad);
            loadingOperation.allowSceneActivation = false;
        }


        // check progress of graphics scene
        while (loadingOperation.progress < 0.85f)
        {
            yield return null;
        }
        // wait a while in loading scene
        
        if(loadingOperationSecond != null)
            yield return new WaitForSeconds(2f);
        

        
        // cover screen to black
        alphaAnimator.StopAnimation();
        alphaAnimator.valueFrom = 0f;
        alphaAnimator.valueTo = 1f;
        alphaAnimator.PrepareAnimation();
        alphaAnimator.StartAnimation();
        while (alphaAnimator.IsPlaying())
        {
            yield return null;
        }
        
        // activate scene with graphics


        loadingOperation.allowSceneActivation = true;

        if (loadingOperationSecond != null)
        {

            yield return new WaitForSeconds(2f);
            loadingOperation.allowSceneActivation = true;
            // check progress of scene with game logic
            while (loadingOperationSecond.progress < 0.89f)
            {
                yield return null;
            }
            //  // fade out game after delay
            yield return new WaitForSeconds(2f);
            Destroy(loadingCamera);
            loadingOperationSecond.allowSceneActivation = true;
            StartCoroutine(FadeOutAnimation(DestroyAfterDelay));
        }
        else
        {
          //  yield return new WaitForSeconds(2f);
            StartCoroutine(FadeOutAnimation(DestroyAfterDelay));
        }

    }

    private void ActivateTargetScene()
	{
		StartCoroutine(WaitForSceneActivation());
	}
	IEnumerator WaitForSceneActivation()
	{
		loadingOperation.allowSceneActivation = true;

      
        while (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == previousSceneName)
		{
			yield return null;
		}

		yield return new WaitForSeconds(4f);

		StartCoroutine(FadeOutAnimation(DestroyAfterDelay));
	}

	private IEnumerator FadeOutAnimation(System.Action OnFadeFinished = null)
	{

        Debug.Log("@@ fade out");
        alphaAnimator.StopAnimation();
		alphaAnimator.valueFrom = 1f;
		alphaAnimator.valueTo = 0f;
		alphaAnimator.PrepareAnimation();
		alphaAnimator.StartAnimation();

		while(alphaAnimator.IsPlaying())
		{
			yield return null;
		}

		if(OnFadeFinished != null)
			OnFadeFinished.Invoke();
	}

	private void DestroyAfterDelay()
	{
		Destroy(this.gameObject, alphaAnimator.timeInSeconds + 0.2f);
	}
}
