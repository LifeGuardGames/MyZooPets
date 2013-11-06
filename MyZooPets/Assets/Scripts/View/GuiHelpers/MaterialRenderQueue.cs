using UnityEngine;
using System.Collections;

/// <summary>
/// Material render queue.
/// Used for particle effects.
/// Applies the render queue order to itself and all of its children.
/// 
/// NOTE: This is set on the material afterwards!
/// 
/// </summary>
public class MaterialRenderQueue : MonoBehaviour {

	public int renderQueueLevel;
	
	void Start () {
		ParticleSystem pSystem = gameObject.GetComponent<ParticleSystem>();
		pSystem.renderer.material.renderQueue = 5000;
		
		ParticleSystem[] pSystemList = gameObject.GetComponentsInChildren<ParticleSystem>();
		foreach(ParticleSystem pSys in pSystemList){
			pSys.renderer.material.renderQueue = 5000;
		}
	}
}
