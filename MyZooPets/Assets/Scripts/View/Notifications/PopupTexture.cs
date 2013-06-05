using UnityEngine;
using System.Collections;

public class PopupTexture : MonoBehaviour {
	
	public Texture2D greatTexture;
	public LTRect textureRect;
	//public LTRect textureRect2 = new LTRect(0, 0, 551, 175);
	
	//public Vector2 initPosition = new Vector2(100, 100);
	//public Vector2 initScale = new Vector2(0, 0);
	//public Vector2 initRotation;
	
	//public Vector2 finalPosition = new Vector2(100, 100);
	//public Vector2 finalScale = new Vector2(551, 175);
	//public Vector2 finalRotation;
	
	void Start(){
		textureRect = new LTRect(Screen.width/2 - greatTexture.width/2, -300, 551, 175);
		
		Hashtable optional = new Hashtable();
//		optional.Add("ease", LeanTweenType.easeInOutQuad);
		//LeanTween.scale(textureRect, new Vector2(551, 175), 0.5f, optional);
		
		optional.Add("ease", LeanTweenType.easeOutBounce);
		LeanTween.move(textureRect, new Vector2(Screen.width/2 - greatTexture.width/2, Screen.height/2 - greatTexture.height/2), 1.0f, optional);
	}
	
	void Update(){

	}
	
	void OnGUI(){
		GUI.DrawTexture(textureRect.rect, greatTexture);
	}
}
