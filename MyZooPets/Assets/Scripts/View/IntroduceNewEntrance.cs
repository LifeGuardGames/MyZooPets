using UnityEngine;

/// <summary>
/// Attach this script to any entrance helper gameobject.
/// Use this to show bouncing arrow on any new zoomable entrance or object in the
/// game that we want the user to discover.
/// </summary>
public class IntroduceNewEntrance : MonoBehaviour {
	void Start() {
		gameObject.SetActive(DataManager.Instance.GameData.Tutorial.AreBedroomTutorialsFinished());
	}
}

