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
	public List<object> FriendList{
		get{ return GetProperty<List<object>>("FriendList");}
//		set{ SetProperty<List<object>>(value, "FriendList");}
	}

	[ParseFieldName("createdBy")]
	public ParseUser CreatedBy{
		get{ return GetProperty<ParseUser>("CreatedBy");}
		set{ SetProperty<ParseUser>(value, "CreatedBy");}
	}
}
