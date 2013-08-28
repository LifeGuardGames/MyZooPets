using UnityEngine;
using System.Collections;
using System;

public class NoteUIManager : SingletonUI<NoteUIManager> {

	public GameObject notePanel;
	
	public CameraMove cameraMove;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    protected override void _OpenUI(){
		//Hide other UI objects
		NavigationUIManager.Instance.HidePanel();
		InventoryUIManager.Instance.HidePanel();
		
		// zoom into pet
		cameraMove.ZoomToggle(ZoomItem.Pet); 
		
		Debug.Log("Note CLicked");
        notePanel.GetComponent<MoveTweenToggle>().Show();
    }

    protected override void _CloseUI(){
		//Show other UI object
		NavigationUIManager.Instance.ShowPanel();
		InventoryUIManager.Instance.ShowPanel();
		
		// zoom away from pet
		cameraMove.ZoomOutMove();
		
		// Make sure callback NoteFinishedClosing is assigned in tween
        notePanel.GetComponent<MoveTweenToggle>().Hide();
    }
}
