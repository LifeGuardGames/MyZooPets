using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class Micro : MonoBehaviour{
	private bool won = false;
	private bool playing = false;
	private int seconds = 0;
	//private Dictionary<Transform, LgTuple<Vector3, bool>> positions = new Dictionary<Transform, LgTuple<Vector3, bool>>();
	private Dictionary<Transform, Vector3> positions = new Dictionary<Transform, Vector3>();

	public abstract string Title{
		get;
	}

	public void SetWon(bool won){
		this.won = won;
	}

	public virtual void StartMicro(int difficulty){
		playing = true;
		StartCoroutine(EndMicro());
		won = false;
		foreach(Transform child in transform){ //Turn on all parents
			child.gameObject.SetActive(true);
		}
		foreach(Transform child in positions.Keys){ //Tell everyone, no matter how many layers in, we are starting
			MicroItem mi = child.GetComponent<MicroItem>();
			if(mi != null){
				mi.StartItem();
			}
		}
	}

	protected virtual void OnComplete(){
	}

	private IEnumerator EndMicro(){
		//Used for deactivating and closing off the micro, and alerting the manager
		for(seconds = 4; seconds > 0; seconds--){
			yield return new WaitForSeconds(1f);
		}
		foreach(Transform child in transform){ //Turn of all parents
			child.gameObject.SetActive(false);
		}
		foreach(Transform child in positions.Keys){ //Here is where we need the transforms
			MicroItem mi = child.GetComponent<MicroItem>();
			if(mi != null){
				mi.OnComplete();
			}
			child.transform.position = positions[child];

		}
		OnComplete();
		playing = false;

		if(won){ //This should always be called last
			MicroMixManager.Instance.WinMicro();
		}
		else{
			MicroMixManager.Instance.LoseMicro();
		}
	}

	void Awake(){
		foreach(Transform child in GetComponentsInChildren<Transform>(true)){
			if(child == transform){
				continue;
			}
			Renderer rend = child.GetComponent<Renderer>();
			positions.Add(child, child.transform.position);
		}
		foreach(Transform child in transform){
			child.gameObject.SetActive(false);
		}
	}

	private IEnumerator WaitThenHide(){
		yield return new WaitForSeconds(.1f);

	}

	void OnGUI(){
		if(!playing){
			return;
		}
		GUI.Box(new Rect(0, 0, 100, 100), seconds.ToString());
	}
}
