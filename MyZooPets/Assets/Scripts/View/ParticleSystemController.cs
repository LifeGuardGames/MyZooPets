using UnityEngine;
using System.Collections;

public class ParticleSystemController : MonoBehaviour {

	public ParticleSystem pSystem;
	public bool destroyAfterStop;
	public float destroyAfterStopDelay = 0;
	
	void Start(){
		
		// If this is null try to find its own
		if(pSystem == null){
			pSystem = GetComponent<ParticleSystem>();
		}
		
		// If still null puke nasty things in log
		if(pSystem == null){
			Debug.LogError("No particle system found for ParticleSystemController");
		}
		
		_Start();
	}
	
	protected virtual void _Start(){
		// Overriden in child
	}
	
	void Update(){
		_Update();
	}
	
	protected virtual void _Update(){
		// Overridden in child
	}
	
	public void Play(){
		pSystem.Play();
		_Play();
	}
	
	public void Play(float fDelay){
		StartCoroutine(PlayAfterDelay(fDelay));
	}
	
	IEnumerator PlayAfterDelay(float fDelay){
		yield return new WaitForSeconds(fDelay);
		Play();
	}	
	
	protected virtual void _Play(){
		// Overridden in child	
	}
	
	public void Stop(){
		pSystem.Stop();
		_Stop();
		
		if(destroyAfterStop){
			Destroy (gameObject, destroyAfterStopDelay);
		}
	}
	
	protected virtual void _Stop(){
		// Overridden in child
	}
}
