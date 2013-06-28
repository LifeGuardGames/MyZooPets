using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ChallengesGUI : MonoBehaviour {

    public GameObject cameraMoveObject;
    public GameObject roomGuiObject;
    public GUISkin defaultSkin;

    //Textures
    public Texture2D challengesTexture;

    //Styles
    public GUIStyle challengesTextStyle;

    //Diary positions
    private Vector2 challengesInitPosition = new Vector2(125,-800);
    private Vector2 challengesFinalPosition = new Vector2(650,100);
    private LTRect challengesRect;

    // native dimensions
    private const float NATIVE_WIDTH = 1280.0f;
    private const float NATIVE_HEIGHT = 800.0f;

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
        challengesRect = new LTRect(challengesInitPosition.x,challengesInitPosition.y, 600, 650);
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
        //////                                         Diary Pages                                          ///////////
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////
        GUI.depth = 0;
        GUI.DrawTexture(challengesRect.rect,challengesTexture);

        //Temp close Button
        //TODO make a prettier icon??
        if(GUI.Button(new Rect(challengesRect.rect.x,challengesRect.rect.y,50,50),"X")){
            HideChallenges();
            showGUI = true;
            ClickManager.ClickLock();
            roomGui.ShowGUIs();
            cameraMove.PetSideZoomToggle();
            challengesActive = false;
        }
    }
}
