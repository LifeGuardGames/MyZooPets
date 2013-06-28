using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CalendarGUI : MonoBehaviour {

    public GameObject cameraMoveObject;
    public GameObject roomGuiObject;
    public GUISkin defaultSkin;

    //Textures
    public Texture2D diaryTexture1;
    public Texture2D tickBoxEmpty;
    public Texture2D tickBoxChecked;
    public Texture2D tickBoxMissed;

    //Styles
    public GUIStyle diaryCheckBoxStyle;
    public GUIStyle diaryTextStyle;

    //Diary positions
    private Vector2 diaryInitPosition = new Vector2(125,-800);
    private Vector2 diaryFinalPosition = new Vector2(650,100);
    private LTRect diaryRect;

    // native dimensions
    private const float NATIVE_WIDTH = 1280.0f;
    private const float NATIVE_HEIGHT = 800.0f;

    //MISC
    private CameraMove cameraMove;
    private RoomGUI roomGui;
    private bool diaryActive = false;
    private bool showGUI = true;
    private int diaryPage = 1;
    private List<CalendarEntry> calendar;

    //Reading calendar entries
    public void Init(){
        calendar = CalendarLogic.GetCalendarEntries();
    }

    // Use this for initialization
    void Start(){
        cameraMove = cameraMoveObject.GetComponent<CameraMove>();
        roomGui = roomGuiObject.GetComponent<RoomGUI>();
        diaryRect = new LTRect(diaryInitPosition.x,diaryInitPosition.y, 600, 650);
    }


    // Called from ClickManager
    public void CalendarClicked(){
        if(!diaryActive){
            diaryActive = true;
            cameraMove.PetSideZoomToggle();
            roomGui.HideGUIs(false, true, true, true);
            showGUI = false;

            CalendarLogic.CalendarOpened();
            if(!showGUI){
                Hashtable optional = new Hashtable();
                optional.Add("ease", LeanTweenType.easeInOutQuad);
                LeanTween.move(diaryRect, diaryFinalPosition, 0.5f, optional);
            }
        }
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
        if(diaryPage == 1){
            GUI.DrawTexture(diaryRect.rect,diaryTexture1);
            GUI.TextArea(new Rect (diaryRect.rect.x+10,diaryRect.rect.y+100,100,70),"Monday",diaryTextStyle);
            GUI.TextArea(new Rect (diaryRect.rect.x+10,diaryRect.rect.y+170,100,70),"Tuesday",diaryTextStyle);
            GUI.TextArea(new Rect (diaryRect.rect.x+10,diaryRect.rect.y+240,100,70),"Wednesday",diaryTextStyle);
            GUI.TextArea(new Rect (diaryRect.rect.x+10,diaryRect.rect.y+310,100,70),"Thursday",diaryTextStyle);
            GUI.TextArea(new Rect (diaryRect.rect.x+10,diaryRect.rect.y+380,100,70),"Friday",diaryTextStyle);
            GUI.TextArea(new Rect (diaryRect.rect.x+10,diaryRect.rect.y+450,100,70),"Saturday",diaryTextStyle);
            GUI.TextArea(new Rect (diaryRect.rect.x+10,diaryRect.rect.y+520,100,70),"Sunday",diaryTextStyle);
            GUI.TextArea(new Rect (diaryRect.rect.x+100,diaryRect.rect.y+577,100,70),""+CalendarLogic.GetComboCount(),diaryTextStyle);

            //Layout for inhaler checks in a week
            GUILayout.BeginArea(new Rect(diaryRect.rect.x+115,diaryRect.rect.y+100,500,500), "");
            GUILayout.BeginVertical("");
            for(int i = 0;i < 7; i++){
                GUILayout.BeginHorizontal("");
                diaryCheckBoxStyle.normal.background = tickBoxEmpty;

                if(i < calendar.Count){
                    if(calendar[i].Morning == DosageRecord.Hit)
                        diaryCheckBoxStyle.normal.background = tickBoxChecked;
                    else if (calendar[i].Morning == DosageRecord.Miss)
                        diaryCheckBoxStyle.normal.background = tickBoxMissed;
                }

                GUILayout.Button("",diaryCheckBoxStyle,GUILayout.Height(70),GUILayout.Width(200));
                diaryCheckBoxStyle.normal.background = tickBoxEmpty;

                if(i < calendar.Count){
                    if(calendar[i].Afternoon == DosageRecord.Hit)
                        diaryCheckBoxStyle.normal.background = tickBoxChecked;
                    else if (calendar[i].Afternoon == DosageRecord.Miss)
                        diaryCheckBoxStyle.normal.background = tickBoxMissed;
                }

                GUILayout.Button("",diaryCheckBoxStyle,GUILayout.Height(70),GUILayout.Width(200));
                GUILayout.EndHorizontal();
            }
            GUILayout.EndVertical();
            GUILayout.EndArea();

        }

        //Temp close Button
        //TODO make a prettier icon??
        if(GUI.Button(new Rect(diaryRect.rect.x,diaryRect.rect.y,50,50),"X")){
            showGUI = !showGUI;
            ClickManager.ClickLock();
            LeanTween.move(diaryRect, diaryInitPosition, 0.5f, optional);
            roomGui.ShowGUIs();
            cameraMove.PetSideZoomToggle();
            diaryActive = false;
        }
    }
}