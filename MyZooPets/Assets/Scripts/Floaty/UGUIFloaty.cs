using UnityEngine;
using UnityEngine.UI;
using System.Collections;
public class UGUIFloaty : MonoBehaviour {
	public Text textUI;
	private float hangTime;
	//public Sprite sprite; //TODO: Implement sprite rendering

	public void StartFloaty(Vector3 startPosition, string text = "", int textSize = 14, float riseTime = .5f,  Vector3? toMove = null, Color? color = null) {
		color =  color ?? Color.white; //color and toMove are entered as a nullable so that there can be defaults
		toMove =  toMove ?? Vector3.zero; //?? = null-coalescing operator
		transform.position = startPosition; //Not optional
		textUI.text = text;
		textUI.fontSize = textSize;
		textUI.color=(Color) color;
		LeanTween.textAlpha(GetComponent<RectTransform>(),0,riseTime*3).setEase(LeanTweenType.easeOutQuad);
		LeanTween.move(gameObject, transform.position+ (Vector3) toMove, riseTime).setOnComplete(OnComplete).setEase(LeanTweenType.easeOutQuad);
	}

	private void OnComplete() {
		Destroy(gameObject);
	}
}
