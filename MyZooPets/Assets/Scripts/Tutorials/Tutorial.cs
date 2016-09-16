using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

/// <summary>
/// Parent class for all tutorials
/// </summary>
public abstract class Tutorial {
	//---------------------Events--------------------------
	public EventHandler<TutorialEndEventArgs> OnTutorialEnd;// when the tutorial ends

	// ----------- Abstract functions -------------------
	protected abstract void SetKey();                       // the tutorial key is used to mark a lot of lists
	protected abstract void SetMaxSteps();                  // set the max steps of the tutorial
	protected abstract void ProcessStep(int nStep);			// the meat of a tutorial is processing its steps and doing things
	protected abstract void _End(bool isFinished);          // when the tutorial is finishd

	// ----------- Tutorial Popup types -------------------
	protected const string TUTPOPUPTEXT = "TutorialPopupText";

	protected int maxSteps;									// max steps in the tutorial
	protected string tutorialKey;							// key for this tutorial
	protected Vector3 POS_TOP = new Vector3(0, 201, -10);	// top position to spawn the popup (NGUI)
	protected Vector3 POS_BOT = new Vector3(0, -275, -10);	// bottom position to spawn the popup (NGUI)

	private List<GameObject> listCanProcess = new List<GameObject>(); // list of objects that can be processed as input
	private GameObject goSpotlight;							// current (and only) spotlight object this tutorial is highlighting
	protected GameObject goPopup;							// current (and only) tutorial popup
	private GameObject goFingerHint;						// current finger hint
	private GameObject goRetentionPet;						// Current retention pet sprite
	private int currentStep;								// step the tutorial is currently on

	public bool CanProcess(GameObject go) {
		bool canProcess = listCanProcess.Contains(go);
		return canProcess;
	}

	//Return the current step that the tutorial is on
	public int GetStep() {
		return currentStep;
	}

	protected void AddToProcessList(GameObject go, bool isDebugLog = false) {
		listCanProcess.Add(go);
		if(isDebugLog) {
			// Debug tag here
		}
	}

	protected void RemoveFromProcessList(GameObject go) {
		listCanProcess.Remove(go);
	}

	//Set the tutorial to a specific step
	protected void SetStep(int num) {
		currentStep = num;
		// if we have exceeded max steps in this tutorial, end it
		if(currentStep >= maxSteps) {
			End(true);
		}
		else {
			ProcessStep(currentStep);
		}
	}

	//Return the key of this tutorial
	protected string GetKey() {
		if(string.IsNullOrEmpty(tutorialKey))
			SetKey();

		return tutorialKey;
	}

	public Tutorial() {
		// Debug.Log("Starting tutorial " + GetKey());
		SetMaxSteps();
		SetStep(0);
	}

	/// <summary>
	/// Go to the next part of this tutorial
	/// </summary>
	public void Advance() {
		// increment the current step of the tutorial
		int step = GetStep();
		step++;
		SetStep(step);
	}

	/// <summary>
	/// Ends the tutorial early
	/// </summary>
	public void Abort() {
		End(false);
	}

	/// <summary>
	/// When this tutorial is finished.
	/// </summary>
	/// <param name="isFinished">If set to <c>true</c> is finished.</param>
	protected virtual void End(bool isFinished) {
		// Debug.Log("Tutorial Ending: " + GetKey());

		// let children know the tutorial is over
		_End(isFinished);

		// save the fact that the user completed this tutorial
		if(isFinished) {
			if(!DataManager.Instance.GameData.Tutorial.IsTutorialFinished(GetKey())) {
				DataManager.Instance.GameData.Tutorial.ListPlayed.Add(GetKey());
			}
			Analytics.Instance.TutorialCompleted(GetKey());
			//DataManager.Instance.SaveGameData();
		}

		if(goPopup != null) {
			GameObject.Destroy(goPopup);
		}

		// activate tutorial end callback
		if(OnTutorialEnd != null) {
			OnTutorialEnd(this, new TutorialEndEventArgs(isFinished));
		}
	}

	/// <summary>
	/// <para>Puts a spotlight around the incoming object to
	/// draw attention to it.
	/// eAnchor is the incoming anchor of the object/where
	/// the spotlight should be created.  For 3D objects
	/// the anchor should be center, and for GUI elements
	/// the anchor should be whatever anchor the element
	/// is in.</para>
	/// 
	/// Params:
	///		goTarget (GameObject): the target that you want the spotlight to spawn on
	/// 
	/// Optional Params:
	/// 	isGUI (bool): is it a UI element.
	///  	eAnchor (InteraceAnchors): the anchor to spawn the spot light under
	///  	strSpotlightPrefab (string): the string name of the spotlight prefab
	///  	fingerHint (bool): show finger hint or not
	///  	fingerHintOffsetFromSpotlightCenter (Vector2): offset of the finger hint
	///  	delay (float): how long does it take the spot light to fade in
	/// </summary>
	protected void SpotlightObject(GameObject goTarget, bool isGUI = false,
		string spotlightPrefab = "TutorialSpotlight",
		bool fingerHint = false, string fingerHintPrefab = "BedroomTutFingerPress",
		float focusOffsetX = 0f, float focusOffsetY = 0f,
		float fingerHintOffsetX = 0f, float fingerHintOffsetY = 60f,
		bool fingerHintFlip = false, float delay = -1f) {

		// get the proper location of the object we are going to focus on
		Vector3 focusPos;
		if(isGUI) {
			Debug.LogWarning("NGUI REMOVE CHANGED - CORRECT CODE HERE");
			focusPos = Vector3.zero; // TEMP CODE - remove me once fixed
									 //focusPos = LgNGUITools.GetScreenPosition(goTarget);
		}
		else {
			// WorldToScreen returns screen coordinates based on 0,0 being bottom left, so we need to transform those into NGUI center
			focusPos = CameraManager.Instance.WorldToScreen(CameraManager.Instance.CameraMain, goTarget.transform.position);
			focusPos = CameraManager.Instance.TransformAnchorPosition(focusPos, InterfaceAnchors.BottomLeft, InterfaceAnchors.Center);
		}

		// Adjust for custom offset
		focusPos = new Vector3(focusPos.x + focusOffsetX, focusPos.y + focusOffsetY, focusPos.z);

		// destroy the old object if it existed
		if(goSpotlight != null) {
			GameObject.Destroy(goSpotlight);
		}

		if(goFingerHint != null) {
			GameObject.Destroy(goFingerHint);
		}

		// create the spotlight
		GameObject goResource = Resources.Load(spotlightPrefab) as GameObject;
		goSpotlight = GameObjectUtils.AddChildGUI(TutorialManager.Instance.UICanvasParent, goResource);

		// Set the delay if defined
		if(delay > 0) {
			goSpotlight.GetComponent<AlphaTweenToggle>().showDelay = delay;
		}

		// move the spotlight into position
		goSpotlight.transform.localPosition = focusPos;

		// spawn finger hint
		if(fingerHint) {
			GameObject fingerHintResource = (GameObject)Resources.Load(fingerHintPrefab);
			goFingerHint = GameObjectUtils.AddChildGUI(TutorialManager.Instance.UICanvasParent, fingerHintResource);
			focusPos.z = goFingerHint.transform.localPosition.z;
			focusPos.y = focusPos.y + fingerHintOffsetY;        //offset in Y so the finger hint doesn't overlap the image
			focusPos.x = focusPos.x + fingerHintOffsetX;
			goFingerHint.transform.localPosition = focusPos;

			if(fingerHintFlip) {
				goFingerHint.transform.localScale = new Vector3(-1, 1, 1);
			}
		}

		// Show the backdrop
		goSpotlight.GetComponent<AlphaTweenToggle>().ShowAfterInit();
	}

	//--------------------------------------------------------------
	// ShowFingerHint()
	// Use this function if you only want to spawn finger hint
	//--------------------------------------------------------------
	protected void ShowFingerHint(GameObject goTarget, bool isGUI = false, string fingerHintPrefab = "BedroomTutFingerPress",
		float offsetFromCenter = 60.0f, float offsetFromCenterX = 0.0f, bool flipX = false) {

		// get the proper location of the object we are going to focus on
		Vector3 focusPos;
		if(isGUI) {
			Debug.LogWarning("NGUI REMOVE CHANGED - CORRECT CODE HERE");
			focusPos = Vector3.zero; // TEMP CODE - remove me once fixed
									 //focusPos = LgNGUITools.GetScreenPosition(goTarget);
		}
		else {
			// WorldToScreen returns screen coordinates based on 0,0 being bottom left, so we need to transform those into NGUI center
			focusPos = CameraManager.Instance.WorldToScreen(CameraManager.Instance.CameraMain, goTarget.transform.position);
			// Camera.main.WorldToScreenPoint( goTarget.transform.position );

			focusPos = CameraManager.Instance.TransformAnchorPosition(focusPos,
				InterfaceAnchors.BottomLeft, InterfaceAnchors.Center);
		}

		if(goFingerHint != null) {
			GameObject.Destroy(goFingerHint);
		}

		GameObject fingerHintResource = (GameObject)Resources.Load(fingerHintPrefab);
		goFingerHint = GameObjectUtils.AddChildGUI(TutorialManager.Instance.UICanvasParent, fingerHintResource);
		focusPos.x = focusPos.x + offsetFromCenterX;
		focusPos.y = focusPos.y + offsetFromCenter; //offset in Y so the finger hint doesn't overlap the image
		focusPos.z = goFingerHint.transform.localPosition.z;
		goFingerHint.transform.localPosition = focusPos;

		if(flipX) {
			goFingerHint.transform.localScale = new Vector3(-1, 1, 1);
		}

	}

	protected void RemoveFingerHint() {
		if(goFingerHint != null) {
			GameObject.Destroy(goFingerHint);
		}
	}

	//---------------------------------------------------
	// RemoveSpotlight()
	// Removes the current spotlight object.
	//---------------------------------------------------		
	protected void RemoveSpotlight() {
		if(goSpotlight != null) {
			GameObject.Destroy(goSpotlight);
		}
	}

	//---------------------------------------------------
	// RemovePopup()
	// Removes the current popup object.
	//---------------------------------------------------		
	protected void RemovePopup() {
		if(goPopup != null) {
			GameObject.Destroy(goPopup);
		}
	}

	/// <summary>
	/// Display the tutorial popup, messages are loaded automatically unless overridden
	/// </summary>
	protected void ShowPopup(string popupPrefabKey, Vector3 localPosition, string customMessage = null) {
		if(goPopup) {
			GameObject.Destroy(goPopup);
		}
		
		// Create the popup
		GameObject goResource = Resources.Load(popupPrefabKey) as GameObject;
		goPopup = GameObjectUtils.AddChildGUI(TutorialManager.Instance.UICanvasParent, goResource);
		goPopup.transform.localPosition = localPosition;

		// Populate popup text
		Text popupText = goPopup.GetComponentInChildren<Text>();
		if(!string.IsNullOrEmpty(customMessage)) {  // Check if message needs to be overridden
			popupText.text = customMessage;
		}
		else {										// Get text to display from tutorial key + step
			popupText.text = Localization.Localize(GetKey() + "_" + GetStep());
		}
    }

	public void ShowRetentionPet(bool isFlipped, Vector3 position) {
		GameObject goResource = Resources.Load("TutorialRetentionPet") as GameObject;
		goRetentionPet = GameObjectUtils.AddChildGUI(TutorialManager.Instance.UICanvasParent, goResource);
		goRetentionPet.transform.localPosition = position;
		if(isFlipped) {
			goRetentionPet.transform.localScale = new Vector3(-1f, 1f, 1f);
		}
	}

	public void RemoveRetentionPet() {
		if(goRetentionPet != null) {
			GameObject.Destroy(goRetentionPet);
		}
	}
}
