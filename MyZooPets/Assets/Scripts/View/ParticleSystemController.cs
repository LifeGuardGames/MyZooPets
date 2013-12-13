using UnityEngine;
using System.Collections;

public class ParticleSystemController : MonoBehaviour {

	public ParticleSystem pSystem;
	public bool destroyAfterStop;
	public float destroyAfterStopDelay = 0;
	
	public bool randomActive = false;
	public float delayBeforeStart;
	public float minInterval;
	public float maxInterval;
	private float generatedValue;
	private float timeBegin;
	
	void Start(){
		
		// If this is null try to find its own
		if(pSystem == null){
			pSystem = GetComponent<ParticleSystem>();
		}
		
		// If still null puke nasty things in log
		if(pSystem == null){
			Debug.LogError("No particle system found for ParticleSystemController");
		}
		
		if(randomActive){
			if(maxInterval < minInterval){
				Debug.LogError("Max interval is less than min interval, clamping to min");
				maxInterval = minInterval;
			}
			generatedValue = Random.Range(minInterval, maxInterval);
			timeBegin = Time.time;
		}
		
		_Start();
	}
	
	protected virtual void _Start(){
		// Overriden in child
	}
	
	void Update(){
		if(randomActive){	
			if(Time.time > timeBegin + generatedValue){
				timeBegin = Time.time;
				generatedValue = Random.Range(minInterval, maxInterval);
				
				// Do the action here
				Play();
			}
		}
		_Update();
	}
	
	protected virtual void _Update(){
		// Overridden in child
	}
	
	public void ReachedTarget() {
		Play();
	}
	
	public void Play(){
		pSystem.Play();
		_Play();
	}
	
	protected virtual void _Play(){
		// Overridden in child	
	}

	public IEnumerator PlayAfterDelay(float fDelay){
		yield return new WaitForSeconds(fDelay);
		Play();
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
	
	// Note: If some script enables a bunch of buttons at the same time they will all animate at once the first time
	// TODO: untested...
	public void EnableRandom(){
		randomActive = true;
		if(maxInterval < minInterval){
			Debug.LogError("Max interval is less than min interval, clamping to min");
			maxInterval = minInterval;
		}
		generatedValue = Random.Range(minInterval, maxInterval);
		timeBegin = Time.time;
	}
	
	public void DisableRandom(){
		randomActive = false;
	}
}
