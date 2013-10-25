using UnityEngine;
using System.Collections;

/// <summary>
/// Lg particle emitter.
/// Emits particles at random between the two constants: min and max interval
/// Just instantiates it, the behaviour of the particle is up to itself
/// Note: Requires NGUITools for add child
/// </summary>
public class LgParticleEmitter : MonoBehaviour {
	
	public GameObject particleObject;
	
	//Timer stuff
	public bool isActive;
	public float delayBeforeStart;
	public float minInterval;
	public float maxInterval;
	private float generatedValue;
	private float timeBegin;
	
	void Start(){
		if(maxInterval < minInterval){
			Debug.LogError("Max interval is less than min interval, clamping to min");
			maxInterval = minInterval;
		}
		generatedValue = Random.Range(minInterval, maxInterval);
		timeBegin = Time.time;
		
		_Start();
	}
	
	protected virtual void _Start(){
		// Override in child
	}
	
	void OnDestroy() {
		_OnDestroy();	
	}
	
	protected virtual void _OnDestroy() {
		
	}
	
	void Update(){
		if(isActive){	
			if(Time.time > timeBegin + generatedValue){
				timeBegin = Time.time;
				generatedValue = Random.Range(minInterval, maxInterval);
				
				// Do the action here
				GameObject emittedObject = NGUITools.AddChild(gameObject, particleObject);
				_ExtendedAction(emittedObject);
			}
		}
	}
	
	protected virtual void _ExtendedAction(GameObject emittedObject){
		// Override in child	
	}
	
	// Note: If some script enables a bunch of buttons at the same time they will all instantiate at once the first time
	public void Enable(){
		isActive = true;
	}
	
	public void Disable(){
		isActive = false;
	}
}
