using UnityEngine;
using System.Collections;

public class PopupTexture : MonoBehaviour {

	// Native dimensions
    private const float NATIVE_WIDTH = 1280.0f;
    private const float NATIVE_HEIGHT = 800.0f;
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
		textureRect = new LTRect(NATIVE_WIDTH/2 - greatTexture.width/2, -300, greatTexture.width, greatTexture.height);

		Hashtable optional = new Hashtable();
//		optional.Add("ease", LeanTweenType.easeInOutQuad);
		//LeanTween.scale(textureRect, new Vector2(551, 175), 0.5f, optional);

		optional.Add("ease", LeanTweenType.easeOutBounce);
		LeanTween.move(textureRect, new Vector2(NATIVE_WIDTH/2 - greatTexture.width/2, NATIVE_HEIGHT/2 - greatTexture.height/2), 1.0f, optional);
	}

	void Update(){

	}

	void OnGUI(){
		if (NATIVE_WIDTH != Screen.width || NATIVE_HEIGHT != Screen.height){
            float horizRatio = Screen.width/NATIVE_WIDTH;
            float vertRatio = Screen.height/NATIVE_HEIGHT;
            GUI.matrix = Matrix4x4.TRS(new Vector3(0, 0, 0), Quaternion.identity, new Vector3(horizRatio, vertRatio, 1));
		}
		GUI.DrawTexture(textureRect.rect, greatTexture);
	}
}
