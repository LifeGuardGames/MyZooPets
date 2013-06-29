using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ChallengesGUI : MonoBehaviour {

    public GameObject cameraMoveObject;
    public GameObject roomGuiObject;
    public GUISkin defaultSkin;

    //Textures
    public Texture2D bgPanel;
	public Texture2D backButton;

    //Styles
    public GUIStyle challengesTextStyle;
	public GUIStyle blankButtonStyle;
	
    //Challenge positions
    private Vector2 challengesInitPosition = new Vector2(125,-800);
    private Vector2 challengesFinalPosition = new Vector2(650,100);
    private LTRect challengesRect;

    // native dimensions
    private const float NATIVE_WIDTH = 1280.0f;
    private const float NATIVE_HEIGHT = 800.0f;

    //challenge window dimensions
    private const float WINDOW_WIDTH = 600;
    private const float WINDOW_HEIGHT = 650;

    private const float DAILY_CHALLENGE_TITLE_WIDTH = 300;
    private const float DAILY_CHALLENGE_TITLE_HEIGHT = 100;

    //MISC
    private CameraMove cameraMove;
    private RoomGUI roomGui;
    private bool challengesActive = false;
    private bool showGUI = true;

    //Reading challenges entries
    public void Init(){
    }

    // Use this for initialization
    void Start(){
        cameraMove = cameraMoveObject.GetComponent<CameraMove>();
        roomGui = roomGuiObject.GetComponent<RoomGUI>();
        challengesRect = new LTRect(challengesInitPosition.x,challengesInitPosition.y, 
            WINDOW_WIDTH, WINDOW_HEIGHT);
    }

    // Called from ClickManager
    public void ChallengesClicked(){
        if(!challengesActive){
            challengesActive = true;
            cameraMove.PetSideZoomToggle();
            roomGui.HideGUIs(false, true, true, true);
            showGUI = false;

            if(!showGUI){
                ShowChallenges(false);
            }
        }
    }

    // Used in check challenges from diagnose game
    public void ShowChallenges(bool enteredFromDiagnoseGame){
        Hashtable optional = new Hashtable();
        optional.Add("ease", LeanTweenType.easeInOutQuad);
        LeanTween.move(challengesRect, challengesFinalPosition, 0.5f, optional);
    }

    public void HideChallenges(){
        Hashtable optional = new Hashtable();
        optional.Add("ease", LeanTweenType.easeInOutQuad);
        LeanTween.move(challengesRect, challengesInitPosition, 0.5f, optional);
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
        //////                                         Challenge Pages
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////
        GUI.depth = 0;
		
		
		
        GUI.BeginGroup(challengesRect.rect);
            // GUI.DrawTexture(new Rect(0, 0, WINDOW_WIDTH, WINDOW_HEIGHT), bgPanel);
            GUI.Label(new Rect(WINDOW_WIDTH/2 - DAILY_CHALLENGE_TITLE_WIDTH/2, 0, 
                DAILY_CHALLENGE_TITLE_WIDTH, DAILY_CHALLENGE_TITLE_HEIGHT), 
                "Daily Challenge", challengesTextStyle); //Title
            
            GUI.BeginGroup(new Rect(50, 100, 500, 200)); //Todays Challenge group
                GUI.Box(new Rect(0, 0, 500, 300), "");
                GUI.Label(new Rect(500/2 - 250/2, 25, 250, 100), "Description"); //Description
            GUI.EndGroup();

            GUI.BeginGroup(new Rect(50, WINDOW_HEIGHT/2, 500, 300)); //reward group
                GUI.Box(new Rect(0, 0, 500, 300), "");
                GUI.BeginGroup(new Rect(25, 0, 450, 100)); //Day 1 to 4
                GUI.Box(new Rect(0, 0, 450, 100), "");
                    GUI.Button(new Rect(10, 10, 80, 80), "Day 1");
                GUI.EndGroup();
                GUI.Box(new Rect(25, 120, 450, 180), "Day 5"); //Day 5
            GUI.EndGroup();
        GUI.EndGroup();
        // GUI.DrawTexture(challengesRect.rect,challengesTexture);

        //Temp close Button
        if(GUI.Button(new Rect(challengesRect.rect.x - 20, challengesRect.rect.y - 20, backButton.width, backButton.height), backButton, blankButtonStyle)){
            HideChallenges();
            showGUI = true;
            ClickManager.ClickLock();
            roomGui.ShowGUIs();
            cameraMove.PetSideZoomToggle();
            challengesActive = false;
        }
    }
}
