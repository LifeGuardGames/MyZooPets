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
	public bool IsUnlocked{
		get{ return isUnlocked; }
	}

	private string activeSpriteName;
	public string ActiveSpriteName{
		get{ return activeSpriteName; }
	}

	private string inactiveSpriteName;
	public string InactiveSpriteName{
		get{ return inactiveSpriteName; }
	}

	private int renderOrderAux;

	public void Init(bool isUnlocked, string activeSpriteName, string inactiveSpriteName){
		this.isUnlocked = isUnlocked;
		this.activeSpriteName = activeSpriteName;
		this.inactiveSpriteName = inactiveSpriteName;

		sprite.spriteName = isUnlocked ? activeSpriteName : inactiveSpriteName;
		renderOrderAux = sprite.depth;
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
		sprite.spriteName = activeSpriteName;
	}

	public void PlaySlamParticle(){
		BadgeBoardUIManager.Instance.PlaySlamParticle(transform.position);
	}

	public void AnimationDoneEvent(){
		StartCoroutine(BadgeBoardUIManager.Instance.BadgeAnimationDone());
	}

	// For use in animation event, set it to render in front of others if true
	// 0 for false, 1 for true
	public void RenderOrderFrontToggle(int isFront){
		sprite.depth = (isFront == 1) ? 20 : renderOrderAux;
	}
}
