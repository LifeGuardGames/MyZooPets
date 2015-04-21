using UnityEngine;
using System.Collections;

public class BonusVisualController : MonoBehaviour {
	public Animation labelAnimation;
	public ParticleSystem bonusVisualParticle;

	public void PlayBonusVisuals(){
		labelAnimation.Play("BonusLabelPop");
		bonusVisualParticle.Play();
	}

	public void StopBonusVisuals(){
		labelAnimation.Play("BonusLabelExit");
		bonusVisualParticle.Stop();
	}
}
