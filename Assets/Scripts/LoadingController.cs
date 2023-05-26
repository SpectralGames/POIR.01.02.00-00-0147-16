using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingController : MonoBehaviour 
{	
	public Animator introAnimator;
	public GameObject skipButton;
	private AsyncOperation gameLevelLoader;
	private bool levelLoaded;
	private bool isSceneChanging = false;
	private bool isIntroFinish = false;

	void Start()
	{
		gameLevelLoader = SceneManager.LoadSceneAsync ("BruteVsMaidenVR_MainMenu");
		gameLevelLoader.allowSceneActivation = false;
		StartCoroutine (LoadingScene());
	}
		
	void Update ()
	{
		if (isSceneChanging == false && levelLoaded && isIntroFinish) 
		{
			ChangeScene ();
		}
	}

	public void ChangeScene()
	{
		if (isSceneChanging == false) {
			isSceneChanging = true;
			gameLevelLoader.allowSceneActivation = true;

		}
	}

	public void OnIntroFinish()
	{
		isIntroFinish = true;
	}

	public void OnSkipButtonClicked()
	{
	//	skipButton.gameObject.SetActive(false);
		isIntroFinish = true;
		ChangeScene ();
		//skipPressed = true;
	}
		
	IEnumerator LoadingScene()
	{
		Debug.Log("@@ LoadingScene");
		while(gameLevelLoader.progress < 0.899f) //czekaj na zaladowanie levelu
		{
			Debug.Log ("@@ Progress:" + gameLevelLoader.progress);
			yield return null;
		}
		levelLoaded = true;
	}
}
