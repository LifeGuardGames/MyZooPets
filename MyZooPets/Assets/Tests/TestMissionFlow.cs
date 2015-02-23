using UnityEngine;
using System.Collections;

public class TestMissionFlow : MonoBehaviour {

	public void getMission(){
		DataManager.Instance.GameData.Wellapad.ResetMissions();
		MiniPetManager.Instance.needMission = true;
		WellapadMissionController.Instance.AddMission("Critical");

	}
}
