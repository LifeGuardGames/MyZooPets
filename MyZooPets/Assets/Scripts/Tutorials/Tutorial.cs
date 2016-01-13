using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Parent class for all tutorials
/// </summary>
public abstract class Tutorial{
	//---------------------Events--------------------------
	public EventHandler<TutorialEndEventArgs> OnTutorialEnd; // when the tutorial ends

	// ----------- Abstract functions -------------------
	protected abstract void SetKey();						// the tutorial key is used to mark a lot of lists
	protected abstract void SetMaxSteps();					// set the max steps of the tutorial
	protected abstract void ProcessStep(int nStep);		// the meat of a tutorial is processing its steps and doing things
	protected abstract void _End(bool isFinished);			// when the tutorial is finishd
	
	// ----------- Tutorial Popup types -------------------
	protected const string POPUP_STD = "TutorialPopup_Standard";
	protected const string POPUP_STD_WITH_IMAGE = "TutorialPopup_StandardWithImage";
	protected const string POPUP_LONG = "TutorialPopup_Long";
	protected const string POPUP_LONG_WITH_BUTTON = "TutorialPopup_LongWithButton";
	protected const string POPUP_LONG_WITH_BUTTON_AND_IMAGE = "TutorialPopup_LongWithButtonAndImage";
	protected int maxSteps; // max steps in the tutorial
	protected string tutorialKey; // key for this tutorial
	protected Vector3 POS_TOP = new Vector3(0, 201, -10); //top position to spawn the popup (NGUI)
	protected Vector3 POS_BOT = new Vector3(0, -275, -10); //bottom position to spawn the popup (NGUI)

	private List<GameObject> listCanProcess = new List<GameObject>(); // list of objects that can be processed as input
	private GameObject goSpotlight;	// current (and only) spotlight object this tutorial is highlighting
	private GameObject goPopup; // current (and only) tutorial popup
	private GameObject goFingerHint; //current finger hint
	private GameObject goRetentionPet;	// Current retention pet sprite
	private int currentStep; // step the tutorial is currently on

	
	public bool CanProcess(GameObject go){
		bool canProcess = listCanProcess.Contains(go);
		return canProcess;
	}

	//Return the current step that the tutorial is on
	public int GetStep(){
		return currentStep;	
	}

	protected void AddToProcessList(GameObject go, bool isDebugLog = false){
		listCanProcess.Add(go);
		if(isDebugLog){
			// Debug tag here
		}
	}
	
	protected void RemoveFromProcessList(GameObject go){
		listCanProcess.Remove(go);	
	}

	//Set the tutorial to a specific step
	protected void SetStep(int num){
		currentStep = num;
		// if we have exceeded max steps in this tutorial, end it
		if(currentStep >= maxSteps){
			End(true);
		}
		else{
			ProcessStep(currentStep);
		}
	}

	//Return the key of this tutorial
	protected string GetKey(){
		if(string.IsNullOrEmpty(tutorialKey))
			SetKey();
		
		return tutorialKey;	
	}
	
	public Tutorial(){
		// Debug.Log("Starting tutorial " + GetKey());
		SetMaxSteps();
		SetStep(0);
	}

	/// <summary>
	/// Go to the next part of this tutorial
	/// </summary>
	public void Advance(){
		// increment the current step of the tutorial
		int step = GetStep();
		step++;
		SetStep(step);
	}	

	/// <summary>
	/// Ends the tutorial early
	/// </summary>
	public void Abort(){
		End(false);	
	}

	/// <summary>
	/// When this tutorial is finished.
	/// </summary>
	/// <param name="isFinished">If set to <c>true</c> is finished.</param>
	protected virtual void End(bool isFinished){
		// debug message
		// Debug.Log("Tutorial Ending: " + GetKey());
		
		// let children know the tutorial is over
		_End(isFinished);
		
		// save the fact that the user completed this tutorial
		if(isFinished){
			DataManager.Instance.GameData.Tutorial.ListPlayed.Add(GetKey());
			Analytics.Instance.TutorialCompleted(GetKey());
		}

		if(goPopup != null){
			GameObject.Destroy(goPopup);
		}
		
		// activate tutorial end callback
		if(OnTutorialEnd != null){
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
		InterfaceAnchors anchor = InterfaceAnchors.Center, string spotlightPrefab = "TutorialSpotlight",
		bool fingerHint = false, string fingerHintPrefab = "PressTut",
	    float focusOffsetX = 0f, float focusOffsetY = 0f, 
		float fingerHintOffsetX = 0f, float fingerHintOffsetY = 60f, 
		bool fingerHintFlip = false, float delay = -1f){

		// get the proper location of the object we are going to focus on
		Vector3 focusPos;
		if(isGUI){
			focusPos = LgNGUITools.GetScreenPosition(goTarget);
		}
		else{
			// WorldToScreen returns screen coordinates based on 0,0 being bottom left, so we need to transform those into NGUI center
			focusPos = CameraManager.Instance.WorldToScreen(CameraManager.Instance.CameraMain, goTarget.transform.position);
			focusPos = CameraManager.Instance.TransformAnchorPosition(focusPos, InterfaceAnchors.BottomLeft, InterfaceAnchors.Center);
		}

		// Adjust for custom offset
		focusPos = new Vector3(focusPos.x + focusOffsetX, focusPos.y + focusOffsetY, focusPos.z);
		
		// destroy the old object if it existed
		if(goSpotlight != null){
			GameObject.Destroy(goSpotlight);
		}

		if(goFingerHint != null){
			GameObject.Destroy(goFingerHint);
		}
		
		// create the spotlight
		GameObject goResource = Resources.Load(spotlightPrefab) as GameObject;
		string anchorName = "Anchor-" + anchor.ToString();
		goSpotlight = GameObjectUtils.AddChildWithPositionAndScale(GameObject.Find(anchorName), goResource);
		
		// Set the delay if defined
		if(delay > 0){
			goSpotlight.GetComponent<TweenAlpha>().delay = delay;
		}
		
		// move the spotlight into position
		focusPos.z = goSpotlight.transform.localPosition.z; // keep the default z-value of the spotlight
		goSpotlight.transform.localPosition = focusPos;

		// spawn finger hint
		if(fingerHint){
			GameObject fingerHintResource = (GameObject)Resources.Load(fingerHintPrefab);
			goFingerHint = GameObjectUtils.AddChildWithPositionAndScale(GameObject.Find(anchorName), fingerHintResource);
			focusPos.z = goFingerHint.transform.localPosition.z;
			focusPos.y = focusPos.y + fingerHintOffsetY; //offset in Y so the finger hint doesn't overlap the image
			focusPos.x = focusPos.x + fingerHintOffsetX;
			goFingerHint.transform.localPosition = focusPos;

			if(fingerHintFlip){
				goFingerHint.transform.localScale = new Vector3(-1, 1, 1);
			}
		}
	}

	//--------------------------------------------------------------
	// ShowFingerHint()
	// Use this function if you only want to spawn finger hint
	//--------------------------------------------------------------
	protected void ShowFingerHint(GameObject goTarget, bool isGUI = false, 
		InterfaceAnchors anchor = InterfaceAnchors.Center, string fingerHintPrefab = "PressTut",
		float offsetFromCenter = 60.0f, float offsetFromCenterX = 0.0f, bool flipX = false){

		string anchorName = "Anchor-" + anchor.ToString();

		// get the proper location of the object we are going to focus on
		Vector3 focusPos;
		if(isGUI){
			focusPos = LgNGUITools.GetScreenPosition(goTarget);
		}
		else{
			// WorldToScreen returns screen coordinates based on 0,0 being bottom left, so we need to transform those into NGUI center
			focusPos = CameraManager.Instance.WorldToScreen(CameraManager.Instance.CameraMain, goTarget.transform.position);
			// Camera.main.WorldToScreenPoint( goTarget.transform.position );

			focusPos = CameraManager.Instance.TransformAnchorPosition(focusPos, 
				InterfaceAnchors.BottomLeft, InterfaceAnchors.Center);
		}

		if(goFingerHint != null){
			GameObject.Destroy(goFingerHint);
		}

		GameObject fingerHintResource = (GameObject)Resources.Load(fingerHintPrefab);
		goFingerHint = GameObjectUtils.AddChildWithPositionAndScale(GameObject.Find(anchorName), fingerHintResource);
		focusPos.x = focusPos.x + offsetFromCenterX;
		focusPos.y = focusPos.y + offsetFromCenter; //offset in Y so the finger hint doesn't overlap the image
		focusPos.z = goFingerHint.transform.localPosition.z;
		goFingerHint.transform.localPosition = focusPos;

		if(flipX){
			goFingerHint.transform.localScale = new Vector3(-1, 1, 1);
		}

	}

	protected void RemoveFingerHint(){
		if(goFingerHint != null){
			GameObject.Destroy(goFingerHint);
		}
	}
	
	//---------------------------------------------------
	// RemoveSpotlight()
	// Removes the current spotlight object.
	//---------------------------------------------------		
	protected void RemoveSpotlight(){
		if(goSpotlight != null){
			GameObject.Destroy(goSpotlight);
		}
	}	
	
	//---------------------------------------------------
	// RemovePopup()
	// Removes the current popup object.
	//---------------------------------------------------		
	protected void RemovePopup(){
		if(goPopup != null){
			GameObject.Destroy(goPopup);
		}
	}	

	//---------------------------------------------------
	// ShowPopup()
	// Display the tutorial popup
	// Option Params:
	//	Message(string): the text you want to display
	//	SpriteAtlas(string): name of the atlas that the sprite is from. Required if loading an image
	//	SpriteName(string): name of the image
	//	Button1Callback(function): action to do when button is clicked
	//	Button1Label(string): what does the button say
	//	ShrinkBgToFitText(bool): default to T. background size is automatically adjusted to fit label
	//---------------------------------------------------
	protected void ShowPopup(string popupKey, Vector3 location, Hashtable option=null){
		if(goPopup){
			GameObject.Destroy(goPopup);
		}

		if(option == null){
			option = new Hashtable();
		}

		Vector3 newPos = location;

		if(!option.ContainsKey(TutorialPopupFields.Message)){
			// get text to display from tutorial key + step
			string strText = Localization.Localize(GetKey() + "_" + GetStep());
			option.Add(TutorialPopupFields.Message, strText);
		}

		if(!option.ContainsKey(TutorialPopupFields.ShrinkBgToFitText))
			option.Add(TutorialPopupFields.ShrinkBgToFitText, false);

		// create the popup
		GameObject goResource = Resources.Load(popupKey) as GameObject;
		goPopup = GameObjectUtils.AddChildWithPositionAndScale(GameObject.Find("Anchor-Center"), goResource);
//		newPos.z = goPopup.transform.position.z; // keep the default z-value
		goPopup.transform.localPosition = newPos;
		
		//feed the script the option hashtable		
		TutorialPopup script = goPopup.GetComponent<TutorialPopup>();
		script.Init(option);
	}

	public void ShowRetentionPet(bool isFlipped, Vector3 position,
	                             bool isButton = false, GameObject buttonTarget = null, string buttonFunctionName = null){
		GameObject goResource = Resources.Load("TutorialRetentionPet") as GameObject;
		goRetentionPet = GameObjectUtils.AddChild(GameObject.Find("Anchor-Center"), goResource);
		goRetentionPet.transform.localPosition = position;
		if(isFlipped){
			goRetentionPet.transform.localScale = new Vector3(-1f, 1f, 1f);
		}

		if(isButton){
			goRetentionPet.collider.enabled = true;
			UIButtonMessage message = goRetentionPet.GetComponent<UIButtonMessage>();
			message.enabled = true;
			message.target = buttonTarget;
			message.functionName = buttonFunctionName;
		}
		else{
			goRetentionPet.collider.enabled = false;
			UIButtonMessage message = goRetentionPet.GetComponent<UIButtonMessage>();
			message.enabled = false;
		}
	}

	public void RemoveRetentionPet(){
		if(goRetentionPet != null){
			GameObject.Destroy(goRetentionPet);
		}
	}
}
