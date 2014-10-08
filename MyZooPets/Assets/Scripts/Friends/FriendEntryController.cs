using UnityEngine;
using System.Collections;

public class FriendEntryController : MonoBehaviour {

	public UISprite friendSprite;
	public UILabel friendLabel;
	private LgButtonMessage deleteButtonMessage;

	public void Initilize(string friendName, Hashtable friendPetInfo){
		friendLabel.text = friendName;

		// Assign delete button properties
		deleteButtonMessage.target = FriendsUIManager.Instance.gameObject;
		deleteButtonMessage.functionName = "OpenDeleteFriendWindowCallback";
	}
}
