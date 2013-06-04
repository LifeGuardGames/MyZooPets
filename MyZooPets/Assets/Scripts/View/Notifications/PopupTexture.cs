using UnityEngine;
using System.Collections;

public class PopupTexture : MonoBehaviour {
	
	public Texture2D greatTexture;
	public LTRect textureRect = new LTRect(0, 0, 0, 0);
	//public LTRect textureRect2 = new LTRect(0, 0, 551, 175);
	
	//public Vector2 initPosition = new Vector2(100, 100);
	public Vector2 initScale = new Vector2(0, 0);
	public Vector2 initRotation;
	
	//public Vector2 finalPosition = new Vector2(100, 100);
	public Vector2 finalScale = new Vector2(551, 175);
	public Vector2 finalRotation;
	
	void Start(){
		Hashtable optional = new Hashtable();
		optional.Add("ease", LeanTweenType.easeInOutQuad);
		LeanTween.scale(textureRect, new Vector2(551, 175), 0.5f, optional);
	}
	
	void Update(){
		Debug.Log(textureRect.rect.width + " " + textureRect.rect.height);
	}
	
	void OnGUI(){
		GUI.DrawTexture(textureRect.rect, greatTexture);
	}
}
