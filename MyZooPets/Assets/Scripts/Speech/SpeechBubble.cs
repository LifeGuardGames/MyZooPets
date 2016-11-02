using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// SpeechBubble prefab to be loaded, handles initialization and button click storage
/// </summary>
public class SpeechBubble : MonoBehaviour {
	public Text text;
	public Image image;
	public LgUIButton button;
	public Animator anim;

	private GameObject objectToCall;
	private string functionNameToCall;

	public void Init(string textString = null, string imageSprite = null, GameObject _objectToCall = null, string _functionNameToCall = null, UIModeTypes _modeType = UIModeTypes.None) {
		if(!string.IsNullOrEmpty(textString)) {
			text.text = textString;
		}
		if(!string.IsNullOrEmpty(imageSprite)) {
			image.sprite = SpriteCacheManager.GetSprite(imageSprite);
		}
		if(_objectToCall != null) {
			objectToCall = _objectToCall;
			if(!string.IsNullOrEmpty(_functionNameToCall)) {
				functionNameToCall = _functionNameToCall;
			}
			button.modeType1 = _modeType;
			button.interactable = true;
		}
		else {
			button.interactable = false;
		}
	}

	public void CallTarget() {
		objectToCall.SendMessage(functionNameToCall, gameObject, SendMessageOptions.RequireReceiver);
	}

	public void Finish() {
		anim.Play("SpeechBubbleFinish");
    }

	// Event from animation
	public void DestroySelf() {
		Destroy(gameObject);
	}
}
