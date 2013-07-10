using UnityEngine;
using System.Collections;

public class PopupNotification : MonoBehaviour {

	public GUISkin skin;
	public Texture2D notificationPanel;
    private string message;

    // default values, change them using the accessors if necessary
    private string button1 = "Yes";
    private string button2 = "Ignore";

    public string Button1String{
        get {return button1;}
        set {button1 = value;}
    }
    public string Button2String{
        get {return button2;}
        set {button2 = value;}
    }

	// Lean Tween
    private LTRect panelRect;
    private Vector2 initPosition;
    private Vector2 finalPosition;

    private Rect infoRect;
    private Rect centerButtonRect;
    private Rect leftButtonRect;
    private Rect rightButtonRect;

	// Styles
	public GUIStyle styleLabel;

    // Native dimensions
    private const float NATIVE_WIDTH = 1280.0f;
    private const float NATIVE_HEIGHT = 800.0f;

    // Delegate method for buttons
    // Will be called when buttons are clicked
    public delegate void OnButtonClicked();
    public OnButtonClicked yesButtonClicked;
    public OnButtonClicked noButtonClicked;
    private NotificationType notificationType;

	// Use this for initialization
	void Start () {

	}

    void OnGUI(){
		GUI.skin = skin;
        // Proportional scaling
        if (NATIVE_WIDTH != Screen.width || NATIVE_HEIGHT != Screen.height){
            float horizRatio = Screen.width/NATIVE_WIDTH;
            float vertRatio = Screen.height/NATIVE_HEIGHT;
            GUI.matrix = Matrix4x4.TRS(new Vector3(0, 0, 0), Quaternion.identity, new Vector3(horizRatio, vertRatio, 1));
        }

		GUI.BeginGroup(panelRect.rect, notificationPanel);
		GUI.Label(infoRect, message, styleLabel);

        if (notificationType == NotificationType.YesNo){
            if(GUI.Button(leftButtonRect, button1)){
                if(yesButtonClicked != null) yesButtonClicked();
                Hide();
            }

            if(GUI.Button(rightButtonRect, button2)){
                if(noButtonClicked != null) noButtonClicked();
                Hide();
            }
        }else if (notificationType == NotificationType.OK){
            if(GUI.Button(centerButtonRect, button1)){
                if(yesButtonClicked != null) yesButtonClicked();
                Hide();
            }
        }
        GUI.EndGroup();
    }

    //Initialize for 2 button popup
    public void Init(string message, OnButtonClicked yesButton, OnButtonClicked noButton){
        notificationType = NotificationType.YesNo;
        this.message = message;
        yesButtonClicked = yesButton;
        noButtonClicked = noButton;

        InitializeGUIPosition();
        leftButtonRect = new Rect(90, 350, 200, 100);
        rightButtonRect = new Rect(450, 350, 200, 100);

        Display();
    }

    //Initialize for 1 button popup
    public void Init(string message, OnButtonClicked yesButton){
        notificationType = NotificationType.OK;
        this.message = message;
        yesButtonClicked = yesButton;

        InitializeGUIPosition();
        centerButtonRect = new Rect(270, 350, 200, 100);

        Display();
    }

    private void InitializeGUIPosition(){
        // Initialize positions for LTRect
        initPosition = new Vector2(NATIVE_WIDTH / 2 - notificationPanel.width / 2,
            notificationPanel.height * -1);
        finalPosition = new Vector2(NATIVE_WIDTH / 2 - notificationPanel.width / 2,
            NATIVE_HEIGHT / 2 - notificationPanel.height / 2);
        panelRect = new LTRect(initPosition.x, initPosition.y, notificationPanel.width,
            notificationPanel.height);

        infoRect = new Rect(90, 50, 560, 260);
    }

    // Display the popup panel
    private void Display(){
        Hashtable optional = new Hashtable();
        optional.Add("ease", LeanTweenType.easeOutBounce);
        LeanTween.move(panelRect, finalPosition, 1.0f, optional);
    }

    // Hide the popup panel
    private void Hide(){
        Hashtable optional = new Hashtable();
        optional.Add("onCompleteTarget", gameObject);
        optional.Add("onComplete", "DestroyNotification");
        optional.Add("ease", LeanTweenType.easeInOutQuad);
        LeanTween.move(panelRect, initPosition, 0.5f, optional);
    }

    // Destroy PopupNotification prefab after it's done
    private void DestroyNotification(){
        Destroy(gameObject);
    }

}
