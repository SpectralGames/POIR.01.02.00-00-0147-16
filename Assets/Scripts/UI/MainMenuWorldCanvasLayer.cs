using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuWorldCanvasLayer : MonoBehaviour 
{
	public GameObject[] tabList;
	private int currentTabSelected;

	void OnEnable()
	{
		currentTabSelected = 0;
		this.SwitchToTab(currentTabSelected);
	}


	public void OnNextTabButtonClicked()
	{
		currentTabSelected = (currentTabSelected+1) % tabList.Length;

		this.SwitchToTab(currentTabSelected);
	}

	public void OnPreviousTabButtonClicked()
	{
		currentTabSelected--;
		if(currentTabSelected < 0)
			currentTabSelected = tabList.Length-1;
		
		this.SwitchToTab(currentTabSelected);
	}

	private void SwitchToTab(int tabNumber)
	{
		for(int i=0; i<tabList.Length; i++)
		{
			tabList[i].SetActive(i == tabNumber);
		}
	}
}
