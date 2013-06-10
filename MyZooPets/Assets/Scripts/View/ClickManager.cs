using UnityEngine;
using System.Collections;

public class ClickManager : MonoBehaviour {
	
	private bool isMobilePlatform;
	
	// All the classes that need a click input go here
	public GameObject cameraMoveObject;
	private CameraMove cameraMove;
	
	public GameObject diaryUIManagerObject;
	private DiaryUIManager diaryUIManager;
	
	
	void Start(){
		if(Application.platform == RuntimePlatform.Android ||
			Application.platform == RuntimePlatform.IPhonePlayer){
			isMobilePlatform = true;
		}
		else{
			isMobilePlatform = false;
		}
		
		// Linking script references
		cameraMove = cameraMoveObject.GetComponent<CameraMove>();
		diaryUIManager = diaryUIManagerObject.GetComponent<DiaryUIManager>();
	}

	void Update(){
		if((isMobilePlatform && Input.touchCount > 0) || (!isMobilePlatform && Input.GetMouseButtonUp(0)))
		{	
			if(isMobilePlatform && (Input.GetTouch(0).phase == TouchPhase.Ended) || !isMobilePlatform)
			{	
				Ray myRay = Camera.main.ScreenPointToRay(Input.mousePosition);
				RaycastHit hit;
				if(Physics.Raycast(myRay,out hit))
				{
					if(hit.collider.name == "room_shelf")
					{
						cameraMove.ShelfZoomToggle();
					}
					
					if(hit.collider.name == "room_table")
					{
						diaryUIManager.DiaryClicked();
					}
				}
			}
		}
	}
}
