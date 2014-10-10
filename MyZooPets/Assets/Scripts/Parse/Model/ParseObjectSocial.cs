using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Parse;

[ParseClassName("Social")]
public class ParseObjectSocial : ParseObject{

	public ParseObjectSocial(){}

	/// <summary>
	/// Gets the friend list. The current friend list of the user
	/// </summary>
	/// <value>The friend list.</value>
//	[ParseFieldName("currentFriendList")]
//	public IList<ParseObjectKidAccount> FriendList{
//		get{ return GetProperty<IList<ParseObjectKidAccount>>("FriendList");}
//		//		set{ SetProperty<IList<ParseObject>>(value, "FriendList");}
//	}

	/// <summary>
	/// How mandy rewards are to be given to the user
	/// </summary>
	/// <value>The reward count.</value>
	[ParseFieldName("rewardCount")]
	public int RewardCount{
		get{ return GetProperty<int>("RewardCount"); }
		set{ SetProperty<int>(value, "RewardCount"); }
	}

	/// <summary>
	/// Gets or sets the number of stars. How many stars to show to indicate
	/// friend referral progress
	/// </summary>
	/// <value>The number of stars.</value>
	[ParseFieldName("numOfStars")]
	public int NumOfStars{
		get{ return GetProperty<int>("NumOfStars"); }
		set{ SetProperty<int>(value, "NumOfStars"); }
	}
}
