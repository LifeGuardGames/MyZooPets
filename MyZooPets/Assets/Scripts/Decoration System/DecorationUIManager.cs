using UnityEngine;
using System;
using System.Collections;

public class DecorationUIManager : SingletonUI<DecorationUIManager> {

	public EventHandler<EventArgs> OnDecoPickedUp;   // when a decoration is picked up
	public EventHandler<EventArgs> OnDecoDropped;   // when a decoration is picked up

	private bool isActive = false;

	public GameObject backButton;
	public DecorationItem currentDeco;

	//When the highscore board is clicked and zoomed into
	protected override void _OpenUI(){
		if(!isActive){
			isActive = true;

			this.GetComponent<TweenToggleDemux>().Show();

			//Hide other UI objects
			NavigationUIManager.Instance.HidePanel();
			HUDUIManager.Instance.HidePanel();
			InventoryUIManager.Instance.HidePanel();
			EditDecosUIManager.Instance.HideNavButton();
			RoomArrowsUIManager.Instance.HidePanel();

			backButton.SetActive(true);
		}
	}
	
	//The back button on the left top corner is clicked to zoom out of the highscore board
	protected override void _CloseUI(){
		if(isActive){
			isActive = false;

			this.GetComponent<TweenToggleDemux>().Hide();
			
			//Show other UI Objects
			NavigationUIManager.Instance.ShowPanel();
			HUDUIManager.Instance.ShowPanel();
			InventoryUIManager.Instance.ShowPanel();
			EditDecosUIManager.Instance.ShowNavButton();
			RoomArrowsUIManager.Instance.ShowPanel();
			
			backButton.SetActive(false);
		}
	}
}
