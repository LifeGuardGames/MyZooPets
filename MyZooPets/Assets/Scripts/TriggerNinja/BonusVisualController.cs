using UnityEngine;
using System.Collections;

public class BonusVisualController : MonoBehaviour {
	public Animation labelAnimation;
	public ParticleSystem bonusVisualParticle;
	private bool isPlaying = false;

	public void PlayBonusVisuals(){
		if(!isPlaying){
			isPlaying = true;
			labelAnimation.Play("BonusLabelPop");
			bonusVisualParticle.Play();
		}
	}

	public void StopBonusVisuals(){
		if(isPlaying){
			isPlaying = false;
			labelAnimation.Play("BonusLabelExit");
			bonusVisualParticle.Stop();
		}
	}
}
