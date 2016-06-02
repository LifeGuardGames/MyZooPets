using UnityEngine;

public class NavigationUIManager : Singleton<NavigationUIManager> {
	public TweenToggleDemux demux;
	
	public void ShowPanel(){
		demux.Show();
	}

	public void HidePanel(){
		demux.Hide();
	}

	// Modetype notInited, check CM
	public void OnEditRoomButton() {
		// if we are currently in edit deco mode, close the UI, otherwise, open it
		DecoInventoryUIManager.Instance.OpenUI();
	}

	// Modetype notInited, check CM
	public void OnStoreButton() {
		StoreUIManager.Instance.OpenUI();
	}

	// Modetype generic, check CM
	public void OnMissionsButton() {
		// Call from fire crystal ui manager > opens wellapad uimanager > opens fire crystal ui
		if(FireCrystalUIManager.Instance.IsOpen())
			FireCrystalUIManager.Instance.CloseUIBasedOnScene();
		else {
			FireCrystalUIManager.Instance.OpenUIBasedOnScene();
		}
	}
}
