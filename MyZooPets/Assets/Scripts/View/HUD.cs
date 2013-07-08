using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class HUD : MonoBehaviour {
	
	// TODO LOTS OF UNUSED TEXTURES AND VARIABLES, DISCUSS WITH JASON AND CLEAN UP
	
	public GameObject notificationUIManagerObject;
	public GUISkin defaultSkin;

	public bool isDebug;

	//Crazy long Texture bundle	
	public Texture2D guiPanelFill;
	public Texture2D guiPanelLevel;
	public Texture2D guiPanelStars;
	public Texture2D guiPanelStats;
	public Texture2D foodIcon;
	public Texture2D healthIcon;
	public Texture2D moodIcon;
	public Texture2D starIcon;
	public Texture2D progressBarFill;
	
	//GUI style for Texts on screen
	public GUIStyle starTextStyle;
	public GUIStyle expreTextStyle;
	public GUIStyle tierTextStyle;

	//Positions/Offsets for all GUI elements
	public LTRect foodIconRect;
	public LTRect healthIconRect;
	public LTRect moodIconRect;
	public LTRect starIconRect;

	//navigation arrows
	public GUIStyle blankButtonStyle;
	public Texture2D leftArrow;
	public Texture2D rightArrow;
	public UserNavigation userNavigation;

	// native dimensions
    private const float NATIVE_WIDTH = 1280.0f;
    private const float NATIVE_HEIGHT = 800.0f;

    //LTRects for LeanTween movement for all GUI Objects
	private LTRect statsRect = new LTRect (0, 0, 1280, 75);
	private LTRect RightArrowRect; // only the x-value is used

	//MISC
	private bool showOption = false;
	private NotificationUIManager notificationUIManager;
	private HUDAnimator animator;
	private DiagnoseGUI diagnoseGUI;

	//stat indicator
	private float progress;
	private float food;
	private float mood;
	private float health;
	private string tierLevel;
	private string tierProgressText;
	private int nextLevelPoints; //the minimum points required to level up
	private string starCount;


	void Start(){

	//Reading & init from other classes
		notificationUIManager = notificationUIManagerObject.GetComponent<NotificationUIManager>();
		animator = this.GetComponent<HUDAnimator>();

		progress = animator.DisplayPoints;
		food = animator.DisplayHunger;
		mood = animator.DisplayMood;
		health = animator.DisplayHealth;

		healthIconRect = new LTRect(710, 20, healthIcon.width, healthIcon.height);
		moodIconRect = new LTRect(900, 20, moodIcon.width, moodIcon.height);
		foodIconRect = new LTRect(1085, 20, foodIcon.width, foodIcon.height);
		starIconRect = new LTRect(495, 15, starIcon.width, starIcon.height);

		// RightArrowRect = new LTRect(NATIVE_WIDTH - rightArrow.width, 0, 0, 0); // only the x-value is used
	}

	void Update(){
		//don't draw until all data is loaded
		if(!LoadDataLogic.IsDataLoaded) return;
	  	
		//Data reading from Data Manager
		progress = animator.DisplayPoints;
		food = animator.DisplayHunger;
		mood = animator.DisplayMood;
		health = animator.DisplayHealth;

		//points progress bar data
		tierLevel = "Lv " + (int)animator.LastLevel;
		nextLevelPoints = animator.NextLevelPoints;
		tierProgressText = animator.DisplayPoints + "/" + nextLevelPoints;

		//Star data
		starCount = animator.DisplayStars.ToString();

	}

	// //Hide all GUIs
	// public void HideGUIs(bool hideTop, bool hideLeft, bool hideMenu, bool hideOption){
	// 	// if(hideTop){
	// 	// 	LeanTween.move(TopGuiRect,new Vector2(0,-100),0.5f);
	// 	// 	LeanTween.move(starIconRect,new Vector2(555,-100),0.5f);
	// 	// }
	// 	// if(hideLeft){
	// 	// 	LeanTween.move(LeftGuiRect,new Vector2(-100,0),0.5f);
	// 	// 	LeanTween.move(healthIconRect,new Vector2(-100,100),0.5f);
	// 	// 	LeanTween.move(moodIconRect,new Vector2(-100,200),0.5f);
	// 	// 	LeanTween.move(foodIconRect,new Vector2(-100,300),0.5f);
	// 	// }
	// 	// if(hideOption){
	// 	// 	LeanTween.move(optionRect, new Vector2(1150, 850), 0.5f);
	// 	// 	LeanTween.move(RightArrowRect, new Vector2(NATIVE_WIDTH, 850), 0.5f);
	// 	// }
	// }

	// //Show all GUIs
	// public void ShowGUIs(){
	// 	// LeanTween.move(TopGuiRect,new Vector2(0,0),0.5f);
	// 	// LeanTween.move(LeftGuiRect,new Vector2(0,0),0.5f);
	// 	// LeanTween.move(optionRect, new Vector2(1150, 700), 0.5f);
	// 	// LeanTween.move(RightArrowRect, new Vector2(NATIVE_WIDTH - rightArrow.width, 850), 0.5f);
	// 	// LeanTween.move(healthIconRect,new Vector2(5,100),0.5f);
	// 	// LeanTween.move(moodIconRect,new Vector2(5,200),0.5f);
	// 	// LeanTween.move(foodIconRect,new Vector2(5,300),0.5f);	
	// 	// LeanTween.move(starIconRect,new Vector2(555,5),0.5f);

	// }

	public void Display(){
		Hashtable optional = new Hashtable();
		optional.Add("ease", LeanTweenType.easeOutElastic);
		LeanTween.move(statsRect, new Vector2(0, 0), 0.5f, optional); //show stats Rect
	}

	public void Hide(){
		Hashtable optional = new Hashtable();
		optional.Add("ease", LeanTweenType.easeInElastic);
		LeanTween.move(statsRect, new Vector2(0, -75), 0.5f, optional);
	}

	void OnGUI(){
		//don't draw until all data is loaded
		if(!LoadDataLogic.IsDataLoaded) return;
		GUI.skin = defaultSkin;
		GUI.depth = 1;

		// Proportional scaling
		if (NATIVE_WIDTH != Screen.width || NATIVE_HEIGHT != Screen.height){
            float horizRatio = Screen.width/NATIVE_WIDTH;
            float vertRatio = Screen.height/NATIVE_HEIGHT;
            GUI.matrix = Matrix4x4.TRS(new Vector3(0, 0, 0), Quaternion.identity, new Vector3(horizRatio, vertRatio, 1));
		}

		// Room GUI Positioning
		
		GUI.BeginGroup(statsRect.rect);
		// Level Panel
		GUI.DrawTexture(new Rect(113, 23, guiPanelFill.width, guiPanelFill.height), guiPanelFill);
		GUI.DrawTexture(new Rect(116, 22, 346 * Mathf.Clamp01(progress/nextLevelPoints), 36), progressBarFill, ScaleMode.ScaleAndCrop, true, 150/13);	//TODO-s Crop them
		GUI.DrawTexture(new Rect(6, 6, guiPanelLevel.width, guiPanelLevel.height), guiPanelLevel);
		GUI.Label(new Rect(15, 20, 200, 40), tierLevel, tierTextStyle);
		GUI.Label(new Rect(200, 20, 200, 40), tierProgressText, expreTextStyle);
		
		// Stars Panel
		GUI.DrawTexture(new Rect(486, 6, guiPanelStars.width, guiPanelStars.height), guiPanelStars);
		GUI.Label(new Rect(550, 20, 60, 60), starCount, starTextStyle);

		// Stats Panel
		GUI.DrawTexture(new Rect(730, 23, 533, guiPanelFill.height), guiPanelFill);	// generic filler
		GUI.DrawTexture(new Rect(699, 6, guiPanelStats .width, guiPanelStats.height), guiPanelStats);

		GUI.DrawTexture(new Rect(743, 22, 132 * Mathf.Clamp01(health/100), 36), progressBarFill, ScaleMode.StretchToFill, true, 1f);	//TODO-s Crop them
		GUI.Label(new Rect(773, 20, 60, 60), health.ToString());
		GUI.DrawTexture(new Rect(932, 22, 132 * Mathf.Clamp01(mood/100), 36), progressBarFill, ScaleMode.StretchToFill, true, 1f);	//TODO-s Crop them
		GUI.Label(new Rect(962, 20, 60, 60), mood.ToString());
		GUI.DrawTexture(new Rect(1121, 22, 132 * Mathf.Clamp01(food/100), 36), progressBarFill, ScaleMode.StretchToFill, true, 1f);	//TODO-s Crop them
		GUI.Label(new Rect(1151, 20, 60, 60), food.ToString());
		
		GUI.DrawTexture(starIconRect.rect, starIcon);
		GUI.DrawTexture(healthIconRect.rect, healthIcon);
		GUI.DrawTexture(moodIconRect.rect, moodIcon);
		GUI.DrawTexture(foodIconRect.rect, foodIcon);
		GUI.EndGroup();
		// // Exit to yard button
		// if(Application.loadedLevelName == "NewBedRoom"){
		// 	if(GUI.Button(new Rect(optionRect.rect.x - 50,optionRect.rect.y, 150, 90), "Yard")){
		// 		Application.LoadLevel("Yard");
		// 	}
		// }
		// else if(Application.loadedLevelName == "Yard"){
		// 	if(GUI.Button(new Rect(optionRect.rect.x - 50,optionRect.rect.y, 150, 90), "Room")){
		// 		Application.LoadLevel("NewBedRoom");
		// 	}
		// }

		// //Temp store Button
		// if(GUI.Button(new Rect(optionRect.rect.x - 220,optionRect.rect.y, 150, 90),"Store")){
		// 	GameObject.Find("StoreGUI").GetComponent<StoreGUI>().showStore();
		// }

		// // Diary button
		// if(GUI.Button(new Rect(optionRect.rect.x - 390,optionRect.rect.y, 160, 90),"Notes")){
		// 	GameObject.Find("DiaryGUI").GetComponent<DiaryGUI>().DiaryClicked();
		// }

//		//Temp option Menu
//		if(GUI.Button(new Rect(optionRect.rect.x,optionRect.rect.y,90,90),optionIconTexture)){
//			showOption = !showOption;
//		}
//
//		//Options menu
//		if(showOption){
//			GUI.DrawTexture(new Rect(optionLoc.x,optionLoc.y,610,611),optionMenuTexture);
//			if(GUI.Button(new Rect(optionLoc.x,optionLoc.y,50,50),"X"))
//			{
//				showOption = false;
//			}
//			GUI.Button(new Rect(optionLoc.x+150,optionLoc.y+50,310,100),"Volume");
//			GUI.Button(new Rect(optionLoc.x+150,optionLoc.y+50+125,310,100),"Volume");
//			GUI.Button(new Rect(optionLoc.x+150,optionLoc.y+50+125*2,310,100),"Volume");
//			GUI.Button(new Rect(optionLoc.x+150,optionLoc.y+50+125*3,310,100),"Volume");
//		}

		
		//debuggin options
		if(isDebug){
			if(GUI.Button(new Rect(500,500,200,100),"+ stats")){
				DataManager.AddHunger(50);
				DataManager.AddHealth(50);
				DataManager.AddMood(50);
				DataManager.AddStars(50);
				DataManager.AddPoints(500);
				// Hide();
			}
			if(GUI.Button(new Rect(500,700,200,100),"+ stats")){
				DataManager.SubtractHunger(50);
				DataManager.SubtractMood(50);
				DataManager.SubtractHealth(50);
				DataManager.SubtractStars(50);
				// notificationUIManager.GameOverRewardMessage(300, 250, null, null);
				// Display();
			}
		}

		// // navigation arrows
  //       if(GUI.Button(new Rect(LeftGuiRect.rect.x, NATIVE_HEIGHT / 2, leftArrow.width, leftArrow.height), leftArrow, blankButtonStyle)){
  //       	userNavigation.ToTheLeft();
  //       }
  //       if(GUI.Button(new Rect(RightArrowRect.rect.x, NATIVE_HEIGHT / 2, rightArrow.width, rightArrow.height), rightArrow, blankButtonStyle)){
  //       	userNavigation.ToTheRight();
  //       }

	}
}
