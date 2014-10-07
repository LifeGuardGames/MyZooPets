using UnityEngine;
using System.Collections;

public class RequestEntryController : MonoBehaviour {
	public UILabel labelName;
	public string name;

	public void Initialize(string name){
		this.name = name;
		labelName.text = name;
	}

	public void ButtonAccept(){
		FriendsUIManager.Instance.RequestAccept(name);
	}

	public void ButtonDecline(){
		FriendsUIManager.Instance.RequestDecline(name);
	}
}
