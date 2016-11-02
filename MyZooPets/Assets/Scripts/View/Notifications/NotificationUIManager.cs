using UnityEngine;
using System.Collections;

/// <summary>
/// Should be in every scene within UI Canvas.
/// Manages a Queue of notification data, popped one at a time
/// </summary>
public class NotificationUIManager : Singleton<NotificationUIManager> {
	public TweenToggleDemux fadeDemux;

	private bool isNotificationActive = false;
	public bool IsNotificationActive() {
		return isNotificationActive;
	}

	void Start() {
		// Start is called after some notifications pushed!!! Check beforehand
		if(!isNotificationActive) {
			// Check the static queue to see if anything is there on level load
			TryNextNotification();
		}
	}

	/// <summary>
	/// Adds new notification to queue.
	/// </summary>
	/// <param name="notificationEntry">Notification entry.</param>
	public void AddToQueue(Hashtable notificationEntry) {
		NotificationQueueData.AddNotification(notificationEntry);

		if(!isNotificationActive) {
			TryNextNotification();
		}
	}

	public void TryNextNotification() {
		if(!NotificationQueueData.IsEmpty()) {
			// Block all UI clicks
			fadeDemux.Show();

			isNotificationActive = true;
			Hashtable entry = NotificationQueueData.PopNotification();

			StartCoroutine(ShowPopup(
				(string)entry[NotificationPopupData.PrefabName],
				(string)entry[NotificationPopupData.Title],
				(string)entry[NotificationPopupData.Message],
				(PopupController.Callback)entry[NotificationPopupData.SpecialButtonCallback]
			));
		}
		else {
			isNotificationActive = false;

			// Release block on UI clicks
			fadeDemux.Hide();
		}
	}

	// Loads the prefab and inits the prefab's PopupController values
	public IEnumerator ShowPopup(string prefabName, string title, string message,
		PopupController.Callback specialButtonCallback) {
		GameObject popupRef = Resources.Load<GameObject>(prefabName);
		GameObject popup = GameObjectUtils.AddChildGUI(gameObject, popupRef);
		PopupController controller = popup.GetComponent<PopupController>();
		controller.Init(title, message, specialButtonCallback);
		yield return 0;     // Wait a frame for the popup to finish
		controller.ShowPopup();
	}

	void OnGUI() {
		if(GUI.Button(new Rect(100, 100, 100, 100), "open")) {
			StatsManager.Instance.ChangeStats(xpDelta: 140);
		}
	}
}
