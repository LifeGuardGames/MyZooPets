using UnityEngine;
using System.Collections;
using System;

public class NoteUIManager : MonoBehaviour {

    //======================Event=============================
    public static event EventHandler<EventArgs> OnNoteClosed;
    //=======================================================

	public GameObject notePanel;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void NoteClicked(){
		Debug.Log("Note CLicked");
        notePanel.GetComponent<MoveTweenToggle>().Show();
    }

    public void NoteClosed(){
		// Make sure callback NoteFinishedClosing is assigned in tween
        notePanel.GetComponent<MoveTweenToggle>().Hide();

		if(D.Assert(OnNoteClosed != null, "OnNoteClosed has no listeners"))
            OnNoteClosed(this, EventArgs.Empty);
    }
}
