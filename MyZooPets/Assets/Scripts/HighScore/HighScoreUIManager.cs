using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HighScoreUIManager : SingletonUI<HighScoreUIManager>{
	public GameObject backButton;
	public GameObject highscoreBoard;
	public UIGrid scoreBoardGrid;

	// related to the camera move
	public Vector3 finalPosition;		// offset of camera on the target
	public Vector3 finalRotation;		// how the camera should rotate
	private float zoomTime = 0.5f;			// how long the tween should last

	private bool isActive = false;

	protected override void Start(){
		base.Start();
		RefreshScoreBoard();
	}

	//Refresh high score
	private void RefreshScoreBoard(){
		GameObject highScoreEntryPrefab = (GameObject)Resources.Load("HighScoreEntry");
		Dictionary<string, int> highScoreDict = HighScoreManager.Instance.MinigameHighScore;

		foreach(KeyValuePair<string, int> score in highScoreDict){
			GameObject highScoreEntryGO = NGUITools.AddChild(scoreBoardGrid.gameObject, highScoreEntryPrefab);
			highScoreEntryGO.GetComponent<HighScoreEntryUIController>().Init(score.Key, score.Value);
		}
	}

	//When the highscore board is clicked and zoomed into
	protected override void _OpenUI(){
		if(!isActive){
			AudioManager.Instance.PlayClip("subMenu");

			// if there is a camera move, do it -- otherwise, just skip to the move being complete
			if(zoomTime > 0){
				CameraManager.Callback cameraDoneFunction = delegate(){
					CameraMoveDone();
				};
				CameraManager.Instance.ZoomToTarget(finalPosition, finalRotation, zoomTime, cameraDoneFunction);
			}
			else{
				CameraMoveDone();
			}
			
			//Hide other UI objects
			NavigationUIManager.Instance.HidePanel();
			HUDUIManager.Instance.HidePanel();
			InventoryUIManager.Instance.HidePanel();
			RoomArrowsUIManager.Instance.HidePanel();
			
			isActive = true;
			highscoreBoard.collider.enabled = false;
			
			backButton.SetActive(true);
		}
	}

	/// <summary>
	/// Callback for when the camera is done tweening to its target
	/// </summary>
	private void CameraMoveDone(){

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
			RoomArrowsUIManager.Instance.ShowPanel();

			backButton.SetActive(false);
		}
	}
}
