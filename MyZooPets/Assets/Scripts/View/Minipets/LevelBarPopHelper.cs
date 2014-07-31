using UnityEngine;
using System.Collections;

public class LevelBarPopHelper : MonoBehaviour {

	public ParticleSystem particle;

	// Play this when pop occurs
	public void PopAction(){
		if(particle){
			particle.Play();
		}

		MiniPetHUDUIManager.Instance.LevelUpAnimationCompleted();
	}
}
