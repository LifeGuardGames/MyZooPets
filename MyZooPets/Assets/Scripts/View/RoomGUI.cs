using UnityEngine;
using System.Collections;

public class RoomGui : MonoBehaviour {
	
	// native dimensions
    private const float NATIVE_WIDTH = 1280.0f;
    private const float NATIVE_HEIGHT = 800.0f;
	
	private bool isMenuExpanded;
	private LTRect menuRect;
	public Texture2D starTexture;
	public Texture2D tierBarTexture;
	public Texture2D starBarTexture;
	public Texture2D statBarTexture;
	public Texture2D roomTexture;
	public Texture2D foodIcon;
	public Texture2D healthIcon;
	public Texture2D moodIcon;
	public Texture2D demopet;
	public GUIStyle starTextStyle;
	
	void Start (){
		isMenuExpanded = true;
		menuRect = new LTRect(0, NATIVE_HEIGHT - 100, 475, 100);
	}
	
	void Update (){
	
	}
	
	void OnGUI(){
		if (NATIVE_WIDTH != Screen.width || NATIVE_HEIGHT != Screen.height){
            float horizRatio = Screen.width/NATIVE_WIDTH;
            float vertRatio = Screen.height/NATIVE_HEIGHT;
            GUI.matrix = Matrix4x4.TRS(new Vector3(0, 0, 0), Quaternion.identity, new Vector3(horizRatio, vertRatio, 1));
		}
	
		GUI.DrawTexture(new Rect(0,0,1280,800), roomTexture);
		GUI.DrawTexture(new Rect(0,2,530,75), tierBarTexture);
		GUI.DrawTexture(new Rect(540,2,215,75), starBarTexture);
		GUI.DrawTexture(new Rect(550,6,60,60), starTexture, ScaleMode.ScaleToFit);
		GUI.DrawTexture(new Rect(0,80,100,100), statBarTexture);
		GUI.DrawTexture(new Rect(0,180,100,100), statBarTexture);
		GUI.DrawTexture(new Rect(0,280,100,100), statBarTexture);
		GUI.DrawTexture(new Rect(5,100,54,53),healthIcon);
		GUI.DrawTexture(new Rect(5,200,54,53),moodIcon);
		GUI.DrawTexture(new Rect(3,300,61,53),foodIcon);
		
		
		
		GUI.DrawTexture(new Rect(330,300,500,500), demopet);
	   	
		GUI.TextField(new Rect(630,20,60,60),"X 500",starTextStyle);
		
		GUILayout.BeginArea(menuRect.rect, "test");
		GUILayout.BeginHorizontal("box");
		GUILayout.Button("food", GUILayout.Height(90), GUILayout.Width(90));
		GUILayout.Button("Inhaler", GUILayout.Height(90), GUILayout.Width(90));
		GUILayout.Button("explore", GUILayout.Height(90), GUILayout.Width(90));
		GUILayout.Button("EmInhaler", GUILayout.Height(90), GUILayout.Width(90));
		
		if(isMenuExpanded){
			if(GUILayout.Button("-", GUILayout.Height(90), GUILayout.Width(90))){
				isMenuExpanded = false;
				Hashtable optional = new Hashtable();
				optional.Add("ease", LeanTweenType.easeInOutQuad);
				LeanTween.move(menuRect, new Vector2(-376, NATIVE_HEIGHT - 100), 0.3f, optional);
			}
		}
		else{
			if(GUILayout.Button("+", GUILayout.Height(90), GUILayout.Width(90))){
				isMenuExpanded = true;
				Hashtable optional = new Hashtable();
				optional.Add("ease", LeanTweenType.easeInOutQuad);
				LeanTween.move(menuRect, new Vector2(0, NATIVE_HEIGHT - 100), 0.3f, optional);
			}
		}
		
		GUILayout.EndHorizontal();
		GUILayout.EndArea();
	}
}
