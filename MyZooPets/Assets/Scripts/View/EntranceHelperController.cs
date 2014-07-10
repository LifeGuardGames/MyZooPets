using UnityEngine;
using System.Collections;

/// <summary>
/// Entrance helper controller.
/// This is the controller script for newly spawned entrances and objects (typically by unlocking something)
/// </summary>
public class EntranceHelperController : MonoBehaviour {
	public GameObject arrowGameObject;
	public float arrowShowDelay = 1f;
	public ParticleSystem spawnParticle;

	void Start(){
		spawnParticle.Play();
		arrowGameObject.SetActive(false);
		Invoke("ShowArrow", arrowShowDelay);
	}

	public void ShowArrow(){
		arrowGameObject.SetActive(true);
	}
}
