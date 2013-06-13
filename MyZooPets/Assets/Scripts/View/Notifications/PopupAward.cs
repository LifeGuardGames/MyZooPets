using UnityEngine;
using System.Collections;

public class PopupAward : MonoBehaviour {
	
	public Texture2D pointsIcon;
	public Texture2D starIcon;
	public Texture2D healthIcon;
	public Texture2D moodIcon;
	public Texture2D hungerIcon;
	
	private int deltaPoints, deltaStars, deltaHealth, deltaMood, deltaHunger;
	
	private LTRect awardRect;
	
	void Start(){
		awardRect = new LTRect(Screen.width/2 - 150, -100, 300, 400);
		//awardHash = new Hashtable<string, string>();
		
		Hashtable optional = new Hashtable();
		optional.Add("ease", LeanTweenType.easeOutBounce);
		LeanTween.move(awardRect, new Vector2(Screen.width/2 - awardRect.rect.width/2, Screen.height/2 + 150), 1.0f, optional);
	}
	
	//TODO-s correct way to contruct initilization?
	public void Populate(int _deltaPoints, int _deltaStars, int _deltaHealth, int _deltaMood, int _deltaHunger){
		deltaPoints = _deltaPoints;
		deltaStars = _deltaStars;
		deltaHealth = _deltaHealth;
		deltaMood = _deltaMood;
		deltaHunger = _deltaHunger;
	}
	
	void OnGUI(){
		GUILayout.BeginArea(awardRect.rect);
		GUILayout.BeginVertical();
		
		if(deltaPoints != 0){
			GUILayout.BeginHorizontal();
			GUILayout.Box(pointsIcon, GUILayout.Height(60), GUILayout.Width(60));
			GUI.color = Color.black;
			GUILayout.Label(deltaPoints.ToString());
			GUI.color = Color.white;
			GUILayout.EndHorizontal();
		}
		if(deltaStars != 0){
			GUILayout.BeginHorizontal();
			GUILayout.Box(starIcon, GUILayout.Height(60), GUILayout.Width(60));
			GUI.color = Color.black;
			GUILayout.Label(deltaStars.ToString());
			GUI.color = Color.white;
			GUILayout.EndHorizontal();
		}
		if(deltaHealth != 0){
			GUILayout.BeginHorizontal();
			GUILayout.Box(healthIcon, GUILayout.Height(60), GUILayout.Width(60));
			GUI.color = Color.black;
			GUILayout.Label(deltaHealth.ToString());
			GUI.color = Color.white;
			GUILayout.EndHorizontal();
		}
		if(deltaMood != 0){
			GUILayout.BeginHorizontal();
			GUILayout.Box(moodIcon, GUILayout.Height(60), GUILayout.Width(60));
			GUI.color = Color.black;
			GUILayout.Label(deltaMood.ToString());
			GUI.color = Color.white;
			GUILayout.EndHorizontal();
		}
		if(deltaHunger != 0){
			GUILayout.BeginHorizontal();
			GUILayout.Box(hungerIcon, GUILayout.Height(60), GUILayout.Width(60));
			GUI.color = Color.black;
			GUILayout.Label(deltaHunger.ToString());
			GUI.color = Color.white;
			GUILayout.EndHorizontal();
		}
		
		GUILayout.EndVertical();
		GUILayout.EndArea();
	}
}
