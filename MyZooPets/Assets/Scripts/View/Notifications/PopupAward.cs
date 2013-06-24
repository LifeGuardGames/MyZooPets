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
	// native dimensions
    private const float NATIVE_WIDTH = 1280.0f;
    private const float NATIVE_HEIGHT = 800.0f;
	
	void Start(){
		awardRect = new LTRect(NATIVE_WIDTH/2 - 150, NATIVE_HEIGHT/2, 300, 400);
		//awardHash = new Hashtable<string, string>();
		
		Hashtable optional = new Hashtable();
		optional.Add("ease", LeanTweenType.easeOutBounce);
		LeanTween.move(awardRect, new Vector2(NATIVE_WIDTH/2 - awardRect.rect.width/2, NATIVE_HEIGHT/2 + 150), 1.0f, optional);
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
		// Proportional scaling
		if (NATIVE_WIDTH != Screen.width || NATIVE_HEIGHT != Screen.height){
            float horizRatio = Screen.width/NATIVE_WIDTH;
            float vertRatio = Screen.height/NATIVE_HEIGHT;
            GUI.matrix = Matrix4x4.TRS(new Vector3(0, 0, 0), Quaternion.identity, new Vector3(horizRatio, vertRatio, 1));
		}

		GUILayout.BeginArea(awardRect.rect);
		GUILayout.BeginHorizontal();
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
		GUILayout.EndVertical();
		GUILayout.BeginVertical();
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
		
		GUILayout.EndHorizontal();
		GUILayout.EndArea();
	}
}
