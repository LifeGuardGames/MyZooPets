using UnityEngine;
using System.Collections;

public class MinipetEggClickController : MonoBehaviour {

	public MiniPet minipet;	// Parent minipet
	public int clicksUntilHatch = 10;
	public string click1Animation;
	public string click2Animation;
	public Animation eggAnimation;
	public ParticleSystem eggParticle;

	private bool clickAuxToggle = true;

	void OnTap(TapGesture gesture){
		eggAnimation.Play(clickAuxToggle ? click1Animation : click2Animation);
		clickAuxToggle = !clickAuxToggle;
		eggParticle.Play();
		AudioManager.Instance.PlayClip("eggCrack");

		if(--clicksUntilHatch <= 0){
			MiniPetManager.Instance.StartHatchSequence(minipet.ID);

			// Tidy up
			Destroy(this.collider);
			Destroy(this);
		}
	}
}
