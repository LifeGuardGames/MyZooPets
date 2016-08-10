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

	public int renderQueueLevel = 5000;
	
	void Start () {
		ParticleSystem pSystem = gameObject.GetComponent<ParticleSystem>();
		pSystem.GetComponent<Renderer>().material.renderQueue = renderQueueLevel;
		
		ParticleSystem[] pSystemList = gameObject.GetComponentsInChildren<ParticleSystem>();
		foreach(ParticleSystem pSys in pSystemList){
			pSys.GetComponent<Renderer>().material.renderQueue = renderQueueLevel;
		}
	}
}
