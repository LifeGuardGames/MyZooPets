using UnityEngine;
using System.Collections;

/// <summary>
/// First time GUI.
/// Stuffed everything that needs to be done in the first run here
/// </summary>

public class FirstTimeGUI : MonoBehaviour {
	
	public bool splashScreenAux = true;
	
	public GameObject eggSprite;
	private Vector3 eggSpritePosition = new Vector3(0f, -1.88f, -10f);

	private tk2dSprite eggSpriteScript;
	public GameObject nestSprite;
	public string petName;
	
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
	public Texture2D logo;
	private LTRect logoRect;
	
	//TODO-s Make this an array or something?
	public Texture2D blueButton;
	public Texture2D greenButton;
	public Texture2D yellowButton;
	public Texture2D redButton;
	public Texture2D purpleButton;
	
	void Start(){
		if(DataManager.FirstTime){
			Debug.Log("Hatch sequences");
			eggSpriteScript = eggSprite.GetComponent<tk2dSprite>();
			currentRenderColor = RenderSettings.ambientLight;
			RenderSettings.ambientLight = Color.black;
			
			logoRect = new LTRect(Screen.width/2 - logo.width/2, -200f, 839f, 231f);
			
			editEggRect = new LTRect(editEggRectInitPos.x, editEggRectInitPos.y, 600, 600);
		}
		else{
			Destroy(eggSprite);
			Destroy(nestSprite);
			Destroy(gameObject);
		}
	}
	
	void Update(){
		
		// Splash finished, Drop down the title and the egg sprite, only called once
		if(splashScreenAux && Fader.IsSplashScreenFinished){
			Hashtable optional = new Hashtable();
			optional.Add("ease", LeanTweenType.easeInQuad);
			LeanTween.move(logoRect, new Vector2(Screen.width/2 - logo.width/2, 100f), 0.5f, optional);
			
			Hashtable optional2 = new Hashtable();
			optional2.Add("ease", LeanTweenType.easeOutBounce);
			LeanTween.move(eggSprite, eggSpritePosition, 2.0f, optional2);
			splashScreenAux = false;
		}
		
		if (Input.GetKeyUp(KeyCode.Space)){
			if(isZoomed){
				ZoomOutMove();
				isZoomed = false;
				HideChooseGUI();
			}
			else{
		        CameraTransform(finalPosition,finalFaceDirection);
	    	    isZoomed = true;
				HideTitle();
				ShowChooseGUI();
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
	
	private void HelperFinishEditPet(){
		isEditEgg = false;
		finishHatchCallBack(false);
		Destroy(eggSprite);
		Destroy(nestSprite);
		Destroy(gameObject);
	}
	
	void OnGUI(){
		if(!Fader.IsSplashScreenFinished){
			return;
		}
		if(logo != null){
			GUI.DrawTexture(logoRect.rect, logo);
		}
		if(isEditEgg){
			GUI.Box(editEggRect.rect, ""); 
			GUILayout.BeginArea(editEggRect.rect);
			GUILayout.BeginVertical();
			
			GUILayout.Label("Name");
			petName = GUILayout.TextField(petName, 25);
			
			GUILayout.Label("Color");
			if(GUILayout.Button(blueButton, GUILayout.Width(120), GUILayout.Height(61))){
				eggSpriteScript.SetSprite("eggBlueChoose");
			}
			else if(GUILayout.Button(greenButton, GUILayout.Width(120), GUILayout.Height(61))){
				eggSpriteScript.SetSprite("eggGreenChoose");
			}
			else if(GUILayout.Button(yellowButton, GUILayout.Width(120), GUILayout.Height(61))){
				eggSpriteScript.SetSprite("eggYellowChoose");
			}
			else if(GUILayout.Button(redButton, GUILayout.Width(120), GUILayout.Height(61))){
				eggSpriteScript.SetSprite("eggRedChoose");
			}
			else if(GUILayout.Button(purpleButton, GUILayout.Width(120), GUILayout.Height(61))){
				eggSpriteScript.SetSprite("eggPurpleChoose");
			}
			else if(GUILayout.Button("Finish", GUILayout.Width(90), GUILayout.Height(90))){
				if(isZoomed)
				{
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
