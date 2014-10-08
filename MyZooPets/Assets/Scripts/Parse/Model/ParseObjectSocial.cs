using UnityEngine;
using System.Collections;
using Parse;

[ParseClassName("Social")]
public class ParseObjectSocial : ParseObject{

	public ParseObjectSocial(){}

	[ParseFieldName("numOfFriendReferral")]
	public int NumOfFriendReferral{
		get{ return GetProperty<int>("NumOfFriendReferral"); }
		set{ SetProperty<int>(value, "NumOfFriendReferral"); }
	}

	[ParseFieldName("isReferralRewardClaimed")]
	public bool IsReferralRewardClaimed{
		get{ return GetProperty<bool>("IsReferralRewardClaimed"); }
		set{ SetProperty<bool>(value, "IsReferralRewardClaimed"); }
	}
}
