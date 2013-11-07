using UnityEngine;
using System.Collections;

public class RandomLightFlicker : MonoBehaviour {

	public float minFlickerSpeed = 0.01f;
	public float maxFlickerSpeed = 0.1f;
	public float minLightIntensity = 0f;
	public float maxLightIntensity = 1f;
	
	public bool flashingFlicker = true;
	
	private Light thisLight;
	private bool loop = true;
	
	void Start(){
		thisLight = gameObject.GetComponent<Light>();
		StartCoroutine ("doFlicker");
	}
	
	IEnumerator doFlicker(){
		if(flashingFlicker){
			thisLight.enabled = true;
		}
		thisLight.intensity = Random.Range(minLightIntensity, maxLightIntensity);
	    yield return new WaitForSeconds(Random.Range(minFlickerSpeed, maxFlickerSpeed));
	    
		if(flashingFlicker){
			thisLight.enabled = false;
		}
	    yield return new WaitForSeconds(Random.Range(minFlickerSpeed, maxFlickerSpeed));
		StartCoroutine ("doFlicker");
	}
}
