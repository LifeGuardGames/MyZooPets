using UnityEngine;
using System.Collections;

/// <summary>
/// Particle plane.
/// This is used to render particle effects on its own plane.
/// (ie. NGUI only scene that needs some particle effects, this will use a separate camera to do so).
/// Add the effect as a child of the script, so we dont waste cycles instantiating it all the time
/// </summary>
public class ParticlePlane : Singleton<ParticlePlane>{
	
	public Camera camera;
	public GameObject particleObject;
	public float zDepth = 0.5f;
	
	public void PlayParticle(Vector3 screenPosition){
		
		// Move the object to the intended position
		Vector3 worldAux = camera.ScreenToWorldPoint(screenPosition);
		particleObject.transform.position = new Vector3(worldAux.x, worldAux.y, zDepth);
		
		// Play the particle system
		ParticleSystem pSystem = particleObject.GetComponent<ParticleSystem>();
		pSystem.Play();
	}
	
//	void OnGUI(){
//		if(GUI.Button (new Rect( 100, 100, 100, 100), "play")){
//			PlayParticle(new Vector3(400, 400, 0));
//		}
//	}
}
