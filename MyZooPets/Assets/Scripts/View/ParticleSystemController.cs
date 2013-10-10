using UnityEngine;
using System.Collections;

public class ParticleSystemController : MonoBehaviour {

	public ParticleSystem pSystem;
	
	void Start(){
		
		// If this is null try to find its own
		if(pSystem == null){
			pSystem = GetComponent<ParticleSystem>();
		}
		
		// If still null puke nasty things in log
		if(pSystem == null){
			Debug.LogError("No particle system found for ParticleSystemController");
		}
	}
	
	public void Play(){
		pSystem.Play();
	}
}
