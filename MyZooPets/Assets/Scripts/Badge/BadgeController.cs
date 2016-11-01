using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Interface for controlling the badges via the BadgeBoardUIManager
/// Handles badge animation, flipping, and init
/// </summary>
public class BadgeController : MonoBehaviour {
	public Image image;

	private ImmutableDataBadge badgeData;
	private bool isUnlocked;
	private int sortingOrder;

	public void Init(ImmutableDataBadge _badgeData){
		badgeData = _badgeData;
		isUnlocked = BadgeManager.Instance.IsBadgeUnlocked(_badgeData.ID);
		image.sprite = SpriteCacheManager.GetBadgeSprite(isUnlocked ? _badgeData.ID : null);
	}

	/// <summary>
	/// Events from BadgePopController
	/// </summary>
	public void FlipSpriteEvent(){
		image.sprite = SpriteCacheManager.GetBadgeSprite(badgeData.ID);
	}

	public void AnimationDoneEvent(){
		StartCoroutine(BadgeBoardUIManager.Instance.BadgeAnimationDone());
	}

	// Button call
	public void OnBadgeClicked() {
		if(!BadgeBoardUIManager.Instance.IsBadgeBoardUIAnimating && !BadgeBoardUIManager.Instance.baseDemux.IsMoving) {
			BadgeBoardUIManager.Instance.BadgeClicked(gameObject);
		}
	}
}
