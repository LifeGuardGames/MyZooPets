using UnityEngine;
using System.Collections;

public class RoomArrowsUIManager : Singleton<RoomArrowsUIManager> {

	public TweenToggleDemux roomArrowsDemux;
	public TweenToggle leftArrowTween;
	public TweenToggle rightArrowTween;

	
	// Shows both arrows
	public void ShowPanel(){
		leftArrowTween.Show();
		rightArrowTween.Show();
	}

	// Hides both arrows
	public void HidePanel(){
		leftArrowTween.Hide();
		rightArrowTween.Hide();
	}

	// Shows left arrow
	public void ShowLeftArrow(){
		leftArrowTween.Show();
		rightArrowTween.Hide();
	}

	// Shows right arrow
	public void ShowRightArrow(){
		rightArrowTween.Show();
		leftArrowTween.Hide();
	}

	void OnGUI(){
		if(GUI.Button(new Rect(100, 100, 100, 100), "Show all")){
			ShowPanel();
		}
		else if(GUI.Button(new Rect(200, 100, 100, 100), "Hide all")){
			HidePanel();
		}
		else if(GUI.Button(new Rect(300, 100, 100, 100), "Show Left")){
			ShowLeftArrow();
		}
		else if(GUI.Button(new Rect(400, 100, 100, 100), "Show Right")){
			ShowRightArrow();
		}
	}
}
