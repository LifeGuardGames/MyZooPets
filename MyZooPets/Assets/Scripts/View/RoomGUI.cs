using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class RoomGUI : MonoBehaviour {
	
	// TODO LOTS OF UNUSED TEXTURES AND VARIABLES, DISCUSS WITH JASON AND CLEAN UP
	
	public GameObject notificationUIManagerObject;
	public GUISkin defaultSkin;

	public bool isDebug;

	// native dimensions
    private const float NATIVE_WIDTH = 1280.0f;
    private const float NATIVE_HEIGHT = 800.0f;

	//Crazy long Texture bundle	
	public Texture2D guiPanelFill;
	
	public Texture2D guiPanelLevel;
	public Texture2D guiPanelStars;
	public Texture2D guiPanelStats;
	
	//
	public Texture2D textureSwap;
	public Texture2D starTexture;
	public Texture2D tierBarTexture;
	public Texture2D starBarTexture;
	public Texture2D statBarTexture;
	public Texture2D itemBarTexture;
	public Texture2D foodIcon;
	public Texture2D healthIcon;
	public Texture2D moodIcon;
	public Texture2D progressBarFrame;
	public Texture2D progressBarFill;
	public Texture2D statBarVerFillGreen;
	public Texture2D statBarVerFillYellow;
	public Texture2D statBarVerFillRed;
	public Texture2D statBarVerFrame;
	public Texture2D inhalerTexture;
	public Texture2D emInhalerTexture;
	public Texture2D appleTexture;
	public Texture2D sandwichTexture;
	public Texture2D optionIconTexture;
	public Texture2D optionMenuTexture;

	//GUI style for Texts on screen
	public GUIStyle starTextStyle;
	public GUIStyle expreTextStyle;
	public GUIStyle tierTextStyle;
	public GUIStyle itemCountTextStyle;

	//4 stat indicator
	public float progress;
	public float food;
	public float mood;
	public float health;

	//LTRects for LeanTween movement for all GUI Objects
	private LTRect TopGuiRect = new LTRect (0, 0, 1200, 100);
	private LTRect LeftGuiRect = new LTRect (0, 0, 100, 800);
	private LTRect optionRect = new LTRect(1150, 700, 0, 0);	//TODO wonky placeholder;
	private LTRect RightArrowRect; // only the x-value is used

	//Positions/Offsets for all GUI elements
	private Vector2 optionLoc;
	private Vector2 tierBarloc;
	private Vector2 starBarloc;
	private Vector2 healthBarloc;
	private Vector2 moodBarloc;
	private Vector2 foodBarloc;
	private Vector2 tierTextOffset = new Vector2(25, 12);
	private Vector2 starIconOffset = new Vector2(10, 4);
	private Vector2 starTextOffset = new Vector2(90, 18);
	private Vector2 healthIconOffset = new Vector2(5, 18);
	private Vector2 healthBarOffset = new Vector2(60, 15);
	private Vector2 moodIconOffset = new Vector2(5, 18);
	private Vector2 moodBarOffset = new Vector2(60, 15);
	private Vector2 foodIconOffset = new Vector2(3, 20);
	private Vector2 foodbarOffset = new Vector2(60, 15);
	private Vector2 progressBarOffset = new Vector2(150, 11);
	private Vector2 progressTextOffset = new Vector2(230, 12);
	public Vector2 healthIconSize = new Vector2(60,60);
	public Vector2 moodIconSize = new Vector2(60,60);
	public Vector2 foodIconSize = new Vector2(60,60);
	public LTRect foodIconRect;
	public LTRect healthIconRect;
	public LTRect moodIconRect;
	public LTRect starIconRect;

	//inventory
	private Inventory inventory;
	private ItemLogic itemlogic;

	//navigation arrows
	public GUIStyle blankButtonStyle;
	public Texture2D leftArrow;
	public Texture2D rightArrow;
	public UserNavigation userNavigation;

	//MISC
	private bool showOption = false;
	private bool emInhalerpicked = false;
	private Rect menuTextureRect;
	private NotificationUIManager notificationUIManager;
	private RoomGUIAnimator roomAnimator;
	private DiagnoseGUI diagnoseGUI;

	private string tierLevel;
	private string tierProgressText;
	private int nextLevelPoints; //the minimum points required to level up

	private string starCount;
	private int menuBoxHeight = 75;
	private int menuBoxWidth = 75;


	void Start(){

		inventory = GameObject.Find ("GameManager").GetComponent<Inventory>();
		itemlogic =  GameObject.Find("GameManager").GetComponent<ItemLogic>();
	//Reading & init from other classes
		notificationUIManager = notificationUIManagerObject.GetComponent<NotificationUIManager>();
		roomAnimator = this.GetComponent<RoomGUIAnimator>();

		progress = roomAnimator.DisplayPoints;
		food = roomAnimator.DisplayHunger;
		mood = roomAnimator.DisplayMood;
		health = roomAnimator.DisplayHealth;
	//preset item menu
		optionLoc = new Vector2(NATIVE_WIDTH/2 - optionMenuTexture.width/2, NATIVE_HEIGHT/2 - optionMenuTexture.height/2);

		//TOP GUI bar location updates
		tierBarloc = new Vector2(TopGuiRect.rect.x+ 0,TopGuiRect.rect.y+ 2);
		starBarloc = new Vector2(TopGuiRect.rect.x + 540,TopGuiRect.rect.y + 2);

		//LEFT GUI bar location updates
		healthBarloc = new Vector2(LeftGuiRect.rect.x+ 0,LeftGuiRect.rect.y+80);
	  	moodBarloc = new Vector2(LeftGuiRect.rect.x+0,LeftGuiRect.rect.y+180);
	  	foodBarloc = new Vector2(LeftGuiRect.rect.x+0,LeftGuiRect.rect.y+280);

		healthIconRect = new LTRect(healthBarloc.x + healthIconOffset.x,healthBarloc.y + healthIconOffset.y,healthIconSize.x,healthIconSize.y);
		moodIconRect = new LTRect(moodBarloc.x + moodIconOffset.x,moodBarloc.y+moodIconOffset.y,healthIconSize.x,healthIconSize.y);
		foodIconRect = new LTRect(foodBarloc.x + foodIconOffset.x,foodBarloc.y + foodIconOffset.y,foodIconSize.x,foodIconSize.y);
		starIconRect = new LTRect(starBarloc.x + starIconOffset.x,starBarloc.y + starIconOffset.y,60,60);

		RightArrowRect = new LTRect(NATIVE_WIDTH - rightArrow.width, 0, 0, 0); // only the x-value is used
	}

	void Update(){
		//don't draw until all data is loaded
		if(!LoadDataLogic.IsDataLoaded) return;

		//TOP GUI bar location updates
		tierBarloc = new Vector2(TopGuiRect.rect.x+ 0,TopGuiRect.rect.y+ 2);
		starBarloc = new Vector2(TopGuiRect.rect.x + 540,TopGuiRect.rect.y + 2);

		//LEFT GUI bar location updates
		healthBarloc = new Vector2(LeftGuiRect.rect.x+ 0,LeftGuiRect.rect.y+80);
	  	moodBarloc = new Vector2(LeftGuiRect.rect.x+0,LeftGuiRect.rect.y+180);
	  	foodBarloc = new Vector2(LeftGuiRect.rect.x+0,LeftGuiRect.rect.y+280);
	  	
		//Data reading from Data Manager
		progress = roomAnimator.DisplayPoints;
		food = roomAnimator.DisplayHunger;
		mood = roomAnimator.DisplayMood;
		health = roomAnimator.DisplayHealth;

		//points progress bar data
		tierLevel = "Lv " + (int)roomAnimator.LastLevel;
		nextLevelPoints = roomAnimator.NextLevelPoints;
		tierProgressText = roomAnimator.DisplayPoints + "/" + nextLevelPoints;

		//Star data
		starCount = roomAnimator.DisplayStars.ToString();

	}

	//Hide all GUIs
	public void HideGUIs(bool hideTop, bool hideLeft, bool hideMenu, bool hideOption){
		if(hideTop){
			LeanTween.move(TopGuiRect,new Vector2(0,-100),0.5f);
			LeanTween.move(starIconRect,new Vector2(555,-100),0.5f);
		}
		if(hideLeft){
			LeanTween.move(LeftGuiRect,new Vector2(-100,0),0.5f);
			LeanTween.move(healthIconRect,new Vector2(-100,100),0.5f);
			LeanTween.move(moodIconRect,new Vector2(-100,200),0.5f);
			LeanTween.move(foodIconRect,new Vector2(-100,300),0.5f);
		}
		if(hideOption){
			LeanTween.move(optionRect, new Vector2(1150, 850), 0.5f);
			LeanTween.move(RightArrowRect, new Vector2(NATIVE_WIDTH, 850), 0.5f);
		}
	}

	//Show all GUIs
	public void ShowGUIs(){
		LeanTween.move(TopGuiRect,new Vector2(0,0),0.5f);
		LeanTween.move(LeftGuiRect,new Vector2(0,0),0.5f);
		LeanTween.move(optionRect, new Vector2(1150, 700), 0.5f);
		LeanTween.move(RightArrowRect, new Vector2(NATIVE_WIDTH - rightArrow.width, 850), 0.5f);
		LeanTween.move(healthIconRect,new Vector2(5,100),0.5f);
		LeanTween.move(moodIconRect,new Vector2(5,200),0.5f);
		LeanTween.move(foodIconRect,new Vector2(5,300),0.5f);	
		LeanTween.move(starIconRect,new Vector2(555,5),0.5f);

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
		GUI.DrawTexture(new Rect(743, 22, 132 * Mathf.Clamp01(health/100), 36), progressBarFill, ScaleMode.StretchToFill, true, 1f);	//TODO-s Crop them
		GUI.Label(new Rect(773, 20, 60, 60), health.ToString());
		GUI.DrawTexture(new Rect(932, 22, 132 * Mathf.Clamp01(mood/100), 36), progressBarFill, ScaleMode.StretchToFill, true, 1f);	//TODO-s Crop them
		GUI.Label(new Rect(962, 20, 60, 60), mood.ToString());
		GUI.DrawTexture(new Rect(1121, 22, 132 * Mathf.Clamp01(food/100), 36), progressBarFill, ScaleMode.StretchToFill, true, 1f);	//TODO-s Crop them
		GUI.Label(new Rect(1151, 20, 60, 60), food.ToString());
		GUI.DrawTexture(new Rect(699, 6, guiPanelStats .width, guiPanelStats.height), guiPanelStats);

		// Exit to yard button
		if(Application.loadedLevelName == "NewBedRoom"){
			if(GUI.Button(new Rect(optionRect.rect.x - 50,optionRect.rect.y, 150, 90), "Yard")){
				Application.LoadLevel("Yard");
			}
		}
		else if(Application.loadedLevelName == "Yard"){
			if(GUI.Button(new Rect(optionRect.rect.x - 50,optionRect.rect.y, 150, 90), "Room")){
				Application.LoadLevel("NewBedRoom");
			}
		}

		//Temp store Button
		if(GUI.Button(new Rect(optionRect.rect.x - 220,optionRect.rect.y, 150, 90),"Store")){
			GameObject.Find("StoreGUI").GetComponent<StoreGUI>().showStore();
		}

		// Diary button
		if(GUI.Button(new Rect(optionRect.rect.x - 390,optionRect.rect.y, 160, 90),"Notes")){
			GameObject.Find("DiaryGUI").GetComponent<DiaryGUI>().DiaryClicked();
		}

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
			}
			if(GUI.Button(new Rect(500,700,200,100),"+ stats")){
				DataManager.SubtractHunger(50);
				DataManager.SubtractMood(50);
				DataManager.SubtractHealth(50);
				DataManager.SubtractStars(50);
				notificationUIManager.GameOverRewardMessage(300, 250, null, null);
			}
		}

		// navigation arrows
        if(GUI.Button(new Rect(LeftGuiRect.rect.x, NATIVE_HEIGHT / 2, leftArrow.width, leftArrow.height), leftArrow, blankButtonStyle)){
        	userNavigation.ToTheLeft();
        }
        if(GUI.Button(new Rect(RightArrowRect.rect.x, NATIVE_HEIGHT / 2, rightArrow.width, rightArrow.height), rightArrow, blankButtonStyle)){
        	userNavigation.ToTheRight();
        }

	}
}
