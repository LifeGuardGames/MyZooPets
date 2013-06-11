using UnityEngine;
using System.Collections;

public class RoomGUI : MonoBehaviour {
	
	private RoomGUIAnimator roomAnimator;
	
	public GameObject cameraMoveObject;
	private CameraMove cameraMove;
	
	public GameObject diagnoseGUIObject;
	private DiagnoseGUI diagnoseGUI;
	
	public GUISkin defaultSkin;
	
	// native dimensions
    private const float NATIVE_WIDTH = 1280.0f;
    private const float NATIVE_HEIGHT = 800.0f;
	
	private bool isMenuExpanded;
	private bool showOption = false;
	private bool inhalerpicked = false; 
	private bool emInhalerpicked = false;
	
	private Rect menuTextureRect;

	public Texture2D textureSwap1;
	public Texture2D textureSwap2;
	
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
	
	public GUIStyle starTextStyle;
	public GUIStyle expreTextStyle;
	public GUIStyle tierTextStyle;
	
	public float progress;
	public float food;
	public float mood;
	public float health;
		
	private LTRect TopGuiRect = new LTRect (0, 0, 1200, 100);
	private LTRect LeftGuiRect = new LTRect (0, 0, 100, 800);
	private LTRect menuRect;
	private LTRect optionRect = new LTRect(1150, 700, 0, 0);	//TODO wonky placeholder;
	
	//private Vector2 optionIconLoc = new Vector2(1150, 700);
	private Vector2 optionMenuLoc = new Vector2(500, 100);
	
	private Vector2 tierBarloc;
	private Vector2 tierTextOffset = new Vector2(25, 12);
	
	private Vector2 starBarloc;
	private Vector2 starIconOffset = new Vector2(10, 4);
	private Vector2 starTextOffset = new Vector2(90, 18);
	private string starCount;
	
	private Vector2 healthBarloc;
	private Vector2 healthIconOffset = new Vector2(5, 18);
	private Vector2 healthBarOffset = new Vector2(60, 15);
	
	private Vector2 moodBarloc;
	private Vector2 moodIconOffset = new Vector2(5, 18);
	private Vector2 moodBarOffset = new Vector2(60, 15);
	
	private Vector2 foodBarloc;
	private Vector2 foodIconOffset = new Vector2(3, 20);
	private Vector2 foodbarOffset = new Vector2(60, 15);
	
	private Vector2 progressBarOffset = new Vector2(150, 11);
	private Vector2 progressTextOffset = new Vector2(230, 12);
	
	private string tierLevel;
	private string tierProgressText;
	
	private int menuBoxHeight = 75;
	private int menuBoxWidth = 75;
	
	void Start(){
		cameraMove = cameraMoveObject.GetComponent<CameraMove>();
		roomAnimator = this.GetComponent<RoomGUIAnimator>();
		
		diagnoseGUI = diagnoseGUIObject.GetComponent<DiagnoseGUI>();
		
		progress = roomAnimator.displayPoints;
		food = roomAnimator.displayHunger;
		mood = roomAnimator.displayMood;
		health = roomAnimator.displayHealth;
		
		isMenuExpanded = true;
		menuRect = new LTRect(0, NATIVE_HEIGHT - 100, 1013, 105);	
	}
	
	void Update(){
		//TOP GUI bar location updates
		tierBarloc = new Vector2(TopGuiRect.rect.x+ 0,TopGuiRect.rect.y+ 2);
		starBarloc = new Vector2(TopGuiRect.rect.x + 540,TopGuiRect.rect.y + 2);
	
		//LEFT GUI bar location updates
		healthBarloc = new Vector2(LeftGuiRect.rect.x+ 0,LeftGuiRect.rect.y+80);
	  	moodBarloc = new Vector2(LeftGuiRect.rect.x+0,LeftGuiRect.rect.y+180);
	  	foodBarloc = new Vector2(LeftGuiRect.rect.x+0,LeftGuiRect.rect.y+280);
		
		//Data reading from Data Manager
		progress = roomAnimator.displayPoints;
		food = roomAnimator.displayHunger;
		mood = roomAnimator.displayMood;
		health = roomAnimator.displayHealth;
		
		tierLevel = "Tier 1";//TODO-s change this later
		tierProgressText = roomAnimator.displayPoints + "/50000";
		starCount = roomAnimator.displayStars.ToString();
	}
	
	public void HideGUIs(){
		LeanTween.move(TopGuiRect,new Vector2(0,-100),0.5f);
		LeanTween.move(LeftGuiRect,new Vector2(-100,0),0.5f);
		LeanTween.move(menuRect,new Vector2(0,850),0.5f);
		LeanTween.move(optionRect, new Vector2(1150, 850), 0.5f);
	}
	
	public void ShowGUIs(){
		LeanTween.move(TopGuiRect,new Vector2(0,0),0.5f);
		LeanTween.move(LeftGuiRect,new Vector2(0,0),0.5f);
		LeanTween.move(menuRect,new Vector2(0,700),0.5f);
		LeanTween.move(optionRect, new Vector2(1150, 700), 0.5f);
	}
	
	void OnGUI(){
		if(!SplashScreen.IsFinished) return; //don't draw until splash screen is done
		if(!LoadDataLogic.IsDataLoaded) return; //don't draw until all data is loaded

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
		GUI.DrawTexture(new Rect(tierBarloc.x + progressBarOffset.x,tierBarloc.y+progressBarOffset.y,350 * Mathf.Clamp01(progress/50000),50),progressBarFill, ScaleMode.ScaleAndCrop, true, 150/13);
		GUI.Label(new Rect(tierBarloc.x + progressTextOffset.x,tierBarloc.y+progressTextOffset.y,200,40),tierProgressText,expreTextStyle);
		GUI.Label(new Rect(tierBarloc.x+tierTextOffset.x,tierBarloc.y+tierTextOffset.y,200,40),tierLevel,tierTextStyle);
		
		//Star Bar		
		GUI.DrawTexture(new Rect(starBarloc.x,starBarloc.y,215,75), starBarTexture);
		GUI.DrawTexture(new Rect(starBarloc.x + starIconOffset.x,starBarloc.y + starIconOffset.y,60,60), starTexture, ScaleMode.ScaleToFit);
		GUI.Label(new Rect(starBarloc.x+starTextOffset.x,starBarloc.y+starTextOffset.y,60,60),starCount,starTextStyle);            
		
		//Health Bar
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
		GUI.DrawTexture(new Rect(healthBarloc.x + healthIconOffset.x,healthBarloc.y + healthIconOffset.y,60,60),healthIcon, ScaleMode.ScaleToFit, true, 0f);
		
		//Mood Bar	
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
		GUI.DrawTexture(new Rect(moodBarloc.x + moodIconOffset.x,moodBarloc.y+moodIconOffset.y,60,60),moodIcon,ScaleMode.ScaleToFit, true, 0f);
		
		//Food Bar
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
		GUI.DrawTexture(new Rect(foodBarloc.x + foodIconOffset.x,foodBarloc.y + foodIconOffset.y,60,60),foodIcon,ScaleMode.ScaleToFit, true, 0f);
		
		//Extending Button Groups
		menuTextureRect = new Rect(menuRect.rect.x - 600, menuRect.rect.y - 10, menuRect.rect.width, menuRect.rect.height);
		GUI.DrawTexture(menuTextureRect, itemBarTexture);
		GUILayout.BeginArea(menuRect.rect, "");
		GUILayout.BeginHorizontal("");
		if(GUILayout.Button(sandwichTexture, GUILayout.Height(menuBoxHeight), GUILayout.Width(menuBoxWidth))){
			DataManager.AddHealth(50);	
		}
		if(GUILayout.Button(appleTexture, GUILayout.Height(menuBoxHeight), GUILayout.Width(menuBoxWidth))){
			DataManager.SubtractHealth(40);
		}
		
		if(GUILayout.RepeatButton(textureSwap1, GUILayout.Height(menuBoxHeight), GUILayout.Width(menuBoxWidth))){
			inhalerpicked = true;
		}
		
		if(GUILayout.RepeatButton(textureSwap2, GUILayout.Height(menuBoxHeight), GUILayout.Width(menuBoxWidth))){
			emInhalerpicked = true;
		}
		
		if(isMenuExpanded){
			if(GUILayout.Button(minusTexture, GUILayout.Height(menuBoxHeight), GUILayout.Width(menuBoxWidth))){
				isMenuExpanded = false;
				Hashtable optional = new Hashtable();
				optional.Add("ease", LeanTweenType.easeInOutQuad);
				LeanTween.move(menuRect, new Vector2(-317, NATIVE_HEIGHT - 100), 0.3f, optional);
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
		
		//TODO-w Refactor this somewhere else?
		if(inhalerpicked){
			textureSwap1 = null; 
			GUI.DrawTexture(new Rect(Input.mousePosition.x-50,Screen.height- Input.mousePosition.y-50, menuBoxWidth,menuBoxHeight),inhalerTexture);
			if(Input.touchCount > 0){
				if(Input.GetTouch(0).phase == TouchPhase.Ended){
					Ray myRay = Camera.main.ScreenPointToRay(Input.mousePosition);
					RaycastHit hit;
					
					if(Physics.Raycast(myRay,out hit)){
						if(hit.collider.name == "SpritePet(Clone)"){
							print("You hit pet!");
							DataManager.AddPoints(1000);	
						}
					}			
				}
			}
		}
		
		if(emInhalerpicked){
			textureSwap2 = null; 
			GUI.DrawTexture(new Rect(Input.mousePosition.x-50,Screen.height- Input.mousePosition.y-50, menuBoxWidth,menuBoxHeight),emInhalerTexture);
		}
		
		if(Input.GetMouseButtonUp(0)){
			emInhalerpicked = false;
			textureSwap1 = inhalerTexture;
			inhalerpicked = false;
			textureSwap2 = emInhalerTexture;
		}
		
		if(GUI.Button(new Rect(optionRect.rect.x,optionRect.rect.y,90,90),optionIconTexture)){
			showOption = !showOption;
		}
		
		// TODO-s change this later
		if(GUI.Button(new Rect(optionRect.rect.x - 100,optionRect.rect.y ,90,90), "Diagnose Pet")){
			diagnoseGUI.DiagnoseClicked();
		}
		
		if(showOption){
			GUI.DrawTexture(new Rect(optionMenuLoc.x,optionMenuLoc.y,400,600),optionMenuTexture);
		}
	}
}
