using UnityEngine;
using System.Collections;

public class PopupSpeechBubble : MonoBehaviour {
	
	public Texture2D greatTexture;
	public LTRect textureRect;
	
	void Start(){
		textureRect = new LTRect(Screen.width/2 - greatTexture.width/2, -300, 551, 175);
	}
	
	void Update(){
	
	}
	
	void OnGUI(){
		GUI.DrawTexture(textureRect.rect, greatTexture);
	}
}
