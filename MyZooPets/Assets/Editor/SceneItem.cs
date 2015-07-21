//// Copyright (c) 2015 LifeGuard Games Inc.

using UnityEngine;
using UnityEditor;
using System.Collections;

public class SceneItem : Editor {

	[MenuItem("Open Scene/LoadingScene")]
	public static void OpenLoadingScene(){
		OpenScene(SceneUtils.LOADING);
	}

	[MenuItem("Open Scene/MenuScene")]
	public static void OpenMenuScene(){
		OpenScene(SceneUtils.MENU);
	}

	[MenuItem("Open Scene/ZoneBedroom")]
	public static void OpenZoneBedroom(){
		OpenScene(SceneUtils.BEDROOM);
	}

	[MenuItem("Open Scene/ZoneYard")]
	public static void OpenZoneYard(){
		OpenScene(SceneUtils.YARD);
	}

	[MenuItem("Open Scene/InhalerGame")]
	public static void OpenInhalerGame(){
		OpenScene(SceneUtils.INHALERGAME);
	}

	[MenuItem("Open Scene/TriggerNinja")]
	public static void OpenTriggerNinja(){
		OpenScene("TriggerNinja");
	}

	[MenuItem("Open Scene/MemoryGame")]
	public static void OpenMemoryGame(){
		OpenScene("MemoryGame");
	}

	[MenuItem("Open Scene/ShooterGame")]
	public static void OpenShooterGame(){
		OpenScene("ShooterGame");
	}

	[MenuItem("Open Scene/DoctorMatch")]
	public static void OpenDoctorMatch(){
		OpenScene("DoctorMatch");
	}

	[MenuItem("Open Scene/Runner")]
	public static void OpenRunner(){
		OpenScene("Runner");
	}

	static void OpenScene(string name){
		if(EditorApplication.SaveCurrentSceneIfUserWantsTo()){
			EditorApplication.OpenScene("Assets/Scenes/" + name + ".unity");
		}
	}
}
