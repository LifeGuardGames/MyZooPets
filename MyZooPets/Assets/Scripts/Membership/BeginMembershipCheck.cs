using UnityEngine;
using System.Collections;

/// <summary>
/// Begin membership check.
/// This script is attached to MembershipCheck gameobject. It's main purpose is
/// to activate membership check when appropriate. Membership check only happens 
/// when the game is in LoadingScene.unity. 
/// This class 
/// </summary>
public class BeginMembershipCheck : MonoBehaviour {

	// Use this for initialization
	void Start(){
		MembershipCheck.Instance.StartCheck();
	}
}
