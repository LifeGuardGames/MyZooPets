using UnityEngine;
using System.Collections;

public class TestMissionFlow : MonoBehaviour {

	public void getMission(){
		WellapadMissionController.Instance.AddTask("Ninja");

	}
}
