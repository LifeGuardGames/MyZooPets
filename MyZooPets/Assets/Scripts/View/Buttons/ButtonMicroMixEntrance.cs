using System;

public class ButtonMicroMixEntrance : ButtonChangeScene {
	public TweenToggle canvasToggle;
	public ButtonMicroMixEntranceHelper microMixEntranceHelper;

	private bool isCanvasShowing = false;

	void Start () {
		// Hide the object if you have not beat the game yet
		if(!DataManager.Instance.GameData.MicroMix.HasWon) {
			gameObject.SetActive(false);
		}
		else {
			if(DataManager.Instance.GameData.MicroMix.EntranceHasCrystal) {
				microMixEntranceHelper.FireEffectOn();
            }
			else {
				microMixEntranceHelper.FireEffectOff();
			}
		}
	}

	protected override void ProcessClick() {
		if(!isCanvasShowing) {
			canvasToggle.Show();
			isCanvasShowing = true;
            if(DataManager.Instance.GameData.MicroMix.EntranceHasCrystal) {
				Pass();
			}
		}
		else {
			canvasToggle.Hide();
			isCanvasShowing = false;
		}
	}
	 
	// Copy of the base processClick method bad design here but not worth it to patch
	public void Pass() {
		if(!isCheckMood || DataManager.Instance.GameData.Stats.Mood > moodThreshold) {
			// Mark the crystal entrance used
			DataManager.Instance.GameData.MicroMix.EntranceHasCrystal = false;

			// lock the click manager
			ClickManager.Instance.Lock();

			//Hide other UI Objects
			//Assuming that HUD is present at all scenes, so need to be hidden before scene change
			if(HUDUIManager.Instance != null) {
				HUDUIManager.Instance.HidePanel();
			}
			if(NavigationUIManager.Instance != null) {
				NavigationUIManager.Instance.HidePanel();
			}
			if(InventoryUIManager.Instance != null) {
				InventoryUIManager.Instance.HidePanel();
			}
			RoomArrowsUIManager.Instance.HidePanel();

			//Sent an change scene event out, so other objects can run appropriate logic before scene change
			if(OnChangeScene != null) {
				OnChangeScene(this, EventArgs.Empty);
			}

			//Save some basic data for current scene
			RememberCurrentScene();

			//record that this entrance has been used
			if(entranceHelper != null) {
				entranceHelper.EntranceUsed();
			}

			// if there is a camera move, do it -- otherwise, just skip to the move being complete
			if(zoomTime > 0) {
				CameraManager.Callback cameraDoneFunction = delegate () {
					CameraMoveDone();
				};
				CameraManager.Instance.ZoomToTarget(finalPosition, finalRotation, zoomTime, cameraDoneFunction);
			}
			else {
				CameraMoveDone();
			}
		}
		else {
			PetSpeechAI.Instance.ShowSadMessage();
		}
	}
}
