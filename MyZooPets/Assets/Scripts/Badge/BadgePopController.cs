using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// Used for Unlock badge animation, UI canvas requires us to do this on aux for layering issues
/// </summary>
public class BadgePopController : MonoBehaviour {
	public Image imageAux;
	public Animation popAnimation;
	public ParticleSystem slamParticle;

	private BadgeController poppedBadgeController;

	void Start() {
		imageAux.enabled = false;	// Animation will turn this on
	}

	public void InitializeAndPlay(GameObject poppedBadge) {
		poppedBadgeController = poppedBadge.GetComponent<BadgeController>();
		transform.position = poppedBadge.transform.position;
		popAnimation.Play();
    }

	public void PopUnlockBadgeEvent() {
		poppedBadgeController.FlipSpriteEvent();
    }

	public void AnimationDoneEvent() {
		poppedBadgeController.AnimationDoneEvent();
	}

	public void PlaySoundEvent() {
		AudioManager.Instance.PlayClip("BadgeSlam");
	}

	public void PlaySlamParticleEvent() {
		slamParticle.Play();
	}
}
