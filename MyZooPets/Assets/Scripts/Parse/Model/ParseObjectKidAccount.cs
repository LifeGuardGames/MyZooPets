using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Parse;

[ParseClassName("KidAccount")]
public class ParseObjectKidAccount : ParseObject{

	public ParseObjectKidAccount(){}

	[ParseFieldName("isLinkedToParentAccount")]
	public bool IsLinkedToParentAccount{
		get{ return GetProperty<bool>("IsLinkedToParentAccount");}
		set{ SetProperty<bool>(value, "IsLinkedToParentAccount");}
	}

	[ParseFieldName("friendList")]
	public IList<ParseObject> FriendList{
		get{ return GetProperty<IList<ParseObject>>("FriendList");}
//		set{ SetProperty<IList<ParseObject>>(value, "FriendList");}
	}

	[ParseFieldName("accountCode")]
	public string AccountCode{
		get{ return GetProperty<string>("AccountCode");}
//		set{ SetProperty<string>(value, "AccountCode");}
	}
	
	[ParseFieldName("createdBy")]
	public ParseUser CreatedBy{
		get{ return GetProperty<ParseUser>("CreatedBy");}
		set{ SetProperty<ParseUser>(value, "CreatedBy");}
	}

	[ParseFieldName("petInfo")]
	public ParseObjectPetInfo PetInfo{
		get{ return GetProperty<ParseObjectPetInfo>("PetInfo");}
		set{ SetProperty<ParseObjectPetInfo>(value, "PetInfo");}
	}

	[ParseFieldName("petAccessory")]
	public ParseObject PetAccessory{
		get{ return GetProperty<ParseObject>("PetAccessory");}
		set{ SetProperty<ParseObject>(value, "PetAccessory");}
	}
}
