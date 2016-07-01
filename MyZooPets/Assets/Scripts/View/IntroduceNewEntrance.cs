using UnityEngine;
using System.Collections;

/// <summary>
/// Introduce new entrance. Attach this script to any entrance helper gameobject.
/// Use this to show bouncing arrow on any new zoomable entrance or object in the
/// game that we want the user to discover.
/// </summary>
public class IntroduceNewEntrance : MonoBehaviour{

	// Use this for initialization
	void Start(){
		CheckToHighlightEntrance();
	}

	private void CheckToHighlightEntrance(){
		bool areTutorialsFinished = DataManager.Instance.GameData.Tutorial.AreBedroomTutorialsFinished();

		if(areTutorialsFinished){
			this.gameObject.SetActive(true);
		}
		else{
			this.gameObject.SetActive(false);
		}
	}
}

