using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HighScoreUIManager : SingletonUI<HighScoreUIManager> {
	public GameObject backButton;
	public GameObject highscoreBoard;
	public UIGrid scoreBoardGrid;
	
	// related to zooming into the badge board
	public float fZoomTime;
	public Vector3 vOffset;
	public Vector3 vRotation;

	private bool isActive = false;

	void Start(){
		RefreshScoreBoard();
	}

	//Refresh high score
	private void RefreshScoreBoard(){
		GameObject highScoreEntryPrefab = (GameObject) Resources.Load("HighScoreEntry");
		Dictionary<string, int> highScoreDict = HighScoreManager.Instance.MinigameHighScore;

		foreach(KeyValuePair<string, int> score in highScoreDict){
			GameObject highScoreEntryGO = NGUITools.AddChild(scoreBoardGrid.gameObject, highScoreEntryPrefab);
			highScoreEntryGO.GetComponent<HighScoreEntryUIController>().Init(score.Key, score.Value);
		}
	}

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
			RoomArrowsUIManager.Instance.HidePanel();
			
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
			RoomArrowsUIManager.Instance.ShowPanel();
			
			if(D.Assert(backButton != null, "No back button to delete"))
				backButton.SetActive(false);
		}
	}
}
