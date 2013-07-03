using UnityEngine;
using System.Collections;

public class GameOverRewardMessage : MonoBehaviour {
    public GUISkin skin;
    public Texture2D notificationPanel;
    public Texture2D greatTexture;
    public Texture2D pointTexture;
    public Texture2D starTexture;

    // Delegate method for buttons
    // Will be called when buttons are clicked
    public delegate void OnButtonClicked();
    public OnButtonClicked yesButtonClicked;
    public OnButtonClicked noButtonClicked;

    //reward points
    public int dataPoints;
    public int dataStars;

    private int displayPoints = 0;
    private int displayStars = 0;

    // Lean Tween
    private LTRect panelRect;
    private Vector2 initPosition;
    private Vector2 finalPosition;

    // Native dimensions
    private const float NATIVE_WIDTH = 1280.0f;
    private const float NATIVE_HEIGHT = 800.0f;

    //GUI components
    private Rect titleRect;
    private Rect rewardRect;
    private Rect yesButtonRect;
    private Rect noButtonRect;
    private Rect centerButtonRect;

    private NotificationType notificationType;

    void Update(){
        AnimateScore();
    }

    //count score from 0 up
    private void AnimateScore(){
        if(displayPoints < dataPoints){
            if(displayPoints + 2 <= dataPoints){
                displayPoints += 2;
            }else{
                displayPoints += dataPoints - displayPoints;
            }
        }
        if(displayStars < dataStars){
            if(displayStars + 2 <= dataStars){
                displayStars += 2;    
            }else{
                displayStars += dataStars - displayStars;
            }
            
        }
    }

    void OnGUI(){
        GUI.depth = -1;
        GUI.skin = skin;

        // Proportional scaling
        if (NATIVE_WIDTH != Screen.width || NATIVE_HEIGHT != Screen.height){
            float horizRatio = Screen.width/NATIVE_WIDTH;
            float vertRatio = Screen.height/NATIVE_HEIGHT;
            GUI.matrix = Matrix4x4.TRS(new Vector3(0, 0, 0), Quaternion.identity, 
                new Vector3(horizRatio, vertRatio, 1));
        }

        //Panel
        GUI.BeginGroup(panelRect.rect, notificationPanel);

        //Title
        GUI.DrawTexture(titleRect, greatTexture); 

        //reward area
        GUILayout.BeginArea(rewardRect);
        GUILayout.BeginVertical();
        if(dataPoints != 0){
            GUILayout.BeginHorizontal();
                GUILayout.Label(pointTexture, GUILayout.Height(70), GUILayout.Width(90));
                GUILayout.Label("+" + displayPoints, GUILayout.Height(70), GUILayout.Width(165));
            GUILayout.EndHorizontal();
        }
        if(dataStars != 0){
            GUILayout.BeginHorizontal();
                GUILayout.Label(starTexture, GUILayout.Height(70), GUILayout.Width(90));
                GUILayout.Label("+" + displayStars, GUILayout.Height(70), GUILayout.Width(165));
            GUILayout.EndHorizontal();
        }
        GUILayout.EndVertical();
        GUILayout.EndArea();

        //buttons
        if(notificationType == NotificationType.YesNo){
            if(GUI.Button(yesButtonRect, "Play")){
                if(yesButtonClicked != null) yesButtonClicked();
                Hide();
            }
            if(GUI.Button(noButtonRect, "Done")){
                if(noButtonClicked != null) noButtonClicked();
                Hide();
            }
        }else if(notificationType == NotificationType.OK){
            if(GUI.Button(centerButtonRect, "Done")){
                if(yesButtonClicked != null) yesButtonClicked();
                Hide();
            }
        }
        
        GUI.EndGroup();
    }

    //Initialize 2 button reward popup
    public void Init(int deltaStars, int deltaPoints, OnButtonClicked yesButton,
            OnButtonClicked noButton){
        notificationType = NotificationType.YesNo;
        dataStars = deltaStars;
        dataPoints = deltaPoints;
       
       InitializeGUIPosition();

        yesButtonRect = new Rect(100, 360, 200, 100);
        noButtonRect = new Rect(450, 360, 200, 100);

        Display();
    }

    //Initialize 1 button reward popup
    public void Init(int deltaStars, int deltaPoints, OnButtonClicked yesButton){
        notificationType = NotificationType.OK;

        dataStars = deltaStars;
        dataPoints = deltaPoints;

        InitializeGUIPosition();

        centerButtonRect = new Rect(270, 360, 200, 100);
    }

    private void InitializeGUIPosition(){
         // Initialize positions for LTRect
        initPosition = new Vector2(NATIVE_WIDTH / 2 - notificationPanel.width / 2, 
            notificationPanel.height * -1);
        finalPosition = new Vector2(NATIVE_WIDTH / 2 - notificationPanel.width / 2, 
            NATIVE_HEIGHT / 2 - notificationPanel.height / 2);
        panelRect = new LTRect(initPosition.x, initPosition.y, notificationPanel.width, 
            notificationPanel.height); 

        //Initialize position for other GUI elements
        titleRect = new Rect(30, 0, 680, 150);
        rewardRect = new Rect(240, 180, 260, 150);
    }

    //Display popup panel
    private void Display(){
        Hashtable optional = new Hashtable();
        optional.Add("ease", LeanTweenType.easeOutBounce);
        LeanTween.move(panelRect, finalPosition, 1.0f, optional);
    }

    private void Hide(){
        Hashtable optional = new Hashtable();
        optional.Add("onCompleteTarget", gameObject);
        optional.Add("onComplete", "DestroyNotification");
        optional.Add("ease", LeanTweenType.easeInOutQuad);
        LeanTween.move(panelRect, initPosition, 0.5f, optional);
    }

    private void DestroyNotification(){
        Destroy(this.gameObject);
    }
}
