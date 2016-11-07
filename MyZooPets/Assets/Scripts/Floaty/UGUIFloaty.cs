using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
public class UGUIFloaty : MonoBehaviour{
	public Text textUI;
	public CanvasGroup canvasGroup;

	public virtual void StartFloaty(Vector3 startPosition, string text, float riseTime, Vector3 offset){
		transform.position = startPosition;
		if(!string.IsNullOrEmpty(text)) {
			textUI.text = text;
		}
		LeanTween.value(gameObject, SetAlpha, 1, 0, riseTime).setEase(LeanTweenType.easeOutQuad);
		LeanTween.move(gameObject, transform.position + offset, riseTime).setEase(LeanTweenType.easeOutQuad).setDestroyOnComplete(true);
	}

	public virtual void StartFloatyLocal(string text, float riseTime, Vector3 offset) {
		transform.localPosition = Vector3.zero;
		if(!string.IsNullOrEmpty(text)) {
			textUI.text = text;
		}
		LeanTween.value(gameObject, SetAlpha, 1, 0, riseTime).setEase(LeanTweenType.easeOutQuad);
		LeanTween.moveLocal(gameObject, offset, riseTime).setEase(LeanTweenType.easeOutQuad).setDestroyOnComplete(true);
	}

	private void SetAlpha(float value) {
		canvasGroup.alpha = value;
	}
}
