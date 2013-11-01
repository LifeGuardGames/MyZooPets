using UnityEngine;
using System.Collections;

public class UIUtility : Singleton<UIUtility> {

	public GameObject mainCameraObject;
	private Camera mainCamera;
	public GameObject nguiCameraObject;
	private Camera nguiCamera;
	public float ratioX;
	public float ratioY;

	void Start(){
		mainCamera = mainCameraObject.GetComponent<Camera>();
		nguiCamera = nguiCameraObject.GetComponent<Camera>();

		ratioX = 1280f/(Screen.width * 1.0f);
		ratioY = 800f/(Screen.height * 1.0f);
	}

	public Vector3 mainCameraWorld2Screen(Vector3 worldPos){
		// Accomodate for screen ratio scale
		Vector3 trueRatioScreenPos = mainCamera.WorldToScreenPoint(worldPos);
		Vector3 scaledRatioScreenPos = new Vector3(trueRatioScreenPos.x * ratioX, trueRatioScreenPos.y * ratioY, 0);
		return scaledRatioScreenPos;
	}

	public Vector3 nguiCameraWorld2Screen(Vector3 guiPos){
		// Accomodate for screen ratio scale
		Vector3 trueRatioScreenPos = nguiCamera.WorldToScreenPoint(guiPos);
		Vector3 scaledRatioScreenPos = new Vector3(trueRatioScreenPos.x * ratioX, trueRatioScreenPos.y * ratioY, 0);
		return scaledRatioScreenPos;
	}
}
