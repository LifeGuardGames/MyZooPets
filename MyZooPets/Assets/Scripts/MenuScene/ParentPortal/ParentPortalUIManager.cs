using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class ParentPortalUIManager : SingletonUI<ParentPortalUIManager> {

	public UIGrid grid;
	public GameObject parentPortalEntryPetPrefab;
	public GameObject parentPortalEntryCreatePrefab;

	void Awake(){
		eModeType = UIModeTypes.ParentPortal;
	}

	void UpdateGrid(){
		// TODO Jason populate grid here based on data

		grid.Reposition();
	}

	protected override void _OpenUI(){
		GetComponent<TweenToggleDemux>().Show();
	}

	protected override void _CloseUI(){
		GetComponent<TweenToggleDemux>().Hide();
	}

		void OnGUI(){
			if(GUI.Button(new Rect(100, 100, 100, 100), "Open")){
				OpenUI();
			}
			if(GUI.Button(new Rect(200, 100, 100, 100), "Close")){
				CloseUI();
			}
		}
}
