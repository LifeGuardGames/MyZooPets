using UnityEngine;
using System.Collections;

public class progressBar : MonoBehaviour {
	
	private const float NATIVE_WIDTH = 1280.0f;
    private const float NATIVE_HEIGHT = 800.0f;
	public float progress;
	public float food;
	public float mood;
	public float health;
	public Texture2D progressBarFrame;
	public Texture2D progressBarFill;
	public Texture2D statBarVerFill;
	public Texture2D statBarVerFrame;
	
	
	public GUIStyle expreTextStyle;
	public GUIStyle tierTextStyle;
	
	// Use this for initialization
	void Start () {
		progress = 50f;
		food = 30f;
		mood = 50f;
		health = 80f;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnGUI(){
		
		if (NATIVE_WIDTH != Screen.width || NATIVE_HEIGHT != Screen.height){
            float horizRatio = Screen.width/NATIVE_WIDTH;
            float vertRatio = Screen.height/NATIVE_HEIGHT;
            GUI.matrix = Matrix4x4.TRS(new Vector3(0, 0, 0), Quaternion.identity, new Vector3(horizRatio, vertRatio, 1));
		}
		
		GUI.DrawTexture(new Rect(150,13,350,50),progressBarFrame);
		GUI.DrawTexture(new Rect(150,13,350 * Mathf.Clamp01(progress/100),50),progressBarFill, ScaleMode.ScaleAndCrop, true, 150/13);
		
		GUI.DrawTexture(new Rect(60,95,25,70),statBarVerFrame);
		GUI.DrawTexture(new Rect(60,95+(70-70*health/100),25, 70 * Mathf.Clamp01(health/100)),statBarVerFill, ScaleMode.ScaleAndCrop, true, 25/70);
		
		GUI.DrawTexture(new Rect(60,195,25,70),statBarVerFrame);
		GUI.DrawTexture(new Rect(60,195+(70-70*mood/100),25, 70 * Mathf.Clamp01(mood/100)),statBarVerFill, ScaleMode.ScaleAndCrop, true, 25/70);
		
		GUI.DrawTexture(new Rect(60,295,25,70),statBarVerFrame);
		GUI.DrawTexture(new Rect(60,295+(70-70*food/100),25, 70 * Mathf.Clamp01(food/100)),statBarVerFill, ScaleMode.ScaleAndCrop, true, 25/70);		
		
		GUI.TextField(new Rect(230,14,200,40),"5000/10000",expreTextStyle);
		GUI.TextField(new Rect(25,14,200,40),"Tier 1",tierTextStyle);
	}
}
