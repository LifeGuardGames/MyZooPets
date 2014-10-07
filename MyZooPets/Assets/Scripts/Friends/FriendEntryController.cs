using UnityEngine;
using System.Collections;

public class FriendEntryController : MonoBehaviour {

	public UISprite friendSprite;
	public UILabel friendLabel;


	public void Initilize(string friendName, Hashtable friendPetInfo){
		friendLabel.text = friendName;
	}
}
