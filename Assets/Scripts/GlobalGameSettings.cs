using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GlobalGameSettings 
{
	private static int selectedLandNumber = -1;
	private static int selectedStageNumber = -1;

	public static int SelectedLandNumber
	{
		get
		{
			return GlobalGameSettings.selectedLandNumber;
		}
		set
		{
			GlobalGameSettings.selectedLandNumber = value;
		}
	}

	public static int SelectedStageNumber
	{
		get
		{
			return GlobalGameSettings.selectedStageNumber;
		}
		set
		{
			GlobalGameSettings.selectedStageNumber = value;
		}
	}
}
