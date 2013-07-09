using UnityEngine;
using System.Collections;

public class DiagnoseGUI : MonoBehaviour {
	
	public Texture2D txPanel;
	public GameObject spritePet;
	public GUISkin skin;

	//button texture
	public GUIStyle buttonBlankStyle;
	public Texture2D buttonGreen;
	public Texture2D buttonYellow;
	public Texture2D buttonRed;
	
	//timer texture
	public Texture2D timerFrame;
	public Texture2D timerFiller;
	
	//progress bar texture
	public Texture2D statBarVerFillGreen;
	public Texture2D statBarVerFillYellow;
	public Texture2D statBarVerFillRed;
	public Texture2D statBarTexture;

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
	private Vector2 timerTxLoc = new Vector2(531, 166);
	private Vector2 timerBarOffset = new Vector2(55, 146);

	private float timer; //count timer to time the user
	private bool pickedUp; //True: inhaler picked up by user, False: stationary
	private AsthmaStage chosenStage; //what stage did the user choose

	private Texture2D currentTexture; //TODO: this will need to be replaced with animation
	private bool buttonClicked; //used to prevent buttons from clicking more than once
	private bool isActive; //True: game in process, False: game over
	private bool showTimerProgress; //True: display count down meter, False: hide it

	// native dimensions
    private const float NATIVE_WIDTH = 1280.0f;
    private const float NATIVE_HEIGHT = 800.0f;

    //Button dimensions
    private const float BUTTON_WIDTH = 518;
    private const float BUTTON_HEIGHT = 150;

    //progress bar max value
    private const int MAX_VALUE = 10;
    private const int BAR_LENGTH = 330;

    private const int INHALER_SIZE = 75;

    //testing
    private GUIStyle buttonStyle = new GUIStyle();
	
	void Start(){
		timer = 10; //10 seconds game

		diagnoseFinalPosition = new Vector2(NATIVE_WIDTH/2, NATIVE_HEIGHT+100);
		diagnoseInitPosition = new Vector2(NATIVE_WIDTH/2, 100);
		diagnoseRect = new LTRect(diagnoseInitPosition.x, diagnoseInitPosition.y, 600, 600);
		inhalerFinalPosition = new Vector2(NATIVE_WIDTH/2, 100);
		inhalerInitPosition = new Vector2(NATIVE_WIDTH/2, -100);
        inhalerRect = new LTRect(inhalerInitPosition.x, inhalerInitPosition.y, INHALER_SIZE, INHALER_SIZE);

		buttonClicked = false;
		isActive = true;

		DiagnoseGameLogic.Init();
		switch(DiagnoseGameLogic.CurrentStage){
			case AsthmaStage.OK:
				currentTexture = buttonGreen;
				spritePet.GetComponent<tk2dSprite>().SetSprite("OkPet");
			break;
			case AsthmaStage.Sick:
				currentTexture = buttonYellow;
				spritePet.GetComponent<tk2dSprite>().SetSprite("SickPet");
			break;
			case AsthmaStage.Attack:
				currentTexture = buttonRed;
				spritePet.GetComponent<tk2dSprite>().SetSprite("AttackPet");
			break;
		}
	}

	void Update(){
		if(isActive){
			timer -= Time.deltaTime;
			if(timer <= 0){
				isActive = false;
				HideGUIPanel(false);
				RewardNotification(false);
			}
		}
	}
	
	void OnGUI(){
		GUI.skin = skin;
		// Proportional scaling
		if (NATIVE_WIDTH != Screen.width || NATIVE_HEIGHT != Screen.height){
            float horizRatio = Screen.width/NATIVE_WIDTH;
            float vertRatio = Screen.height/NATIVE_HEIGHT;
            GUI.matrix = Matrix4x4.TRS(new Vector3(0, 0, 0), Quaternion.identity, 
            	new Vector3(horizRatio, vertRatio, 1));
		}

		//=========Diagnose symptoms panel=============
		GUI.BeginGroup(diagnoseRect.rect, txPanel);
		GUI.Label(new Rect(0,0, 600, 100), "How is your pet feeling?", diagnoseStyle);
		if(GUI.Button(new Rect(40, 90, BUTTON_WIDTH, BUTTON_HEIGHT), buttonGreen, buttonBlankStyle)){
			chosenStage = AsthmaStage.OK;
			Clicked();
		}
		if(GUI.Button(new Rect(40, 255, BUTTON_WIDTH, BUTTON_HEIGHT), buttonYellow, buttonBlankStyle)){
			chosenStage = AsthmaStage.Sick;
			Clicked();			
		}
		if(GUI.Button(new Rect(40, 420, BUTTON_WIDTH, BUTTON_HEIGHT), buttonRed, buttonBlankStyle)){
			chosenStage = AsthmaStage.Attack;
			Clicked();	
		}
		GUI.EndGroup();
		//=============================================
		
		//=====Timer progress bar======
		if(isActive){			
			GUI.DrawTexture(new Rect(timerTxLoc.x + 57, timerTxLoc.y + 147, timerFiller.width, timerFiller.height), timerFiller);

			if(timer > 8){
				GUI.DrawTexture(new Rect(timerTxLoc.x + timerBarOffset.x, timerTxLoc.y + timerBarOffset.y +(BAR_LENGTH-BAR_LENGTH*timer/MAX_VALUE),
					57, BAR_LENGTH * Mathf.Clamp01(timer/MAX_VALUE)),statBarVerFillGreen, ScaleMode.StretchToFill, true, 1f);
			}else if(timer > 5){
				GUI.DrawTexture(new Rect(timerTxLoc.x + timerBarOffset.x, timerTxLoc.y + timerBarOffset.y +(BAR_LENGTH-BAR_LENGTH*timer/MAX_VALUE),
					57, BAR_LENGTH * Mathf.Clamp01(timer/MAX_VALUE)),statBarVerFillYellow, ScaleMode.StretchToFill, true, 1f);
			}else{
				GUI.DrawTexture(new Rect(timerTxLoc.x + timerBarOffset.x, timerTxLoc.y + timerBarOffset.y +(BAR_LENGTH-BAR_LENGTH*timer/MAX_VALUE),
					57, BAR_LENGTH * Mathf.Clamp01(timer/MAX_VALUE)),statBarVerFillRed, ScaleMode.StretchToFill, true, 1f);
			}
			
			GUI.DrawTexture(new Rect(timerTxLoc.x, timerTxLoc.y, timerFrame.width, timerFrame.height), timerFrame);
		}
		//=============================
		
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
							RewardNotification(true);
							
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
			
			if(DiagnoseGameLogic.IsThisStageCorrect(chosenStage)){

				if(chosenStage.Equals(AsthmaStage.OK)){
					isActive = false;
					RewardNotification(true);
				}
				HideGUIPanel(true);
			}else{
				//wrong stage
				//nice try notification
				isActive = false;
				HideGUIPanel(false);
				RewardNotification(false);
			}
		}
	}

	private void RewardNotification(bool moreReward){
		int rewardStars;
		int rewardPoints;
		if(moreReward){
			rewardStars = 200;
			rewardPoints = 400;
		}else{
			rewardStars = 150;
			rewardPoints = 150;
		}

		DataManager.AddPoints(rewardPoints);
		DataManager.AddStars(rewardStars);
		notificationUIManager.GameOverRewardMessage(rewardStars, rewardPoints,
			delegate(){
				Application.LoadLevel("NewBedRoom");
			});
	}

	private void ShowInhaler(){
		Hashtable optional = new Hashtable();
		optional.Add("ease", LeanTweenType.easeInOutQuad);
		LeanTween.move(inhalerRect, inhalerFinalPosition, 0.5f, optional);
	}

	//hide the game panel after button has been clicked
	private void HideGUIPanel(bool isAnswerCorrect){
		Hashtable optional = new Hashtable();
		if(isAnswerCorrect && (chosenStage.Equals(AsthmaStage.Sick) || chosenStage.Equals(AsthmaStage.Attack))){
			optional.Add("onCompleteTarget", gameObject);
			optional.Add("onComplete", "ShowInhaler");	
		}
		
		optional.Add("ease", LeanTweenType.easeInOutQuad);
		LeanTween.move(diagnoseRect, diagnoseFinalPosition, 0.5f, optional);
	}
}
