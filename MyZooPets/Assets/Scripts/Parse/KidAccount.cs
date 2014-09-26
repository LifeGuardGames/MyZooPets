using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Parse;

[ParseClassName("KidAccount")]
public class KidAccount : ParseObject{

	public KidAccount(){}

	[ParseFieldName("isLinkedToParentAccount")]
	public bool IsLinkedToParentAccount{
		get{ return GetProperty<bool>("IsLinkedToParentAccount");}
		set{ SetProperty<bool>(value, "IsLinkedToParentAccount");}
	}

	[ParseFieldName("friendList")]
	public IList<ParseObject> FriendList{
		get{ return GetProperty<IList<ParseObject>>("FriendList");}
		set{ SetProperty<IList<ParseObject>>(value, "FriendList");}
	}

	[ParseFieldName("createdBy")]
	public ParseUser CreatedBy{
		get{ return GetProperty<ParseUser>("CreatedBy");}
		set{ SetProperty<ParseUser>(value, "CreatedBy");}
	}

	[ParseFieldName("petInfo")]
	public ParseObject PetInfoPointer{
		get{ return GetProperty<ParseObject>("PetInfoPointer");}
		set{ SetProperty<ParseObject>(value, "PetInfoPointer");}
	}
}
