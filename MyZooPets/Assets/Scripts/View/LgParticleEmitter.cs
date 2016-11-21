using UnityEngine;
using System.Collections;

/// <summary>
/// Lg particle emitter.
/// Emits particles at random between the two constants: min and max interval
/// Just instantiates it, the behaviour of the particle is up to itself
/// Note: Requires NGUITools for add child
/// </summary>
public class LgParticleEmitter : MonoBehaviour{
	
	public GameObject particleObject;
	
	//Timer stuff
	public bool isActive;

	private void SetActive(bool bActive){
		isActive = bActive;	
	}
	
	public float delayBeforeStart;
	public float minInterval;
	public float maxInterval;
	private float generatedValue;
	private float timeBegin;
	
	protected virtual void Start(){
		if(maxInterval < minInterval){
			Debug.LogError("Max interval is less than min interval, clamping to min");
			maxInterval = minInterval;
		}
		generatedValue = Random.Range(minInterval, maxInterval);
		timeBegin = Time.time;
	}
	
	protected virtual void OnDestroy(){
	}
	
	void Update(){
		if(isActive){	
			if(Time.time > timeBegin + generatedValue){
				timeBegin = Time.time;
				generatedValue = Random.Range(minInterval, maxInterval);
				
				// Do the action here
				GameObject emittedObject = GameObjectUtils.AddChild(gameObject, particleObject);
				ExtendedAction(emittedObject);
			}
		}
	}

	// Override in child	
	protected virtual void ExtendedAction(GameObject emittedObject){

	}
	
	// Note: If some script enables a bunch of buttons at the same time they will all instantiate at once the first time
	public void Enable(bool isImmediate = false){
		SetActive(true);
		
		// if we are to fire this trigger immediately, override the generated value
		if(isImmediate)
			generatedValue = 0;
		
		OnEnabled(isImmediate);
	}
	
	protected virtual void OnEnabled(bool isImmediate){
		
	}
	
	public void Disable(){
		SetActive(false);
		
		OnDisabled();
	}
	
	protected virtual void OnDisabled(){
	}
}
