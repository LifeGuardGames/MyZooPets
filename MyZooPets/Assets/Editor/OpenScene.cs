//// Copyright (c) 2016 LifeGuard Games Inc.
using UnityEditor;
using UnityEditor.SceneManagement;

public class OpenScene : Editor {
	[MenuItem("Open Scene/LoadingScene")]
	public static void OpenLoadingScene(){
		LoadScene(SceneUtils.LOADING);
	}

	[MenuItem("Open Scene/MenuScene")]
	public static void OpenMenuScene(){
		LoadScene(SceneUtils.MENU);
	}

	[MenuItem("Open Scene/ZoneBedroom")]
	public static void OpenZoneBedroom(){
		LoadScene(SceneUtils.BEDROOM);
	}

	[MenuItem("Open Scene/ZoneYard")]
	public static void OpenZoneYard(){
		LoadScene(SceneUtils.YARD);
	}

	[MenuItem("Open Scene/InhalerGame")]
	public static void OpenInhalerGame(){
		LoadScene(SceneUtils.INHALERGAME);
	}

	[MenuItem("Open Scene/TriggerNinja")]
	public static void OpenTriggerNinja(){
		LoadScene("TriggerNinja");
	}

	[MenuItem("Open Scene/MemoryGame")]
	public static void OpenMemoryGame(){
		LoadScene("MemoryGame");
	}

	[MenuItem("Open Scene/ShooterGame")]
	public static void OpenShooterGame(){
		LoadScene("ShooterGame");
	}

	[MenuItem("Open Scene/DoctorMatch")]
	public static void OpenDoctorMatch(){
		LoadScene("DoctorMatch");
	}

	[MenuItem("Open Scene/Runner")]
	public static void OpenRunner(){
		LoadScene("Runner");
	}

	[MenuItem("Open Scene/MicroMix")]
	public static void OpenMicroMix() {
		LoadScene("MicroMix");
	}

	[MenuItem("Open Scene/Salon")]
	public static void OpenSalon() {
		LoadScene("WizdyPetSalon");
	}

	static void LoadScene(string name){
		if(EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) {
			EditorSceneManager.OpenScene("Assets/Scenes/" + name + ".unity");
		}
	}
}
