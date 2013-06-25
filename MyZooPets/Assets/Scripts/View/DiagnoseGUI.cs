using UnityEngine;
using System.Collections;

public class DiagnoseGUI : MonoBehaviour {
	public Texture2D txPanel;
	public Texture2D txCheck;
	public Texture2D txHappy;
	public Texture2D txNeutral;
	public Texture2D txSad;
	public GUIStyle diagnoseStyle = new GUIStyle();
	public NotificationUIManager notificationUIManager;
	
	private LTRect diagnoseRect;
	private Vector2 diagnoseInitPosition;
	private Vector2 diagnoseFinalPosition;

	// native dimensions
    private const float NATIVE_WIDTH = 1280.0f;
    private const float NATIVE_HEIGHT = 800.0f;

    //Button dimensions
    private const float BUTTON_WIDTH = 175;
    private const float BUTTON_HEIGHT = 190;
	
	void Start(){
		diagnoseInitPosition = new Vector2(1300, 100);
		diagnoseFinalPosition = new Vector2(NATIVE_WIDTH/2, 100);
		diagnoseRect = new LTRect(diagnoseFinalPosition.x, diagnoseFinalPosition.y, 600, 600);
		notificationUIManager.PopupTexture("award", 100, 100, 100, 100, 100);
	}
	
	void Update(){
	
	}
	
	void OnGUI(){
		// Proportional scaling
		if (NATIVE_WIDTH != Screen.width || NATIVE_HEIGHT != Screen.height){
            float horizRatio = Screen.width/NATIVE_WIDTH;
            float vertRatio = Screen.height/NATIVE_HEIGHT;
            GUI.matrix = Matrix4x4.TRS(new Vector3(0, 0, 0), Quaternion.identity, 
            	new Vector3(horizRatio, vertRatio, 1));
		}

		// GUI.BeginGroup(diagnoseRect.rect, txPanel);
		
		// GUI.Label(new Rect(0,0, 600, 100), "How severe is the asthma?", diagnoseStyle);
		
		// if(GUI.Button(new Rect(10, 200, BUTTON_WIDTH, BUTTON_HEIGHT), txHappy)){
			
		// }
		// if(GUI.Button(new Rect(210, 200, BUTTON_WIDTH, BUTTON_HEIGHT), txNeutral)){
			
		// }
		// if(GUI.Button(new Rect(410, 200, BUTTON_WIDTH, BUTTON_HEIGHT), txSad)){
			
		// }
		// GUI.EndGroup();
	}
}
