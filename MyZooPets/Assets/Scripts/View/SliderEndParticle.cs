using UnityEngine;
using System.Collections;

public class SliderEndParticle : MonoBehaviour {
	public ParticleSystem particle;
	public UISlider slider;

	void Start(){
		if(particle == null){
			particle = GetComponent<ParticleSystem>();

			if(particle == null){
				Debug.LogError("No particle system found for SliderEndParticle");
			}
		}
	}

	// Callback from UISlider
	public void OnSliderChange(){
		// Move the particle to the end of the newly updated position, make sure parented correctly!
		transform.localPosition = new Vector3(slider.transform.localScale.x, 0, 0);
		particle.Play();
	}

}
