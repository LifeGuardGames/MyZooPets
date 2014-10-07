using UnityEngine;
using System.Collections;

public class FriendEntryController : MonoBehaviour {

	public UISprite friendSprite;
	public UILabel friendLabel;


	public void Populate(string friendName, Hashtable friendPetInfo){
		friendLabel.text = friendName;
	}
}
