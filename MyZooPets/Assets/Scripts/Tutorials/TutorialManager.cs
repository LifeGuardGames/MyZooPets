using UnityEngine;

/// <summary>
/// Tutorial manager.
/// Used in scenes like the yard and bedroom to keep
/// track of game tutorials.
/// </summary>
public class TutorialManager : Singleton<TutorialManager> {
	public GameObject UICanvasParent;           // Used for spawning things
	protected bool isTutorialEnabled;           // public on/off switch for testing while in development
	private GameTutorial tutorial;              // tutorial that is currently active

	/// <summary>
	/// Whether tutorial is active in the scene
	/// </summary>
	public bool IsTutorialActive() {
		bool isActive = tutorial != null;
		return isActive;
	}

	public void SetTutorial(GameTutorial tutorial) {
		// check to make sure there are not overlapping tutorials
		if(tutorial != null && this.tutorial != null) {
			return;
		}

		this.tutorial = tutorial;

		// if the incoming tutorial is null...
		if(tutorial == null) {
			DataManager.Instance.SaveGameData();        // now that the tutorial is over, force a save
			IsPlayTutorial();                           // then check for a new tutorial
		}
	}

	/// <summary>
	/// Used in scenes like the yard and bedroom to keep track of game tutorials
	/// </summary>
	public bool CanProcess(GameObject processGameObject) {
		// if the gameobject is null, then tutorial doesn't care (at the moment)
		if(processGameObject == null) {
			return true;
		}

		// if there is no tutorial currently going on right now, the tutorial doesn't care (obviously)
		bool isActive = IsTutorialActive();
		if(!isActive) {
			return true;
		}

		// otherwise we have a valid object and a valid tutorial, so let's get to checkin'
		bool canProcess = tutorial.CanProcess(processGameObject);
		return canProcess;
	}

	protected virtual void Awake() {
		if(DataManager.Instance.isDebug) {
			isTutorialEnabled = Constants.GetConstant<bool>("IntroTutorialsEnabled");
		}
		else {
			isTutorialEnabled = true;
		}
	}

	protected virtual void Start() {
		// listen for partition changing event
		CameraManager.Instance.PanScript.OnPartitionChanged += EnteredRoom;
	}

	/// <summary>
	/// Checks which tutorial should play based on certain game conditions
	/// </summary>
	protected virtual bool IsPlayTutorial() {
		bool playTutorial = true;

		if(!isTutorialEnabled || tutorial != null) {
			playTutorial = false;
		}

		return playTutorial;
	}

	/// <summary>
	/// Entered the room. When player switches rooms.
	/// </summary>
	private void EnteredRoom(object sender, PartitionChangedArgs args) {
		// do a check in case a tutorial was in a different room
		IsPlayTutorial();
	}
}
