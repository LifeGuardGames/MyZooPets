using UnityEngine;
using System.Collections;

public class TestMissionFlow : MonoBehaviour {

	public void getMission(){
		WellapadMissionController.Instance.needMission = true;
		WellapadMissionController.Instance.AddTask("Ninja");

	}
}
