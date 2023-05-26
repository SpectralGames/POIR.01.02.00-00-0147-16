using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class GlobalHelpers 
{
	public static void LoadLevel(int levelBuildIndex, string loadingText)
	{

		
		GameObject levelLoadingCanvas = GameObject.Instantiate(Resources.Load("UI/LevelLoadingCanvas") as GameObject, null);
		levelLoadingCanvas.GetComponent<LevelLoadingCanvasController>().InitLevelLoading(levelBuildIndex, "", loadingText);
		
	}

	public static void LoadLevel(string levelName, string loadingText)
	{
		GameObject levelLoadingCanvas = GameObject.Instantiate(Resources.Load("UI/LevelLoadingCanvas") as GameObject, null);
		levelLoadingCanvas.GetComponent<LevelLoadingCanvasController>().InitLevelLoading(-1, levelName, loadingText);
	}
}
