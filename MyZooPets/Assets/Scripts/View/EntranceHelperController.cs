using UnityEngine;
using System.Collections;
using System;

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
		GatingManager.OnDestroyedGate += OpenTheGates;
	}

	void Start(){
		RefreshState();

		if(MiniPetHUDUIManager.Instance) {
			MiniPetHUDUIManager.Instance.OnManagerOpen += OnManagerOpenEventHandler;
		}
	}

	void OnDestroy() {
		if(MiniPetHUDUIManager.Instance){
			MiniPetHUDUIManager.Instance.OnManagerOpen -= OnManagerOpenEventHandler;
		}
		GatingManager.OnDestroyedGate -= OpenTheGates;
	}

	private void RefreshState(){
		//bool isFirstTime = DataManager.Instance.GameData.FirstTimeEntrance.IsFirstTimeEntrance(entranceKey);
		
		//if(isFirstTime){

			spawnParticle.Play();
			arrowGameObject.SetActive(false);
			Invoke("ShowArrow", arrowShowDelay);
		//}
	//	else{
	//		arrowGameObject.SetActive(false);
	//	}
	}
	public void OpenTheGates(object sender, EventArgs args){
		this.gameObject.transform.parent.gameObject.SetActive(true);
		spawnParticle.Play();
	}

	public void ShowArrow(){
		//arrowGameObject.SetActive(true);
	}
	
	public void EntranceUsed(){
		DataManager.Instance.GameData.FirstTimeEntrance.EntranceUsed(entranceKey);
		RefreshState();
	}

	/// <summary>
	/// Disable arrow hints for entrance when zoom into mini pet. 
	/// </summary>
	/// <param name="sender">Sender.</param>
	/// <param name="args">Arguments.</param>
	private void OnManagerOpenEventHandler(object sender, UIManagerEventArgs args){
		if(args.Opening){
			arrowGameObject.SetActive(false);
		}
		else{
			bool isFirstTime = DataManager.Instance.GameData.FirstTimeEntrance.IsFirstTimeEntrance(entranceKey);
			if(isFirstTime){
				ShowArrow();
			}
		}
	}
}
