using UnityEngine;
using System.Collections;

public class PopupNotification : MonoBehaviour {
	
	public GUISkin skin;
	public Texture2D notificationPanel;
    private string message;
	
	// Lean Tween
    private LTRect panelRect;
    private Vector2 initPosition;
    private Vector2 finalPosition;

	// Styles
	public GUIStyle styleLabel;
	public GUIStyle styleButton;
	
    // Native dimensions
    private const float NATIVE_WIDTH = 1280.0f;
    private const float NATIVE_HEIGHT = 800.0f;


    // Delegate method for buttons
    // Will be called when buttons are clicked
    public delegate void OnButtonClicked();
    public OnButtonClicked yesButtonClicked;
    public OnButtonClicked noButtonClicked;

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
		
		GUI.DrawTexture(panelRect.rect, notificationPanel);
		GUI.Label(new Rect(panelRect.rect.x + 50, panelRect.rect.y + 50, panelRect.rect.width - 100, 300), message, styleLabel);
		
		if(GUI.Button(new Rect(panelRect.rect.x + panelRect.rect.width/2 - 225, panelRect.rect.y + 350, 200, 100), "Yes")){
            if(yesButtonClicked != null) yesButtonClicked();
            Hide();
        }
		
		if(GUI.Button(new Rect(panelRect.rect.x + panelRect.rect.width/2 + 25, panelRect.rect.y + 350, 200, 100), "Ignore")){
            if(noButtonClicked != null) noButtonClicked();
            Hide();
        }
    }

    public void Init(string message, OnButtonClicked yesButton, OnButtonClicked noButton){
        this.message = message;
        yesButtonClicked = yesButton;
        noButtonClicked = noButton;
				
        // Initialize positions for LTRect
        initPosition = new Vector2(NATIVE_WIDTH / 2 - notificationPanel.width / 2, notificationPanel.height * -1);
        finalPosition = new Vector2(NATIVE_WIDTH / 2 - notificationPanel.width / 2, NATIVE_HEIGHT / 2 - notificationPanel.height / 2);
        panelRect = new LTRect(initPosition.x, initPosition.y, notificationPanel.width, notificationPanel.height);
        Display();
    }

    // Display the popup panel
    private void Display(){
        Hashtable optional = new Hashtable();
        optional.Add("ease", LeanTweenType.easeInOutQuad);
        LeanTween.move(panelRect, finalPosition, 0.5f, optional);
    }

    // Hide the popup panel
    private void Hide(){
        Hashtable optional = new Hashtable();
        optional.Add("onCompleteTarget", gameObject);
        optional.Add("onComplete", "DestroyNotificaiton");
        optional.Add("ease", LeanTweenType.easeInOutQuad);
        LeanTween.move(panelRect, initPosition, 0.5f, optional);
    }

    // Destroy PopupNotification prefab after it's done
    private void DestroyNotificaiton(){
        Destroy(gameObject);
    }

}
