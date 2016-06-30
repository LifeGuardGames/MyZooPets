using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
public class FloatyController : MonoBehaviour {
	public float duration = 1f;
	public Vector2 positionDelta = new Vector3(0f, 50f);
	public Text textLabel;
	public Image image;

	private RectTransform rectTrans;

	public void InitAndActivate(Vector3 UIPosition, string customText = null, Sprite spriteData = null) {
		gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(UIPosition.x, UIPosition.y);
		
		// Set default CanvasGroup behaviour
		CanvasGroup canvasGroup = GetComponent<CanvasGroup>();
		canvasGroup.interactable = false;
		canvasGroup.blocksRaycasts = false;
		rectTrans = GetComponent<RectTransform>();

		// Initialize if there are dynamic inputs (stats, etc.)
		if(customText != null) {
			textLabel.text = customText;
		}
		if(spriteData != null) {
			image.sprite = spriteData;
		}

		// Activate
		LeanTween.value(gameObject, ReduceVisibility, 1.0f, 0.0f, duration);
		LeanTween.value(gameObject, Move, rectTrans.anchoredPosition,
			 rectTrans.anchoredPosition + positionDelta, duration)
			.setDestroyOnComplete(true);
	}

	private void ReduceVisibility(float amount) {
		gameObject.GetComponent<CanvasGroup>().alpha = amount;
	}

	private void Move(Vector2 pos) {
		rectTrans.anchoredPosition = pos;
	}
}
