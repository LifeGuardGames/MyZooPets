using UnityEngine;
using System.Collections;

/// <summary>
/// Preview scene.
/// Just a sample scene that accesses all the other scenes
/// </summary>
public class PreviewScene : MonoBehaviour {

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

	void OnGUI(){
		if(Application.loadedLevelName != "Preview"){
			if(GUI.Button(new Rect(100, 100, 100, 100), "Back")){
				Application.LoadLevel("Preview");
			}
		}
		else{
			if(GUI.Button(new Rect(100, 100, 100, 100), "Menu")){
				Application.LoadLevel("Menuscene");
			}

			if(GUI.Button(new Rect(100, 300, 100, 100), "Bedroom")){
				Application.LoadLevel("ZoneBedroom");
			}
			if(GUI.Button(new Rect(200, 300, 100, 100), "Yard")){
				Application.LoadLevel("ZoneYard");
			}

			if(GUI.Button(new Rect(100, 500, 100, 100), "Inhaler")){
				Application.LoadLevel("InhalerGame");
			}
			if(GUI.Button(new Rect(200, 500, 100, 100), "TriggerNinja")){
				Application.LoadLevel("TriggerNinja");
			}
			if(GUI.Button(new Rect(300, 500, 100, 100), "Memory")){
				Application.LoadLevel("MemoryGame");
			}
			if(GUI.Button(new Rect(400, 500, 100, 100), "Clinic")){
				Application.LoadLevel("DoctorMatch");
			}
			if(GUI.Button(new Rect(500, 500, 100, 100), "Runner")){
				Application.LoadLevel("Runner");
			}
			if(GUI.Button(new Rect(600, 500, 100, 100), "Shooter")){
				Application.LoadLevel("ShooterGame");
			}
		}
	}
}
