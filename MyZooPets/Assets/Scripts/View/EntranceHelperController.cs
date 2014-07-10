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
	public string entranceKey; //this key is important

	void Awake(){
		if(string.IsNullOrEmpty(entranceKey)){
			Debug.LogError("Entrance key at " + this.gameObject + " needs to be set");
		}
	}

	void Start(){
		bool isFirstTime = DataManager.Instance.GameData.FirstTimeEntrance.IsFirstTimeEntrance(entranceKey);

		if(isFirstTime){
			spawnParticle.Play();
			arrowGameObject.SetActive(false);
			Invoke("ShowArrow", arrowShowDelay);
		}
		else{
			arrowGameObject.SetActive(false);
		}
	}

	public void ShowArrow(){
		arrowGameObject.SetActive(true);
	}

	public void EntranceUsed(){
		DataManager.Instance.GameData.FirstTimeEntrance.EntranceUsed(entranceKey);
	}
}
