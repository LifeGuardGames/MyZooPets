using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DiaryGUI : MonoBehaviour {

	public GameObject cameraMoveObject;
	public GUISkin defaultSkin;

	//Textures
	public Texture2D diaryTexture3;
	public Texture2D tickBoxEmpty;
	public Texture2D tickBoxChecked;
	public Texture2D tickBoxMissed;
	public Texture2D backButton;

	//Styles
	public GUIStyle diaryTabStyle;
	public GUIStyle diaryCheckBoxStyle;
	public GUIStyle diaryTextStyle;
	public GUIStyle blankButtonStyle;

	//Diary positions
	private Vector2 diaryInitPosition = new Vector2(125,-800);
	private Vector2 diaryFinalPosition = new Vector2(650,100);
	private LTRect diaryRect;

	// native dimensions
    private const float NATIVE_WIDTH = 1280.0f;
    private const float NATIVE_HEIGHT = 800.0f;

	//MISC
	private CameraMove cameraMove;
	private bool diaryActive = false;
	private bool isEnteredFromDiagnose = false;
	private bool showGUI = true;
	private int diaryPage = 3;
	// private List<CalendarEntry> calendar;

	//Reading calendar entries
	public void Init(){
		// calendar = CalendarLogic.GetCalendarEntries();
	}

	// Use this for initialization
	void Start(){
		cameraMove = cameraMoveObject.GetComponent<CameraMove>();
		diaryRect = new LTRect(diaryInitPosition.x,diaryInitPosition.y, 600, 650);
	}

	// Called from ClickManager
	public void DiaryClicked(){
		if(!diaryActive){
			ClickManager.ModeLock();
			ClickManager.ClickLock();

			diaryActive = true;
			// cameraMove.PetSideZoomToggle();
			cameraMove.ZoomToggle(ZoomItem.Pet);
			showGUI = false;

			// CalendarLogic.CalendarOpened();
			if(!showGUI){
				// ShowDiary(1, false);
				ShowDiary(3, false);
			}
		}
	}

	// Used in check diary from diagnose game
	public void ShowDiary(int pageNumber, bool enteredFromDiagnoseGame){
		isEnteredFromDiagnose = enteredFromDiagnoseGame;
		diaryPage = pageNumber;
		Hashtable optional = new Hashtable();
		optional.Add("ease", LeanTweenType.easeInOutQuad);
		LeanTween.move(diaryRect, diaryFinalPosition, 0.5f, optional);
	}

	public void HideDiary(){
		Hashtable optional = new Hashtable();
		optional.Add("ease", LeanTweenType.easeInOutQuad);
		LeanTween.move(diaryRect, diaryInitPosition, 0.5f, optional);
	}

	void OnGUI(){
		//don't draw until all data is loaded
		if(!LoadDataLogic.IsDataLoaded) return;

		GUI.skin = defaultSkin;

		// Proportional scaling
		if (NATIVE_WIDTH != Screen.width || NATIVE_HEIGHT != Screen.height){
            float horizRatio = Screen.width/NATIVE_WIDTH;
            float vertRatio = Screen.height/NATIVE_HEIGHT;
            GUI.matrix = Matrix4x4.TRS(new Vector3(0, 0, 0), Quaternion.identity, new Vector3(horizRatio, vertRatio, 1));
		}

		///////////////////////////////////////////////////////////////////////////////////////////////////////////////
		//////                                         Diary Pages                                          ///////////
		///////////////////////////////////////////////////////////////////////////////////////////////////////////////
		Hashtable optional = new Hashtable();
		optional.Add("ease", LeanTweenType.easeInOutQuad);
		GUI.depth = 0;

		GUI.DrawTexture(diaryRect.rect,diaryTexture3);

		if(GUI.Button(new Rect(diaryRect.rect.x+555,diaryRect.rect.y+480,40,110),"",diaryTabStyle)){	// TODO-s Change rect to fit pixel from sprite
			diaryPage = 4;
		}

		// Close Button
		if(GUI.Button(new Rect(diaryRect.rect.x - 20, diaryRect.rect.y - 20, backButton.width, backButton.height), backButton, blankButtonStyle)){
			HideDiary();
			if(!isEnteredFromDiagnose){
				showGUI = true;
				ClickManager.ClickLock();
				// cameraMove.PetSideZoomToggle();
				cameraMove.ZoomToggle(ZoomItem.Pet);
				diaryActive = false;
			}
			isEnteredFromDiagnose = false;
		}
	}
}
