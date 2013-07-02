using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class RoomGUI : MonoBehaviour {

	public GameObject notificationUIManagerObject;
	public GUISkin defaultSkin;

	// native dimensions
    private const float NATIVE_WIDTH = 1280.0f;
    private const float NATIVE_HEIGHT = 800.0f;

	//Crazy long Texture bundle
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
	public Texture2D plusTexture;
	public Texture2D minusTexture;

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
	private LTRect menuRect = new LTRect(0, NATIVE_HEIGHT - 100,1000, 105);
	private LTRect optionRect = new LTRect(1150, 700, 0, 0);	//TODO wonky placeholder;

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
	
	//inventory
	private Inventory inventory;
	private ItemLogic itemlogic;

	//MISC
	private bool isMenuExpanded = true;
	private bool showOption = false;
	private bool pickedUp = false;
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
	private int pickUpId= -1;


	void Start(){

		inventory = GameObject.Find ("GameManager").GetComponent<Inventory>();
		itemlogic =  GameObject.Find("GameManager").GetComponent<ItemLogic>();
	//Reading & init from other classes
		notificationUIManager = notificationUIManagerObject.GetComponent<NotificationUIManager>();
		roomAnimator = this.GetComponent<RoomGUIAnimator>();

		progress = roomAnimator.displayPoints;
		food = roomAnimator.displayHunger;
		mood = roomAnimator.displayMood;
		health = roomAnimator.displayHealth;
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
	}

	void Update(){
		//don't draw until all data is loaded
		if(!LoadDataLogic.IsDataLoaded) return;

		//Data reading from Data Manager
		progress = roomAnimator.displayPoints;
		food = roomAnimator.displayHunger;
		mood = roomAnimator.displayMood;
		health = roomAnimator.displayHealth;

		//points progress bar data
		tierLevel = Enum.GetName(typeof(Level), DataManager.CurrentLevel);
		nextLevelPoints = roomAnimator.nextLevelPoints;
		tierProgressText = roomAnimator.displayPoints + "/" + nextLevelPoints;

		//Star data
		starCount = roomAnimator.displayStars.ToString();

		if (pickedUp){
			// if dragging an item, don't treat this as a swipe
			SwipeDetection.CancelSwipe();
		}
	}

	//Hide all GUIs
	public void HideGUIs(bool hideTop, bool hideLeft, bool hideMenu, bool hideOption){
		if(hideTop){
			LeanTween.move(TopGuiRect,new Vector2(0,-100),0.5f);
		}
		if(hideLeft){
			LeanTween.move(LeftGuiRect,new Vector2(-100,0),0.5f);
		}
		if(hideMenu){
			LeanTween.move(menuRect,new Vector2(0,850),0.5f);
		}
		if(hideOption){
			LeanTween.move(optionRect, new Vector2(1150, 850), 0.5f);
		}
	}

	//Show all GUIs
	public void ShowGUIs(){
		LeanTween.move(TopGuiRect,new Vector2(0,0),0.5f);
		LeanTween.move(LeftGuiRect,new Vector2(0,0),0.5f);
		LeanTween.move(menuRect,new Vector2(0,700),0.5f);
		LeanTween.move(optionRect, new Vector2(1150, 700), 0.5f);
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

		//Room GUI Positioning

		//Progress Bar
		GUI.DrawTexture(new Rect(tierBarloc.x,tierBarloc.y,530,75), tierBarTexture);
		GUI.DrawTexture(new Rect(tierBarloc.x + progressBarOffset.x,tierBarloc.y+progressBarOffset.y,350,50),progressBarFrame);
		GUI.DrawTexture(new Rect(tierBarloc.x + progressBarOffset.x,tierBarloc.y+progressBarOffset.y,350 * Mathf.Clamp01(progress/nextLevelPoints),50),progressBarFill, ScaleMode.ScaleAndCrop, true, 150/13);
		GUI.Label(new Rect(tierBarloc.x + progressTextOffset.x,tierBarloc.y+progressTextOffset.y,200,40),tierProgressText,expreTextStyle);
		GUI.Label(new Rect(tierBarloc.x+tierTextOffset.x,tierBarloc.y+tierTextOffset.y,200,40),tierLevel,tierTextStyle);

		//Star Bar
		GUI.DrawTexture(new Rect(starBarloc.x,starBarloc.y,215,75), starBarTexture);
		GUI.DrawTexture(new Rect(starBarloc.x + starIconOffset.x,starBarloc.y + starIconOffset.y,60,60), starTexture, ScaleMode.ScaleToFit);
		GUI.Label(new Rect(starBarloc.x+starTextOffset.x,starBarloc.y+starTextOffset.y,60,60),starCount,starTextStyle);

		//Health Bar
		//Turns Yellow when health < 60
		//Turns Red when health < 30
		GUI.DrawTexture(new Rect(healthBarloc.x,healthBarloc.y,100,100), statBarTexture);
		GUI.DrawTexture(new Rect(healthBarloc.x + healthBarOffset.x,healthBarloc.y + healthBarOffset.y,25,70),statBarVerFrame);
		if(health > 60){
			GUI.DrawTexture(new Rect(healthBarloc.x + healthBarOffset.x,healthBarloc.y + healthBarOffset.y+(70-70*health/100),25, 70 * Mathf.Clamp01(health/100)),statBarVerFillGreen, ScaleMode.StretchToFill, true, 1f);
		}
		else if(health > 30){
			GUI.DrawTexture(new Rect(healthBarloc.x + healthBarOffset.x,healthBarloc.y + healthBarOffset.y+(70-70*health/100),25, 70 * Mathf.Clamp01(health/100)),statBarVerFillYellow, ScaleMode.StretchToFill, true, 1f);
		}
		else{
			GUI.DrawTexture(new Rect(healthBarloc.x + healthBarOffset.x,healthBarloc.y + healthBarOffset.y+(70-70*health/100),25, 70 * Mathf.Clamp01(health/100)),statBarVerFillRed, ScaleMode.StretchToFill, true, 1f);
		}
		GUI.DrawTexture(healthIconRect.rect,healthIcon, ScaleMode.ScaleToFit, true, 0f);

		//Mood Bar
		//Same as health bar
		GUI.DrawTexture(new Rect(moodBarloc.x,moodBarloc.y,100,100), statBarTexture);
		GUI.DrawTexture(new Rect(moodBarloc.x + moodBarOffset.x,moodBarloc.y+moodBarOffset.y,25,70),statBarVerFrame);
		if(mood > 60){
			GUI.DrawTexture(new Rect(moodBarloc.x + moodBarOffset.x,moodBarloc.y+moodBarOffset.y+(70-70*mood/100),25, 70 * Mathf.Clamp01(mood/100)),statBarVerFillGreen, ScaleMode.StretchToFill, true, 1f);
		}
		else if(mood > 30){
			GUI.DrawTexture(new Rect(moodBarloc.x + moodBarOffset.x,moodBarloc.y+moodBarOffset.y+(70-70*mood/100),25, 70 * Mathf.Clamp01(mood/100)),statBarVerFillYellow, ScaleMode.StretchToFill, true, 1f);
		}
		else{
			GUI.DrawTexture(new Rect(moodBarloc.x + moodBarOffset.x,moodBarloc.y+moodBarOffset.y+(70-70*mood/100),25, 70 * Mathf.Clamp01(mood/100)),statBarVerFillRed, ScaleMode.StretchToFill, true, 1f);
		}
		GUI.DrawTexture(moodIconRect.rect,moodIcon,ScaleMode.ScaleToFit, true, 0f);

		//Food Bar
		//Same as food bar
		GUI.DrawTexture(new Rect(foodBarloc.x,foodBarloc.y,100,100), statBarTexture);
		GUI.DrawTexture(new Rect(foodBarloc.x + foodbarOffset.x,foodBarloc.y + foodbarOffset.y,25,70),statBarVerFrame);
		if(food > 60){
			GUI.DrawTexture(new Rect(foodBarloc.x + foodbarOffset.x,foodBarloc.y + foodbarOffset.y+(70-70*food/100),25, 70 * Mathf.Clamp01(food/100)),statBarVerFillGreen, ScaleMode.StretchToFill, true, 1f);
		}
		else if(food > 30){
			GUI.DrawTexture(new Rect(foodBarloc.x + foodbarOffset.x,foodBarloc.y + foodbarOffset.y+(70-70*food/100),25, 70 * Mathf.Clamp01(food/100)),statBarVerFillYellow, ScaleMode.StretchToFill, true, 1f);
		}
		else{
			GUI.DrawTexture(new Rect(foodBarloc.x + foodbarOffset.x,foodBarloc.y + foodbarOffset.y+(70-70*food/100),25, 70 * Mathf.Clamp01(food/100)),statBarVerFillRed, ScaleMode.StretchToFill, true, 1f);
		}
		GUI.DrawTexture(foodIconRect.rect,foodIcon,ScaleMode.ScaleToFit, true, 0f);

		//get count of items owned
		int counter = 0;
		for(int i = 0;i< inventory.inventory.Length;i++){
			if(inventory.inventory[i]!=0) counter++;
		}
		//Extending Button Groups
		//Includes 4 items/Buttons for now.
		menuTextureRect = new Rect(menuRect.rect.x  - 900 + 80f * counter/*- (1000-((counter+1) * 85))*/, menuRect.rect.y - 10, menuRect.rect.width, menuRect.rect.height);
		GUI.DrawTexture(menuTextureRect, itemBarTexture);
		GUILayout.BeginArea(menuRect.rect, "");
		GUILayout.BeginHorizontal();

		counter =0;
		//implementing itemlogic
		for(int i = 0 ;i < itemlogic.items.Count; i++){
			if(i == pickUpId){
				textureSwap = null;
			}
			else{
				textureSwap = itemlogic.items[i].Texture;
			}
			if(inventory.inventory[i]!=0){
				if(GUILayout.RepeatButton(textureSwap, GUILayout.Height(menuBoxHeight), GUILayout.Width(menuBoxWidth))){
					pickedUp = true;
					pickUpId = i;
				}
				counter++;
				GUI.Label(new Rect(-10+counter*80-80,35,100,80),"x " + inventory.inventory[i].ToString(),itemCountTextStyle);
			}
		}

		//move in/out of item bar
		if(isMenuExpanded){
			if(GUILayout.Button(minusTexture, GUILayout.Height(menuBoxHeight), GUILayout.Width(menuBoxWidth))){
				isMenuExpanded = false;
				Hashtable optional = new Hashtable();
				optional.Add("ease", LeanTweenType.easeInOutQuad);
				LeanTween.move(menuRect, new Vector2(-80f * counter, NATIVE_HEIGHT - 100), 0.3f, optional);
			}
		}
		else{
			if(GUILayout.Button(plusTexture, GUILayout.Height(menuBoxHeight), GUILayout.Width(menuBoxWidth))){
				isMenuExpanded = true;
				Hashtable optional = new Hashtable();
				optional.Add("ease", LeanTweenType.easeInOutQuad);
				LeanTween.move(menuRect, new Vector2(0, NATIVE_HEIGHT - 100), 0.3f, optional);
			}
		}
		GUILayout.EndHorizontal();
		GUILayout.EndArea();

		//Temp Pick & Drag of items
		//TODO-w Refactor this somewhere else?
		//Do this when we create Backpack .etc
		if(pickedUp){
			// GUI.DrawTexture(new Rect(Input.mousePosition.x - menuBoxWidth / 2,NATIVE_HEIGHT- Input.mousePosition.y - menuBoxHeight / 2, menuBoxWidth,menuBoxHeight),itemlogic.items[pickUpId].Texture);
			Vector2 convertedGUIPos = GUIUtility.ScreenToGUIPoint(Input.mousePosition);
			GUI.DrawTexture(new Rect(convertedGUIPos.x - menuBoxWidth / 2,NATIVE_HEIGHT- convertedGUIPos.y - menuBoxHeight / 2, menuBoxWidth,menuBoxHeight),itemlogic.items[pickUpId].Texture);
			if(Input.touchCount > 0){
				if(Input.GetTouch(0).phase == TouchPhase.Ended){
					Ray myRay = Camera.main.ScreenPointToRay(Input.mousePosition);
					RaycastHit hit;

					if(Physics.Raycast(myRay,out hit)){
						if(hit.collider.name == "SpritePet" ||
							hit.collider.name == "PetHead" ||
							hit.collider.name == "PetTummy"){
							inventory.useItem(pickUpId);
						}
					}
					pickedUp = false;
					pickUpId = -1;
				}
			}
		}

		//just for testing
		//Delete after
		if(Input.GetMouseButtonUp(0)){
			pickedUp = false;
			pickUpId = -1;
		}

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
		if(GUI.Button(new Rect(0,0,100,100),"food + 50")){
			DataManager.AddHunger(50);
			DataManager.AddHealth(50);
			DataManager.AddMood(50);
		}
		if(GUI.Button(new Rect(500,0,100,100),"food + 50")){
			DataManager.SubtractHunger(50);
			DataManager.SubtractMood(50);
			DataManager.SubtractHealth(50);
		}

	}
}
