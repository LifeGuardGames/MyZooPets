using UnityEngine;
using System.Collections;

//---------------------------------------------------
// LoadingScreen
// Script on prefabs that act as loading screens.
//---------------------------------------------------

public class LoadingScreen : MonoBehaviour {
	// artificial time we wait, if the loading screen is showing something important
	public float fWait = 0;
	
	// scene this screen should load
	private string strScene;
	
	//---------------------------------------------------
	// Init()
	//---------------------------------------------------	
	public void Init( string strScene ) {
		this.strScene = strScene;
		
		StartCoroutine( WaitAndLoad() );
	}

	//---------------------------------------------------
	// WaitAndLoad()
	// Waits the alotted amount of time, and then loads
	// the next scene.
	//---------------------------------------------------	
	private IEnumerator WaitAndLoad() {
		yield return new WaitForSeconds( fWait );
		
		// then load the scene
		Application.LoadLevel( strScene );	
	}
}
