using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

//---------------------------------------------------
// ItemBoxUI
// UI Manager for the item box UI.  This is a little
// different than other UI managers, because the
// object this manager is on is actually instantiated
// and destroyed with each item box.
//---------------------------------------------------

public class ItemBoxUI : SingletonUI<ItemBoxUI> {
	
	// the grid that items inside the box are placed in to
	public GameObject goGrid;
	
	// prefab to instantiate for an individual item
	public GameObject prefabEntry;
	
	// used to properly name items in the grid for proper sorting
	private int nCount = 0;

	// Use this for initialization
	void Start () {
	}
	
	//---------------------------------------------------
	// _OpenUI()
	//---------------------------------------------------	
	protected override void _OpenUI(){
		// now trigger the demux showing
		gameObject.GetComponent<TweenToggleDemux>().Show();		
	}
	
	//---------------------------------------------------
	// _CloseUI()
	//---------------------------------------------------	
	protected override void _CloseUI(){
		gameObject.GetComponent<TweenToggleDemux>().Hide();			
	}
	
	//---------------------------------------------------
	// InitBox()
	//---------------------------------------------------		
	public IEnumerator InitBox( List<KeyValuePair<Item, int>> items ) {
		// wait a frame because of NGUI grid stuff
		yield return 0;
		
		// loop through items in the list and add them to the grid
		foreach( KeyValuePair<Item,int> item in items ) {
			GameObject goEntry = NGUITools.AddChild(goGrid, prefabEntry);
			SetNameForGrid( goEntry );
			
			// init the individual items inside the grid
			goEntry.GetComponent<ItemBoxEntry>().Init( item.Key, item.Value );
		}
		
		goGrid.GetComponent<UIGrid>().Reposition();
		
		OpenUI();
	}
	
	//---------------------------------------------------
	// SetNameForGrid()
	// Because it was apparently too hard for the NGUI
	// guy to just let elements in a grid appear in the
	// order they are added, we must actually rename the
	// objects in the grid numerically as they are added,
	// so they are sorted properly.
	//---------------------------------------------------		
	private void SetNameForGrid( GameObject go ) {
		go.name = nCount + "_ItemBoxEntry";
		nCount += 1;
	}	
	
	//---------------------------------------------------
	// DoneHiding()
	// Callback from the demux when the UI has finished
	// tweening off screen.
	//---------------------------------------------------		
	private void DoneHiding() {
		Destroy( gameObject );
	}
}
