using UnityEngine;
using System.Collections;

public class HighscoreUIManager : SingletonUI<HighscoreUIManager> {
	public GameObject backButton;
	public GameObject highscoreBoard;
	
	// related to zooming into the badge board
	public float fZoomTime;
	public Vector3 vOffset;
	public Vector3 vRotation;

	private bool isActive = false;

	//When the highscore board is clicked and zoomed into
	protected override void _OpenUI(){
		if(!isActive){
			// zoom into the board
			Vector3 vPos = highscoreBoard.transform.position + vOffset;
			CameraManager.Instance.ZoomToTarget( vPos, vRotation, fZoomTime, null );
			
			//Hide other UI objects
			NavigationUIManager.Instance.HidePanel();
			HUDUIManager.Instance.HidePanel();
			InventoryUIManager.Instance.HidePanel();
			EditDecosUIManager.Instance.HideNavButton();
			
			isActive = true;
			highscoreBoard.collider.enabled = false;
			
			backButton.SetActive(true);
		}
	}

	//The back button on the left top corner is clicked to zoom out of the highscore board
	protected override void _CloseUI(){
		if(isActive){
			isActive = false;
			highscoreBoard.collider.enabled = true;
			
			CameraManager.Instance.ZoomOutMove();
			
			//Show other UI Objects
			NavigationUIManager.Instance.ShowPanel();
			HUDUIManager.Instance.ShowPanel();
			InventoryUIManager.Instance.ShowPanel();
			EditDecosUIManager.Instance.ShowNavButton();
			
			if(D.Assert(backButton != null, "No back button to delete"))
				backButton.SetActive(false);
		}
	}
}
