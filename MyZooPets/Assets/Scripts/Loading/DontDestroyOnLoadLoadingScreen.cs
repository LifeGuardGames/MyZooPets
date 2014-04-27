using UnityEngine;
using System.Collections;

/// <summary>
/// Only to be used once! Keeps one instance of loading screen UI manager in each scene
/// </summary>
public class DontDestroyOnLoadLoadingScreen : MonoBehaviour {
	private static bool isCreated;
	void Awake(){
		//Make Object persistent
		if(isCreated){
			//If There is a duplicate in the scene. delete the object and jump Awake
			Destroy(gameObject);
			return;
		}
		DontDestroyOnLoad(gameObject);
		isCreated = true;
	}
}
