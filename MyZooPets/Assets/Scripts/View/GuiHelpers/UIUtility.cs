using UnityEngine;
using System.Collections;

public class UIUtility : Singleton<UIUtility> {

	public GameObject mainCameraObject;
	private Camera mainCamera;
	public GameObject nguiCameraObject;
	private Camera nguiCamera;

	void Start(){
		mainCamera = mainCameraObject.GetComponent<Camera>();
		nguiCamera = nguiCameraObject.GetComponent<Camera>();
	}

	public Vector3 mainCameraWorld2Screen(Vector3 worldPos){
		return mainCamera.WorldToScreenPoint(worldPos);
	}

	public Vector3 nguiCameraWorld2Screen(Vector3 guiPos){
		return nguiCamera.WorldToScreenPoint(guiPos);
	}
}
