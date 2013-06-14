using UnityEngine;
using System.Collections;

/// <summary>
/// First time GUI.
/// Stuffed everything that needs to be done in the first run here.
/// If its not the first time the game is run, this will delete itself.
/// </summary>

public class FirstTimeGUI : MonoBehaviour {
	
	// Native dimensions
    private const float NATIVE_WIDTH = 1280.0f;
    private const float NATIVE_HEIGHT = 800.0f;	
	
	public GUISkin defaultSkin;
	public Texture2D logo;
	public bool splashScreenAux = true;
	
	public GameObject eggObject;
	private Vector3 eggSpritePosition = new Vector3(0f, -1.88f, -10f);
	private tk2dSprite eggSpriteScript;
	public GameObject nestObject;
	public GameObject petObject;
	public string petName;
	public string petColor;
	
	// Edit egg panels
	public Texture2D editPanel;
	private bool isEditEgg = false;
	private LTRect editEggRect;
	private Vector2 editEggRectInitPos = new Vector2(1300f, 100f);
	private Vector2 editEggRectFinalPos = new Vector2(600f, 100f);
	
	// Camera moving
	public GameObject mCamera;
	private float smooth = 1.0f;
	private bool isZoomed = false;
	private Vector3 initPosition = new Vector3(0f, 5.7f, -23f);
	private Vector3 initFaceDirection = new Vector3(11.3f, 0, 0);
	private Vector3 finalPosition = new Vector3(3.44f, 2.55f, -17.3f);
	private Vector3 finalFaceDirection = new Vector3(11.3f, 0, 0);
	
	private Color currentRenderColor;
	private LTRect logoRect;
	
	//TODO-s Make this an array or something?
	public Texture2D blueButton;
	public Texture2D greenButton;
	public Texture2D yellowButton;
	public Texture2D redButton;
	public Texture2D purpleButton;
	
	void Start(){
		if(DataManager.FirstTime){
			eggSpriteScript = eggObject.GetComponent<tk2dSprite>();
			currentRenderColor = RenderSettings.ambientLight;
			RenderSettings.ambientLight = Color.black;
			
			logoRect = new LTRect(Screen.width/2 - logo.width/2, -200f, 839f, 231f);
			
			editEggRect = new LTRect(editEggRectInitPos.x, editEggRectInitPos.y, 610, 611);
		}
		else{
			// TEMPORARY spawn the pet in location
			GameObject goPet = Instantiate(petObject, new Vector3(0f, -2.87f, -10f), Quaternion.identity) as GameObject;
			goPet.name = "SpritePet";
			
			// Kill itself + related objects if not first time
			Destroy(eggObject);
			Destroy(nestObject);
			Destroy(gameObject);
		}
	}
	
	void Update(){
		// Splash finished, Drop down the title and the egg sprite, only called once
		if(splashScreenAux && SplashScreen.IsFinished){
			Hashtable optional = new Hashtable();
			optional.Add("ease", LeanTweenType.easeInQuad);
			LeanTween.move(logoRect, new Vector2(Screen.width/2 - logo.width/2, 100f), 0.5f, optional);
			
			Hashtable optional2 = new Hashtable();
			optional2.Add("ease", LeanTweenType.easeOutBounce);
			LeanTween.move(eggObject, eggSpritePosition, 1.5f, optional2);
			splashScreenAux = false;
		}
		
		//TODO-s Optimize this for touch? / ABSTRACT TO CAMERAMOVE?? perhaps not for coherency
		if(Input.GetMouseButtonUp(0)){
			Ray myRay = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;
			if(Physics.Raycast(myRay,out hit))
			{
				if(hit.collider.name == "SpriteEgg")
				{
					CameraTransform(finalPosition,finalFaceDirection);
		    	    isZoomed = true;
					HideTitle();
					ShowChooseGUI();
				}
			}
		}
	}
	
	void ShowChooseGUI(){
		isEditEgg = true;
		Hashtable optional = new Hashtable();
		optional.Add("ease", LeanTweenType.easeInOutQuad);
		LeanTween.move(editEggRect, editEggRectFinalPos, 1.0f, optional);
	}
	
	void HideChooseGUI(){
		Hashtable optional = new Hashtable();
		optional.Add("onCompleteTarget", gameObject);
		optional.Add("onComplete", "HelperFinishEditPet");
		optional.Add("ease", LeanTweenType.easeInOutQuad);
		LeanTween.move(editEggRect, editEggRectInitPos, 1.0f, optional);
		RenderSettings.ambientLight = currentRenderColor;	// lerp this
	}
	
	// Callback for closing edit panel
	private void HelperFinishEditPet(){
		DataManager.PetName = petName;
		DataManager.PetColor = petColor;
		isEditEgg = false;
		
		// Spawn pet object
		GameObject goPet = Instantiate(petObject, new Vector3(0f, -2.87f, -10f), Quaternion.identity) as GameObject;
		goPet.name = "SpritePet";
		
		// Start normal GUI stuff
		finishHatchCallBack(false);
		
		// Commit seppuku
		Destroy(eggObject);
		Destroy(nestObject);
		Destroy(gameObject);
	}
	
	void OnGUI(){
		if(!SplashScreen.IsFinished) return;
		
		// Proportional scaling
		if(NATIVE_WIDTH != Screen.width || NATIVE_HEIGHT != Screen.height){     
            float horizRatio = Screen.width/NATIVE_WIDTH;
            float vertRatio = Screen.height/NATIVE_HEIGHT;
            GUI.matrix = Matrix4x4.TRS(new Vector3(0, 0, 0), Quaternion.identity, new Vector3(horizRatio, vertRatio, 1));
		}
		
		if(logo != null){
			GUI.DrawTexture(logoRect.rect, logo);
		}
		
		if(isEditEgg){
			GUI.skin = defaultSkin;
			GUI.DrawTexture(editEggRect.rect, editPanel); 
			GUILayout.BeginArea(new Rect(editEggRect.rect.x + 20, editEggRect.rect.y + 20, editEggRect.rect.width - 40, editEggRect.rect.height - 40));
			GUILayout.BeginVertical();
			
			GUILayout.Label("Name");
			petName = GUILayout.TextField(petName, 25);
			
			GUILayout.Label("Color");
			if(GUILayout.Button(blueButton, GUILayout.Width(120), GUILayout.Height(61))){
				eggSpriteScript.SetSprite("eggBlueChoose");
				petColor = "whiteBlue";
			}
			else if(GUILayout.Button(greenButton, GUILayout.Width(120), GUILayout.Height(61))){
				eggSpriteScript.SetSprite("eggGreenChoose");
				petColor = "whiteGreen";
			}
			else if(GUILayout.Button(yellowButton, GUILayout.Width(120), GUILayout.Height(61))){
				eggSpriteScript.SetSprite("eggYellowChoose");
				petColor = "whiteYellow";
			}
			else if(GUILayout.Button(redButton, GUILayout.Width(120), GUILayout.Height(61))){
				eggSpriteScript.SetSprite("eggRedChoose");
				petColor = "whiteRed";
			}
			else if(GUILayout.Button(purpleButton, GUILayout.Width(120), GUILayout.Height(61))){
				eggSpriteScript.SetSprite("eggPurpleChoose");
				petColor = "whiteRed";
			}
			else if(GUILayout.Button("Finish", GUILayout.Width(90), GUILayout.Height(90))){
				if(isZoomed){
					ZoomOutMove();
					isZoomed = false;
					HideChooseGUI();
				}
			}
			GUILayout.EndVertical();
			GUILayout.EndArea();
		}
	}
	
	void HideTitle(){
		if(logo != null){
			Hashtable optional = new Hashtable();
			optional.Add("onCompleteTarget", gameObject);
			optional.Add("onComplete", "HelperDeleteLogo");
			LeanTween.move(logoRect, new Vector2(Screen.width/2 - logo.width/2, -300f), 0.5f, optional);
		}
	}
	
	// Callback for hide title
	private void HelperDeleteLogo(){
		logo = null;
	}
	
	void CameraTransform (Vector3 newPosition, Vector3 newDirection){
		Hashtable optional = new Hashtable();
		optional.Add("ease", LeanTweenType.easeInOutQuad);
		LeanTween.move(mCamera, newPosition, smooth, optional);
		LeanTween.rotate(mCamera, newDirection, smooth, optional);
	}
	
	void ZoomOutMove(){
		CameraTransform(initPosition,initFaceDirection);
	}
	
	public delegate void FinishHatchCallBack(bool boolean);
	public static FinishHatchCallBack finishHatchCallBack;
}
