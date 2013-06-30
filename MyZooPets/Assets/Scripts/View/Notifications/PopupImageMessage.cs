using UnityEngine;
using System.Collections;

public class PopupImageMessage : MonoBehaviour {

    public GUISkin skin;
    public Texture2D notificationPanel;
    private string message = "Level Up!";
    private string trophyMessage;

    // Lean Tween
    private LTRect panelRect;
    private Vector2 initPosition;
    private Vector2 finalPosition;

    // Styles
    public GUIStyle styleLabel;
    public GUIStyle styleMessage;
    public GUIStyle styleButton;

    // trophies
    public Texture2D bronze;
    public Texture2D silver;
    public Texture2D gold;

    private Texture2D currentTexture;

    // Native dimensions
    private const float NATIVE_WIDTH = 1280.0f;
    private const float NATIVE_HEIGHT = 800.0f;


    // Delegate method for buttons
    // Will be called when buttons are clicked
    public delegate void OnButtonClicked();
    public OnButtonClicked okButtonClicked;

    // Use this for initialization
    void Start () {

    }

    void OnGUI(){
        GUI.depth = -1;
        GUI.skin = skin;
        // Proportional scaling
        if (NATIVE_WIDTH != Screen.width || NATIVE_HEIGHT != Screen.height){
            float horizRatio = Screen.width/NATIVE_WIDTH;
            float vertRatio = Screen.height/NATIVE_HEIGHT;
            GUI.matrix = Matrix4x4.TRS(new Vector3(0, 0, 0), Quaternion.identity, new Vector3(horizRatio, vertRatio, 1));
        }

        GUI.DrawTexture(panelRect.rect, notificationPanel);
        int titleHeight = 150;
        int spacing = 50;
        GUI.Label(new Rect(panelRect.rect.x + spacing, panelRect.rect.y + spacing, panelRect.rect.width - spacing * 2, titleHeight), message, styleLabel);

        GUI.DrawTexture(new Rect(panelRect.rect.x + spacing, panelRect.rect.y + titleHeight, panelRect.rect.width / 2 - spacing * 2, panelRect.rect.height - titleHeight), currentTexture);
        GUI.Label(new Rect(panelRect.rect.x + panelRect.rect.width / 2, panelRect.rect.y + titleHeight, panelRect.rect.width / 2, panelRect.rect.height - titleHeight), trophyMessage, styleMessage);

        if(GUI.Button(new Rect(panelRect.rect.x + panelRect.rect.width/2 + 25, panelRect.rect.y + 350, 200, 100), "OK")){
            if(okButtonClicked != null) okButtonClicked();
            Hide();
        }
    }

    public void Init (TrophyTier trophy, OnButtonClicked okButton){
        trophyMessage = GetMessage(trophy);
        currentTexture = GetTexture(trophy);
        okButtonClicked = okButton;

        // Initialize positions for LTRect
        initPosition = new Vector2(NATIVE_WIDTH / 2 - notificationPanel.width / 2, notificationPanel.height * -1);
        finalPosition = new Vector2(NATIVE_WIDTH / 2 - notificationPanel.width / 2, NATIVE_HEIGHT / 2 - notificationPanel.height / 2);
        panelRect = new LTRect(initPosition.x, initPosition.y, notificationPanel.width, notificationPanel.height);
        Display();
    }

    private string GetMessage(TrophyTier trophy){
        string retVal = "";

        switch (trophy){

            case TrophyTier.Null:
            retVal = "Too bad, better luck next time.";
            break;

            case TrophyTier.Bronze:
            retVal = "Nice try, but you can do better.";
            break;

            case TrophyTier.Silver:
            retVal = "Not bad!";
            break;

            case TrophyTier.Gold:
            retVal = "Excellent work!";
            break;

        }

        return retVal;
    }

    private Texture2D GetTexture(TrophyTier trophy){
        Texture2D retVal = null;

        switch (trophy){

            case TrophyTier.Null:
            break;

            case TrophyTier.Bronze:
            retVal = bronze;
            break;

            case TrophyTier.Silver:
            retVal = silver;
            break;

            case TrophyTier.Gold:
            retVal = gold;
            break;

        }

        return retVal;
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

    // Destroy PopupImageMessage prefab after it's done
    private void DestroyNotificaiton(){
        Destroy(gameObject);
    }

}
