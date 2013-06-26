using UnityEngine;
using System.Collections;

public class PopupNotification : MonoBehaviour {

    private string message;
    private LTRect ltRect;
    // native dimensions
    private const float NATIVE_WIDTH = 1280.0f;
    private const float NATIVE_HEIGHT = 800.0f;

    //delegate method for buttons
    public delegate void OnButtonClicked();
    public OnButtonClicked yesButtonClicked;
    public OnButtonClicked noButtonClicked;

	// Use this for initialization
	void Start () {
        ltRect = new LTRect(NATIVE_WIDTH/2 - 50, NATIVE_HEIGHT/2, 300, 300);
	
	}
	
	// Update is called once per frame
	void Update () {
	
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
        }

        if(GUILayout.Button("Ignore")){
            if(noButtonClicked != null) noButtonClicked();
            Destroy(gameObject);
        }
        GUILayout.EndHorizontal();
        GUILayout.EndVertical();
        GUILayout.EndArea();
    }

    

    public void Init(string message, OnButtonClicked yesButton, OnButtonClicked noButton){
        this.message = message;
        yesButtonClicked = yesButton;
        noButtonClicked = noButton;
    }
}
