using UnityEngine;
using System.Collections;

public class PopupNotification : MonoBehaviour {

    private string message;
    
    //Lean Tween
    private LTRect ltRect;
    private Vector2 initPosition;
    private Vector2 finalPosition;

    // native dimensions
    private const float NATIVE_WIDTH = 1280.0f;
    private const float NATIVE_HEIGHT = 800.0f;

    private const float POPUP_SIZE = 300;

    //delegate method for buttons
    //will be called when buttons are clicked
    public delegate void OnButtonClicked();
    public OnButtonClicked yesButtonClicked;
    public OnButtonClicked noButtonClicked;

	// Use this for initialization
	void Start () {
        
	}
	
    void OnGUI(){
        // Proportional scaling
        if (NATIVE_WIDTH != Screen.width || NATIVE_HEIGHT != Screen.height){
            float horizRatio = Screen.width/NATIVE_WIDTH;
            float vertRatio = Screen.height/NATIVE_HEIGHT;
            GUI.matrix = Matrix4x4.TRS(new Vector3(0, 0, 0), Quaternion.identity, new Vector3(horizRatio, vertRatio, 1));
        }

        GUILayout.BeginArea(ltRect.rect);
        GUILayout.BeginVertical();
        GUILayout.Label(message);
        
        GUILayout.BeginHorizontal();
        if(GUILayout.Button("Yes")){
            if(yesButtonClicked != null) yesButtonClicked();
            Hide();
        }

        if(GUILayout.Button("Ignore")){
            if(noButtonClicked != null) noButtonClicked();
            Hide();
        }
        GUILayout.EndHorizontal();
        GUILayout.EndVertical();
        GUILayout.EndArea();
    }

    public void Init(string message, OnButtonClicked yesButton, OnButtonClicked noButton){
        this.message = message;
        yesButtonClicked = yesButton;
        noButtonClicked = noButton;

        //initialize positions for LTRect
        initPosition = new Vector2(NATIVE_WIDTH/2-POPUP_SIZE/2, -100);
        finalPosition = new Vector2(NATIVE_WIDTH/2-POPUP_SIZE/2, NATIVE_HEIGHT/2);
        ltRect = new LTRect(initPosition.x, initPosition.y, POPUP_SIZE, POPUP_SIZE);
        Display();
    }

    //display the popup panel
    private void Display(){
        Hashtable optional = new Hashtable();
        optional.Add("ease", LeanTweenType.easeInQuad);
        LeanTween.move(ltRect, finalPosition, 0.5f, optional);
    }

    //hide the popup panel
    private void Hide(){
        Hashtable optional = new Hashtable();
        optional.Add("onCompleteTarget", gameObject);
        optional.Add("onComplete", "DestroyNotificaiton");
        optional.Add("ease", LeanTweenType.easeOutCirc);
        LeanTween.move(ltRect, initPosition, 0.5f, optional);
    }

    //destroy PopupNotification prefab after it's done
    private void DestroyNotificaiton(){
        Destroy(gameObject);
    }

}
