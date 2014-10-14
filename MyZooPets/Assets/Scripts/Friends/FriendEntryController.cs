using UnityEngine;
using System.Collections;

public class FriendEntryController : MonoBehaviour {

	public UISprite friendSprite;
	public UILabel friendLabel;
	public LgButtonMessage deleteButtonMessage;

	public string FriendName {get; set;}
	public string FriendID {get; set;}

	public void Initilize(string friendName, string friendID, Hashtable petInitInfo){
		FriendName = friendName;
		FriendID = friendID;
		friendLabel.text = friendName;

		if(petInitInfo != null){
			friendSprite.spriteName = "petEntry" + petInitInfo["Color"];
		}
		else{
			Debug.LogWarning("No pet info detected for friendlist");
		}

		// Assign delete button properties
		deleteButtonMessage.target = FriendsUIManager.Instance.gameObject;
		deleteButtonMessage.functionName = "OpenDeleteFriendWindowCallback";
	}
}
