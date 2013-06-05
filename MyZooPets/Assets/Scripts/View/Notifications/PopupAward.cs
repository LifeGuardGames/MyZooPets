using UnityEngine;
using System.Collections;

public class PopupAward : MonoBehaviour {
	
	public Texture2D healthIcon;
	public Texture2D moodIcon;
	public Texture2D hungerIcon;
	public Texture2D starIcon;
	
	private LTRect awardRect;
	//private Hashtable<string, string> awardHash;
	
	void Start(){
		awardRect = new LTRect(Screen.width/2 - 150, -100, 300, 400);
		//awardHash = new Hashtable<string, string>();
		
		Hashtable optional = new Hashtable();
		optional.Add("ease", LeanTweenType.easeOutBounce);
		LeanTween.move(awardRect, new Vector2(Screen.width/2 - awardRect.rect.width/2, Screen.height/2 + 150), 1.0f, optional);
	}
	
	//TODO-s correct way to contruct initilization?
	public void Populate(){
		
	}
	
	void Update(){
		
	}
	
	void OnGUI(){
		GUILayout.BeginArea(awardRect.rect);
		GUILayout.BeginVertical();
		
		GUILayout.BeginHorizontal();
		GUILayout.Box(healthIcon, GUILayout.Height(60), GUILayout.Width(60));
		GUI.color = Color.black;
		GUILayout.Label("+ 100");
		GUI.color = Color.white;
		GUILayout.EndHorizontal();
		
		GUILayout.BeginHorizontal();
		GUILayout.Box(hungerIcon, GUILayout.Height(60), GUILayout.Width(60));
		GUI.color = Color.black;
		GUILayout.Label("+ 50");
		GUI.color = Color.white;
		GUILayout.EndHorizontal();
		
		GUILayout.EndVertical();
		GUILayout.EndArea();
	}
}
