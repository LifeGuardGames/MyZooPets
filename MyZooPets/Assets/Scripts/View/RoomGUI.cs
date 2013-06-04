using UnityEngine;
using System.Collections;

public class RoomGUI : MonoBehaviour {
	
	private RoomGUIAnimator roomAnimator;
	
	// native dimensions
    private const float NATIVE_WIDTH = 1280.0f;    //screen size 
    private const float NATIVE_HEIGHT = 800.0f;
	
	private bool isMenuExpanded;
	private LTRect menuRect;
	private Rect menuTextureRect;
	
	public Texture2D starTexture;
	public Texture2D tierBarTexture;
	public Texture2D starBarTexture;
	public Texture2D statBarTexture;
	public Texture2D itemBarTexture;
	
	public Texture2D roomTexture;
	
	public Texture2D foodIcon;
	public Texture2D healthIcon;
	public Texture2D moodIcon;
	
	public Texture2D demopet;
	public Texture2D progressBarFrame;
	public Texture2D progressBarFill;
	public Texture2D statBarVerFill;
	public Texture2D statBarVerFrame;
	
	public Texture2D inhalerTexture;
	public Texture2D emInhalerTexture;
	public Texture2D appleTexture;
	public Texture2D sandwichTexture;
	
	public Texture2D plusTexture;
	public Texture2D minusTexture;
	
	public GUIStyle starTextStyle;
	public GUIStyle expreTextStyle;
	public GUIStyle tierTextStyle;
	
	public float progress;
	public float food;
	public float mood;
	public float health;
		
	private Vector2 tierBarloc = new Vector2(0,2);
	private Vector2 tierTextOffset = new Vector2(25,12);
	
	private Vector2 starBarloc = new Vector2(540,2);
	private Vector2 starIconOffset = new Vector2(10,4);
	private Vector2 starTextOffset = new Vector2(90,18);
	private string starCount;
	
	private Vector2 healthBarloc = new Vector2(0,80);
	private Vector2 healthIconOffset = new Vector2(5,18);
	private Vector2 healthBarOffset = new Vector2(60,15);
	
	private Vector2 moodBarloc = new Vector2(0,180);
	private Vector2 moodIconOffset = new Vector2(5,18);
	private Vector2 moodBarOffset = new Vector2(60,15);
	
	private Vector2 foodBarloc = new Vector2(0,280);
	private Vector2 foodIconOffset = new Vector2(3,20);
	private Vector2 foodbarOffset = new Vector2(60,15);
	
	private Vector2 progressBarOffset = new Vector2(150,11);
	private Vector2 progressTextOffset = new Vector2(230,12);
	
	private string tierLevel;
	private string tierProgressText;
	
	private int menuBoxHeight = 75;
	private int menuBoxWidth = 75;
	
	void Start (){
		roomAnimator = this.GetComponent<RoomGUIAnimator>();
		
		progress = roomAnimator.displayPoints;
		food = roomAnimator.displayHunger;
		mood = roomAnimator.displayMood;
		health = roomAnimator.displayHealth;
		
		isMenuExpanded = true;
		menuRect = new LTRect(0, NATIVE_HEIGHT - 100, 1013, 105);
		
		
	}
	
	void Update (){
		progress = roomAnimator.displayPoints;
		food = roomAnimator.displayHunger;
		mood = roomAnimator.displayMood;
		health = roomAnimator.displayHealth;
		
		//TODO-s change this to read data
		tierLevel = "Tier 1";
		tierProgressText = roomAnimator.displayPoints + "/10000";
		starCount = roomAnimator.displayStars.ToString();
	}
	
	void OnGUI(){
	
		GUI.depth = 1;
		if (NATIVE_WIDTH != Screen.width || NATIVE_HEIGHT != Screen.height){     //porpotional scaling
            float horizRatio = Screen.width/NATIVE_WIDTH;
            float vertRatio = Screen.height/NATIVE_HEIGHT;
            GUI.matrix = Matrix4x4.TRS(new Vector3(0, 0, 0), Quaternion.identity, new Vector3(horizRatio, vertRatio, 1));
		}
//		GUI.DrawTexture(new Rect(330,300,500,500), demopet);   //temp demo pet

		//Room GUI Positioning 
		
		//Progress Bar
		GUI.DrawTexture(new Rect(tierBarloc.x,tierBarloc.y,530,75), tierBarTexture);
		GUI.DrawTexture(new Rect(tierBarloc.x + progressBarOffset.x,tierBarloc.y+progressBarOffset.y,350,50),progressBarFrame);
		GUI.DrawTexture(new Rect(tierBarloc.x + progressBarOffset.x,tierBarloc.y+progressBarOffset.y,350 * Mathf.Clamp01(progress/100),50),progressBarFill, ScaleMode.ScaleAndCrop, true, 150/13);
		GUI.TextField(new Rect(tierBarloc.x + progressTextOffset.x,tierBarloc.y+progressTextOffset.y,200,40),tierProgressText,expreTextStyle);
		GUI.TextField(new Rect(tierBarloc.x+tierTextOffset.x,tierBarloc.y+tierTextOffset.y,200,40),tierLevel,tierTextStyle);
		
		//Star Bar		
		GUI.DrawTexture(new Rect(starBarloc.x,starBarloc.y,215,75), starBarTexture);
		GUI.DrawTexture(new Rect(starBarloc.x + starIconOffset.x,starBarloc.y + starIconOffset.y,60,60), starTexture, ScaleMode.ScaleToFit);
		GUI.TextField(new Rect(starBarloc.x+starTextOffset.x,starBarloc.y+starTextOffset.y,60,60),starCount,starTextStyle);            
		
		//Health Bar
		GUI.DrawTexture(new Rect(healthBarloc.x,healthBarloc.y,100,100), statBarTexture);
		GUI.DrawTexture(new Rect(healthBarloc.x + healthBarOffset.x,healthBarloc.y + healthBarOffset.y,25,70),statBarVerFrame);
		GUI.DrawTexture(new Rect(healthBarloc.x + healthBarOffset.x,healthBarloc.y + healthBarOffset.y+(70-70*health/100),25, 70 * Mathf.Clamp01(health/100)),statBarVerFill, ScaleMode.ScaleAndCrop, true, 25/70);
		GUI.DrawTexture(new Rect(healthBarloc.x + healthIconOffset.x,healthBarloc.y + healthIconOffset.y,60,60),healthIcon, ScaleMode.ScaleToFit, true, 0f);
		
		//Mood Bar	
		GUI.DrawTexture(new Rect(moodBarloc.x,moodBarloc.y,100,100), statBarTexture);
		GUI.DrawTexture(new Rect(moodBarloc.x + moodBarOffset.x,moodBarloc.y+moodBarOffset.y,25,70),statBarVerFrame);
		GUI.DrawTexture(new Rect(moodBarloc.x + moodBarOffset.x,moodBarloc.y+moodBarOffset.y+(70-70*mood/100),25, 70 * Mathf.Clamp01(mood/100)),statBarVerFill, ScaleMode.ScaleAndCrop, true, 25/70);
		GUI.DrawTexture(new Rect(moodBarloc.x + moodIconOffset.x,moodBarloc.y+moodIconOffset.y,60,60),moodIcon,ScaleMode.ScaleToFit, true, 0f);
		
		//Food Bar
		GUI.DrawTexture(new Rect(foodBarloc.x,foodBarloc.y,100,100), statBarTexture);
		GUI.DrawTexture(new Rect(foodBarloc.x + foodbarOffset.x,foodBarloc.y + foodbarOffset.y,25,70),statBarVerFrame);
		GUI.DrawTexture(new Rect(foodBarloc.x + foodbarOffset.x,foodBarloc.y + foodbarOffset.y+(70-70*food/100),25, 70 * Mathf.Clamp01(food/100)),statBarVerFill, ScaleMode.ScaleAndCrop, true, 25/70);
		GUI.DrawTexture(new Rect(foodBarloc.x + foodIconOffset.x,foodBarloc.y + foodIconOffset.y,60,60),foodIcon,ScaleMode.ScaleToFit, true, 0f);
		
		//Extending Button Groups
		menuTextureRect = new Rect(menuRect.rect.x - 600, menuRect.rect.y - 10, menuRect.rect.width, menuRect.rect.height);
		GUI.DrawTexture(menuTextureRect, itemBarTexture);
		GUILayout.BeginArea(menuRect.rect, "");
		GUILayout.BeginHorizontal("");
		if(GUILayout.Button(sandwichTexture, GUILayout.Height(menuBoxHeight), GUILayout.Width(menuBoxWidth))){
			
			DataManager.AddStars(99);	
		}
		if(GUILayout.Button(appleTexture, GUILayout.Height(menuBoxHeight), GUILayout.Width(menuBoxWidth))){
			
			DataManager.SubtractStars(35);
		}
		GUILayout.Button(inhalerTexture, GUILayout.Height(menuBoxHeight), GUILayout.Width(menuBoxWidth));
		GUILayout.Button(emInhalerTexture, GUILayout.Height(menuBoxHeight), GUILayout.Width(menuBoxWidth));
		
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
	}
}
