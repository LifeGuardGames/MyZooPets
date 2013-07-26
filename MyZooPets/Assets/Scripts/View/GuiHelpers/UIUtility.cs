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

	public Vector3 mCameraWorld2Screen(Vector3 worldPos){
		Debug.Log("Logging! ---- " + worldPos + " ---> " + mainCamera.WorldToScreenPoint(worldPos));
		return mainCamera.WorldToScreenPoint(worldPos);
	}
}
