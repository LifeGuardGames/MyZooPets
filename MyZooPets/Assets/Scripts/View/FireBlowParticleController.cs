using UnityEngine;
using System.Collections;

/// <summary>
/// Fire blow particle controller.
/// Note: The stream has some problems when scaling the parents (rotation doesnt scale...)
/// </summary>
public class FireBlowParticleController : ParticleSystemController {

	public ParticleSystem pSystem2;	// This is the fireball, primary is the stream
	public ParticleLocationBurstList hitEffect;
	public float secondaryDelay;	// Number of seconds to wait before the secondary starts playing
	public bool isStartSimultaneous;// Starts both particles simultaneously or wait till delay
	public bool isStopSimultaneous;	// Stops both particles simultaneously or wait till delay
	
	private float fireballSize = 0.3f;	// Size of the fireball, range: [0.1 ~ 0.4] is ideal
	public float FireballSize{
		get{
			return fireballSize;
		} 
		set{
			fireballSize = value;
			if(pSystem2 != null){
				pSystem2.startLifetime = fireballSize; // Change the "size" of the fireball particle
			}
			else{
				Debug.Log("No fireball found");
			}
		}
	}
	
	protected override void _Start(){
	}
	
	protected override void _Play(){
		float delay = isStartSimultaneous ? 0f : secondaryDelay;
		StartCoroutine(PlaySecondParticle(delay));
	}
	
	IEnumerator PlaySecondParticle(float delay){
		yield return new WaitForSeconds(delay);
		if(pSystem2 != null){
			pSystem2.Play();
			if(hitEffect != null) {
				hitEffect.Play();
			}
		}
	}
	
	protected override void _Stop(){
		float delay = isStopSimultaneous ? 0f : secondaryDelay;
		StartCoroutine(StopSecondParticle(delay));
	}
	
	IEnumerator StopSecondParticle(float delay){
		yield return new WaitForSeconds(delay);
		if(pSystem2 != null){
			pSystem2.Stop();
		}
	}
	
//	 void OnGUI(){
//	 	if(GUI.Button(new Rect(100, 100, 100, 100), "Start")){
//	 		Play();
//	 	}
//	 	if(GUI.Button(new Rect(300, 100, 100, 100), "Stop")){
//	 		Stop();
//	 	}
//	 }
}
