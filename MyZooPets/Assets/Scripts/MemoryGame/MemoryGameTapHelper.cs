using UnityEngine;
using System.Collections;

public class MemoryGameTapHelper : MonoBehaviour{

	void OnFingerDown(FingerDownEvent e){
		if(e.Selection != null){
			Debug.Log(e.Selection.name);
		}
	}
}
