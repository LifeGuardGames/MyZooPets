using UnityEngine;
using System.Collections;
using System;

public class NoteUIManager : SingletonUI<NoteUIManager> {

	public GameObject notePanel;
	
	// related to zooming in on the pet
	public GameObject pet;
	public Vector3 vRotation;
	public Vector3 vOffset;
	public float fZoomTime;

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
		EditDecosUIManager.Instance.HideNavButton();
		
		// zoom into pet
		Vector3 vPos =  pet.transform.position + vOffset;
		CameraManager.Instance.ZoomToTarget( vPos, vRotation, fZoomTime, null ); 
		
		Debug.Log("Note Clicked");
        notePanel.GetComponent<TweenToggle>().Show();
    }

    protected override void _CloseUI(){
		//Show other UI object
		NavigationUIManager.Instance.ShowPanel();
		InventoryUIManager.Instance.ShowPanel();
		EditDecosUIManager.Instance.ShowNavButton();
		
		// zoom away from pet
		CameraManager.Instance.ZoomOutMove();
		
		// Make sure callback NoteFinishedClosing is assigned in tween
        notePanel.GetComponent<TweenToggle>().Hide();
    }
}
