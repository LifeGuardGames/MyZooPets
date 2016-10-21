using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
public class UGUIFloaty : MonoBehaviour{
	public Text textUI;
	public CanvasGroup canvasGroup;

	public void StartFloaty(Vector3 startPosition, string text = "", float riseTime = 1f, Vector3? offset = null){
		offset = offset ?? new Vector3(0, 0.6f);	//?? = null-coalescing operator
		transform.position = startPosition;
		if(textUI != null) {
			textUI.text = text;
		}
		LeanTween.value(gameObject, SetAlpha, 1, 0, riseTime).setEase(LeanTweenType.easeOutQuad);
		LeanTween.move(gameObject, transform.position + (Vector3)offset, riseTime).setEase(LeanTweenType.easeOutQuad).setDestroyOnComplete(true);
	}

	private void SetAlpha(float value) {
		canvasGroup.alpha = value;
	}
}
