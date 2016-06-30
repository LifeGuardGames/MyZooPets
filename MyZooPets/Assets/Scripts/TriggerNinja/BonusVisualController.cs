using UnityEngine;
using UnityEngine.UI;

public class BonusVisualController : MonoBehaviour {
	public Animation labelAnimation;
	public ParticleSystem bonusVisualParticle;
	private bool isPlaying = false;

	public void PlayBonusVisuals(){
		if(!isPlaying){
			labelAnimation.gameObject.SetActive(true);
			labelAnimation.Play("BonusLabelPop");
			isPlaying = true;
			bonusVisualParticle.Play();
		}
	}

	public void StopBonusVisuals(){
		if(isPlaying){
			labelAnimation.gameObject.SetActive(false);
			isPlaying = false;
			bonusVisualParticle.Stop();
		}
	}
}
