using UnityEngine;
using System.Collections;

public class BedroomInhalerUIManager : MonoBehaviour {

	public Animation spinningAnimation;
	public GameObject starParticle;
	public GameObject rechargeParticle;

	// Start the correct animations based on its state
	void Start(){
		if(PlayPeriodLogic.Instance.CanUseRealInhaler){
			spinningAnimation.Play();
			starParticle.SetActive(true);
			rechargeParticle.SetActive(false);
		}
		else{
			spinningAnimation.Stop();
			starParticle.SetActive(false);
			rechargeParticle.SetActive(true);
		}
	}
}
