using UnityEngine;
using System.Collections;

public class RoomGui : MonoBehaviour {
	
	// native dimensions
    private const float NATIVE_WIDTH = 1280.0f;
    private const float NATIVE_HEIGHT = 800.0f;
	
	private bool isMenuExpanded;
	private LTRect menuRect;
	
	
	void Start (){
		isMenuExpanded = true;
		menuRect = new LTRect(0, NATIVE_HEIGHT - 100, 475, 100);
	}
	
	void Update (){
	
	}
	
	void OnGUI(){
//		if (NATIVE_WIDTH != Screen.width || NATIVE_HEIGHT != Screen.height){
//            float horizRatio = Screen.width/NATIVE_WIDTH;
//            float vertRatio = Screen.height/NATIVE_HEIGHT;
//            GUI.matrix = Matrix4x4.TRS(new Vector3(0, 0, 0), Quaternion.identity, new Vector3(horizRatio, vertRatio, 1));
//		}
		
		GUI.Box(new Rect(0, 0, 530, 60), "Tier level");
		GUI.Box(new Rect(540, 0, 200, 60), "Stars x " + 1);
		GUI.Box(new Rect(0, 70, 90, 90), "Health");
		GUI.Box(new Rect(0, 170, 90, 90), "Hunger");
		GUI.Box(new Rect(0, 270, 90, 90), "Mood");
		
		GUILayout.BeginArea(menuRect.rect, "test");
		GUILayout.BeginHorizontal("box");
		GUILayout.Button("food", GUILayout.Height(90), GUILayout.Width(90));
		GUILayout.Button("Inhaler", GUILayout.Height(90), GUILayout.Width(90));
		GUILayout.Button("explore", GUILayout.Height(90), GUILayout.Width(90));
		GUILayout.Button("EmInhaler", GUILayout.Height(90), GUILayout.Width(90));
		
		if(isMenuExpanded){
			if(GUILayout.Button("+", GUILayout.Height(90), GUILayout.Width(90))){
				isMenuExpanded = false;
				Hashtable optional = new Hashtable();
				optional.Add("ease", LeanTweenType.easeInOutQuad);
				LeanTween.move(menuRect, new Vector2(-376, NATIVE_HEIGHT - 100), 0.3f, optional);
			}
		}
		else{
			if(GUILayout.Button("-", GUILayout.Height(90), GUILayout.Width(90))){
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
