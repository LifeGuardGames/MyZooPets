using UnityEngine;
using System.Collections;

public class Fader : MonoBehaviour {

	
	void Awake()
	{
		guiTexture.pixelInset = new Rect(0,0,0,0);
	}
	
	void OnGUI()
	{
		
		//guiTexture.pixelInset = new Rect(0,0,1280,800);
	}
	
	void Update()
	{
		StartScene();	
	}
	
	void StartScene()
	{
		guiTexture.color = Color.Lerp(guiTexture.color,Color.clear,0.5f * Time.deltaTime);
		if(guiTexture.color.a <= 0.05f)
		{
			guiTexture.color = Color.clear;
			guiTexture.enabled = false;
		}
	}
}
