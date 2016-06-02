using UnityEngine;
using UnityEngine.UI;

public class PopupController : MonoBehaviour {
	public delegate void Callback();
	public Callback specialButtonCallback;  // Support for one special callback button
	public Text title;
	public Text textArea;
	public TweenToggleDemux demux;

	private NotificationUIManager notificationManager;

	// Call to prep notification
	public void Init(Callback _specialButtonCallback, string popupTitle, string popupText) {
		specialButtonCallback = _specialButtonCallback;
		title.text = popupTitle;
        textArea.text = popupText;
	}

	// Exit and do the assigned delegate function
	public void OnSpecialButton() {
		HidePopup();
		if(specialButtonCallback != null) {
			specialButtonCallback();
		}
	}

	// Normal exit routine
	public void OnExitButton() {
		HidePopup();
	}

	public void ShowPopup() {
		demux.Show();
		//AudioManager.Instance.PlayClip();
    }

	public void HidePopup() {
		demux.Hide();
		//AudioManager.Instance.PlayClip();
	}

	// Clear callbacks and continue notification queue
	public void OnHideFinished() {
		specialButtonCallback = null;
		notificationManager.TryNextNotification();
		Destroy(gameObject);
	}
}
