using UnityEngine;
using System.Collections;

/// <summary>
/// Badge controller.
/// Interface for controlling the badges via the BadgeBoardUIManager
/// Handles badge animation, flipping, and init
/// </summary>
public class BadgeController : MonoBehaviour {
	public Animation unlockAnimation;
	public SendMessageEvent messageEvent;
	public UISprite sprite;

	private bool isUnlocked;
	private string activeSpriteName;
	private string inactiveSpriteName;

	public void Init(bool isUnlocked, string activeSpriteName, string inactiveSpriteName){
		this.isUnlocked = isUnlocked;
		this.activeSpriteName = activeSpriteName;
		this.inactiveSpriteName = inactiveSpriteName;

		sprite.spriteName = isUnlocked ? activeSpriteName : inactiveSpriteName;
	}

	public void PlayUnlockAnimation(){
		if(!isUnlocked){
			isUnlocked = true;
			unlockAnimation.Play();
		}
		else{
			Debug.LogError("Badge already unlocked! You are trying to play unlock animation");
		}
	}

	/// <summary>
	/// First event call from the SendMessageEvent object
	/// </summary>
	public void PopUnlockBadgeEvent(){
		Debug.Log("popbadge");
		sprite.spriteName = activeSpriteName;
	}

	public void AnimationDoneEvent(){
		Debug.Log("Anim done " + gameObject.name);
		StartCoroutine(BadgeBoardUIManager.Instance.BadgeAnimationDone());
	}
}
