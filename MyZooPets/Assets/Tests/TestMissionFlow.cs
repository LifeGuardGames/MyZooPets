using UnityEngine;
using System.Collections;

public class TestMissionFlow : MonoBehaviour {

	public void getMission(){
		WellapadMissionController.Instance.UnlockTask("Ninja");
		MiniPetManager.Instance.needMission = true;
		WellapadMissionController.Instance.AddMission("Ninja");

	}
}
