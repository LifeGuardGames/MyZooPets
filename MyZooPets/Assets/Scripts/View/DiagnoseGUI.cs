using UnityEngine;
using System.Collections;

public class DiagnoseGUI : MonoBehaviour {
	
	public Texture2D txPanel;

	//stages texture
	public Texture2D green;
	public Texture2D yellow;
	public Texture2D red;

	//progress bar texture
	public Texture2D statBarVerFillGreen;
	public Texture2D statBarVerFillYellow;
	public Texture2D statBarVerFillRed;
	public Texture2D statBarTexture;
	public Texture2D statBarVerFrame;

	public Texture2D inhalerTexture;
	public GUIStyle diagnoseStyle = new GUIStyle();
	public NotificationUIManager notificationUIManager; //reference 
	
	//Lean Tween positions
	private LTRect diagnoseRect;
	private LTRect inhalerRect;
	private Vector2 diagnoseInitPosition;
	private Vector2 diagnoseFinalPosition;
	private Vector2 inhalerInitPosition;
	private Vector2 inhalerFinalPosition;

	//progress bar positions
	private Vector2 timerBarLoc;
	private Vector2 timerBarOffset;

	private float timer; //count timer to time the user
	private bool pickedUp; //True: inhaler picked up by user, False: stationary
	private AsthmaStage chosenStage; //what stage did the user choose

	private Texture2D currentTexture; //TO DO: this will need to be replaced with animation
	private bool buttonClicked; //used to prevent buttons from clicking more than once
	private bool isActive; //True: game in process, False: game over
	// private bool showInhaler; //True: display inhaler, False: hide inhaler
	private bool showTimerProgress; //True: display count down meter, False: hide it

	// native dimensions
    private const float NATIVE_WIDTH = 1280.0f;
    private const float NATIVE_HEIGHT = 800.0f;

    //Button dimensions
    private const float BUTTON_WIDTH = 150;
    private const float BUTTON_HEIGHT = 150;

    //progress bar max value
    private const int MAX_VALUE = 30;
    private const int BAR_LENGTH = 200;

    private const int INHALER_SIZE = 75;

    //testing
    private GUIStyle buttonStyle = new GUIStyle();
	
	void Start(){
		timerBarLoc = new Vector2(50, 100);
		timerBarOffset = new Vector2(10, 10);

		timer = 30; //30 seconds game

		diagnoseFinalPosition = new Vector2(NATIVE_WIDTH/2, NATIVE_HEIGHT+100);
		diagnoseInitPosition = new Vector2(NATIVE_WIDTH/2, 100);
		diagnoseRect = new LTRect(diagnoseInitPosition.x, diagnoseInitPosition.y, 600, 600);
		inhalerFinalPosition = new Vector2(NATIVE_WIDTH/2, 100);
		inhalerInitPosition = new Vector2(NATIVE_WIDTH/2, -100);
        inhalerRect = new LTRect(inhalerInitPosition.x, inhalerInitPosition.y, INHALER_SIZE, INHALER_SIZE);

		buttonClicked = false;
		isActive = true;
		// showInhaler = false;

		DiagnoseGameLogic.Init();
		switch(DiagnoseGameLogic.CurrentStage){
			case AsthmaStage.OK:
				currentTexture = green;
			break;
			case AsthmaStage.Sick:
				currentTexture = yellow;
			break;
			case AsthmaStage.Attack:
				currentTexture = red;
			break;
		}
	}

	void Update(){
		if(isActive){
			timer -= Time.deltaTime;
			if(timer <= 0){
				isActive = false;
				HideGUIPanel();
				notificationUIManager.PopupTexture("nice try");
			}
		}
	}
	
	void OnGUI(){

		// Proportional scaling
		if (NATIVE_WIDTH != Screen.width || NATIVE_HEIGHT != Screen.height){
            float horizRatio = Screen.width/NATIVE_WIDTH;
            float vertRatio = Screen.height/NATIVE_HEIGHT;
            GUI.matrix = Matrix4x4.TRS(new Vector3(0, 0, 0), Quaternion.identity, 
            	new Vector3(horizRatio, vertRatio, 1));
		}

		//=====Timer progress bar======
		if(isActive){
			// GUI.DrawTexture(new Rect(timerBarLoc.x,timerBarLoc.y,100,100), statBarTexture);
			GUI.Label(new Rect(50, 80, 40, 20), "Timer");
			GUI.DrawTexture(new Rect(timerBarLoc.x + timerBarOffset.x,timerBarLoc.y + timerBarOffset.y,25,BAR_LENGTH),statBarVerFrame);
			if(timer > 20){
				GUI.DrawTexture(new Rect(timerBarLoc.x + timerBarOffset.x,timerBarLoc.y + timerBarOffset.y+(BAR_LENGTH-BAR_LENGTH*timer/MAX_VALUE),
					25, BAR_LENGTH * Mathf.Clamp01(timer/MAX_VALUE)),statBarVerFillGreen, ScaleMode.StretchToFill, true, 1f);
			}else if(timer > 10){
				GUI.DrawTexture(new Rect(timerBarLoc.x + timerBarOffset.x,timerBarLoc.y + timerBarOffset.y+(BAR_LENGTH-BAR_LENGTH*timer/MAX_VALUE),
					25, BAR_LENGTH * Mathf.Clamp01(timer/MAX_VALUE)),statBarVerFillYellow, ScaleMode.StretchToFill, true, 1f);
			}else{
				GUI.DrawTexture(new Rect(timerBarLoc.x + timerBarOffset.x,timerBarLoc.y + timerBarOffset.y+(BAR_LENGTH-BAR_LENGTH*timer/MAX_VALUE),
					25, BAR_LENGTH * Mathf.Clamp01(timer/MAX_VALUE)),statBarVerFillRed, ScaleMode.StretchToFill, true, 1f);
			}
		}
		//=============================

		//=========Diagnose symptoms panel=============
		GUI.BeginGroup(diagnoseRect.rect, txPanel);
		GUI.Label(new Rect(0,0, 600, 100), "Diagnose the symptoms!", diagnoseStyle);
		GUI.Label(new Rect(250, 150, 100, 50), "" + DiagnoseGameLogic.CurrentStage);
		if(GUI.Button(new Rect(30, 200, BUTTON_WIDTH, BUTTON_HEIGHT), green)){
			chosenStage = AsthmaStage.OK;
			Clicked();
		}
		if(GUI.Button(new Rect(220, 200, BUTTON_WIDTH, BUTTON_HEIGHT), yellow)){
			chosenStage = AsthmaStage.Sick;
			Clicked();			
		}
		if(GUI.Button(new Rect(410, 200, BUTTON_WIDTH, BUTTON_HEIGHT), red)){
			chosenStage = AsthmaStage.Attack;
			Clicked();	
		}
		GUI.EndGroup();
		//=============================================

		//===========Drag Drop inhaler logic==================================
		if(!pickedUp && isActive){
			if(GUI.RepeatButton(inhalerRect.rect, inhalerTexture, buttonStyle)){
				pickedUp = true;
			}	
		}

		if(pickedUp){
			GUI.DrawTexture(new Rect(Input.mousePosition.x-50, NATIVE_HEIGHT-Input.mousePosition.y-50, 
				INHALER_SIZE, INHALER_SIZE), inhalerTexture);
			if(Input.touchCount > 0){
				if(Input.GetTouch(0).phase == TouchPhase.Ended){
					Ray myRay = Camera.main.ScreenPointToRay(Input.mousePosition);
					RaycastHit hit;

					if(Physics.Raycast(myRay,out hit)){
						if(hit.collider.name == "SpritePet"){
							isActive = false;
							notificationUIManager.PopupTexture("award",0, 1000, 0, 0, 0);
							
						}else{
							//return to position
							// pickedUp = false;
						}
						
					}
					pickedUp = false;
				}
			}
		}
		//=====================================================================
	}

	//user chose one of the stages, so check it the user is correct
	private void Clicked(){
		if(!buttonClicked){
			buttonClicked = true;
			
			HideGUIPanel();
			if(DiagnoseGameLogic.IsThisStageCorrect(chosenStage)){
				if(chosenStage.Equals(AsthmaStage.OK)){
					isActive = false;
					notificationUIManager.PopupTexture("award",0, 1000, 0, 0, 0);
				}
			}else{
				//wrong stage
				//nice try notification
			}
		}
	}

	private void ShowInhaler(){
		Hashtable optional = new Hashtable();
		optional.Add("ease", LeanTweenType.easeInOutQuad);
		LeanTween.move(inhalerRect, inhalerFinalPosition, 0.5f, optional);
	}

	//hide the game panel after button has been clicked
	private void HideGUIPanel(){
		Hashtable optional = new Hashtable();
		if(chosenStage.Equals(AsthmaStage.Sick) || chosenStage.Equals(AsthmaStage.Attack)){
			optional.Add("onCompleteTarget", gameObject);
			optional.Add("onComplete", "ShowInhaler");	
		}
		
		optional.Add("ease", LeanTweenType.easeInOutQuad);
		LeanTween.move(diagnoseRect, diagnoseFinalPosition, 0.5f, optional);
	}

}
