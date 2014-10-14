using UnityEngine;
using System.Collections;

public class RequestEntryController : MonoBehaviour {
	public UILabel labelName;
	private string requestId;

	public void Initialize(string requestId, string friendName){
		this.requestId = requestId;
		labelName.text = friendName;
	}

	public void ButtonAccept(){
		FriendsUIManager.Instance.RequestAccept(this.requestId);
	}

	public void ButtonDecline(){
		FriendsUIManager.Instance.RequestDecline(this.requestId);
	}
}
