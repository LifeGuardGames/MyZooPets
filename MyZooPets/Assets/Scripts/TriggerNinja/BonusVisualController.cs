using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class BonusVisualController : MonoBehaviour {
	public Animation labelAnimation;
	public ParticleSystem bonusVisualParticle;
	public Text bonusText;
	private bool isPlaying = false;

	public void PlayBonusVisuals(){
		if(!isPlaying){
			bonusText.gameObject.SetActive(true);
			isPlaying = true;
			labelAnimation.Play("BonusLabelPop");
			bonusVisualParticle.Play();
		}
	}

	public void StopBonusVisuals(){
		if(isPlaying){
			bonusText.gameObject.SetActive(false);
			isPlaying = false;
			labelAnimation.Play("BonusLabelExit");
			bonusVisualParticle.Stop();
		}
	}
}
