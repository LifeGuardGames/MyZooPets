using UnityEngine;
using System.Collections;

public class PlayPeriodDebugging : MonoBehaviour{

	public GUIStyle style;

	void OnGUI(){
		GUI.Label(new Rect(50, 50, 200, 50), "Current PP: " + PlayPeriodLogic.GetCurrentPlayPeriod().ToString(), style);
		GUI.Label(new Rect(80, 100, 200, 50), "Last PP: " + PlayPeriodLogic.Instance.GetLastPlayPeriod().ToString(), style);
	}
}
