using UnityEngine;
using System.Collections;

public class LevelUpMessage : MonoBehaviour {

    public GUISkin skin;
    public Texture2D notificationPanel;
    
    // Styles
    public GUIStyle styleLabel;
    public GUIStyle styleMessage;
    public GUIStyle styleButton;

    // trophies
    public Texture2D bronze;
    public Texture2D silver;
    public Texture2D gold;

    // Delegate method for buttons
    // Will be called when buttons are clicked
    public delegate void OnButtonClicked();
    public OnButtonClicked okButtonClicked;

    private Texture2D currentTexture;

    // Lean Tween
    private LTRect panelRect;
    private Vector2 initPosition;
    private Vector2 finalPosition;

    private string message = "Level Up!";
    private string trophyMessage;
    

    // Native dimensions
    private const float NATIVE_WIDTH = 1280.0f;
    private const float NATIVE_HEIGHT = 800.0f;

    private const int TITLE_HEIGHT = 150;
    private const int SPACING = 50;

    private Vector2 trophyInitPos;
    private Vector2 trophyFinalPos;
    private Rect trophyRect;

    // Use this for initialization
    void Start () {
        
        
        // trophyTextureInitPos = new Vector2(0, 0);
        // trophyTextureFinalPos = new Vector2(0, 0);
        // trophyRect = new Rect(0, 0, 0, 0);

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

        GUI.BeginGroup(panelRect.rect, notificationPanel);
        GUI.Label(new Rect(SPACING, 0, 
            panelRect.rect.width - SPACING * 2, TITLE_HEIGHT), message, styleLabel);
        
        GUI.DrawTexture(trophyRect, currentTexture);
        GUI.Label(new Rect(notificationPanel.width-380, notificationPanel.height-350, 300, 300), trophyMessage, styleMessage);

        if(GUI.Button(new Rect(notificationPanel.width-240, notificationPanel.height-120 , 200, 100), "OK")){
            if(okButtonClicked != null) okButtonClicked();
            Hide();
        }
        GUI.EndGroup();
    }

    public void Init (TrophyTier trophy, OnButtonClicked okButton){
        trophyMessage = GetMessage(trophy);
        currentTexture = GetTexture(trophy);
        okButtonClicked = okButton;

       // Initialize positions for LTRect
        initPosition = new Vector2(NATIVE_WIDTH / 2 - notificationPanel.width / 2, 
            notificationPanel.height * -1);
        finalPosition = new Vector2(NATIVE_WIDTH / 2 - notificationPanel.width / 2, 
            NATIVE_HEIGHT / 2 - notificationPanel.height / 2);
        panelRect = new LTRect(initPosition.x, initPosition.y, notificationPanel.width, 
            notificationPanel.height); 

        // trophyInitPos = new Vector2(-400, notificationPanel.height-350);
        // trophyFinalPos = new Vector2(SPACING, notificationPanel.height-350);
        trophyRect = new Rect(SPACING, notificationPanel.height-350, 300, 300);

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
        Handheld.Vibrate();
        Hashtable optional = new Hashtable();
        optional.Add("onCompleteTarget", gameObject);
        optional.Add("onComplete", "PauseGame");
        optional.Add("ease", LeanTweenType.easeOutBounce);
        LeanTween.move(panelRect, finalPosition, 1.0f, optional);
    }

    // Hide the popup panel
    private void Hide(){
        PauseGame();
        Hashtable optional = new Hashtable();
        optional.Add("onCompleteTarget", gameObject);
        optional.Add("onComplete", "DestroyNotificaiton");
        optional.Add("ease", LeanTweenType.easeInOutQuad);
        LeanTween.move(panelRect, initPosition, 0.5f, optional);
    }

    //use to pause game when notification is showing
    private void PauseGame(){
        // if(Time.timeScale == 1.0f){
        //     Time.timeScale = 0f;
        // }else{
        //     Time.timeScale = 1.0f;
        // }
    }

    // Destroy PopupImageMessage prefab after it's done
    private void DestroyNotificaiton(){
        Destroy(gameObject);
    }

}
