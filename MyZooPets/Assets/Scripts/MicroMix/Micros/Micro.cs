using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class Micro : MonoBehaviour{
	private bool won = false;
	private bool playing = false;
	private int seconds = 0;
	private Dictionary<Transform, LgTuple<Vector3, bool>> positions = new Dictionary<Transform, LgTuple<Vector3, bool>>();

	protected abstract bool ResetPosition{
		get;
	}

	public abstract string Title{
		get;
	}

	public void SetWon(bool won){
		this.won = won;
	}

	public virtual void StartMicro(int difficulty){
		playing = true;
		StartCoroutine(EndMicro());
		positions.Clear();
		won = false;
		int x=0;
		foreach(Transform child in GetComponentsInChildren<Transform>(true)){
			if(child == transform){
				continue;
			}
			Debug.Log(child.gameObject.name);
			x++;
			positions.Add(child, new LgTuple<Vector3, bool>(child.position,child.gameObject.activeSelf));
			child.gameObject.SetActive(true);
			MicroItem mi = child.GetComponent<MicroItem>();
			if(mi != null){
				mi.StartItem();
			}
		}
		Debug.Log(x);
	}

	protected virtual void OnComplete(){
	}

	private IEnumerator EndMicro(){
		//Used for deactivating and closing off the micro, and alerting the manager
		for(seconds = 4; seconds > 0; seconds--){
			yield return new WaitForSeconds(1f);
		}
		int x=0;
		foreach(Transform child in positions.Keys){
			if(child == transform){
				continue;
			}
			x++;
			if (ResetPosition){
				Debug.Log("Called");
//				child.position = positions[child];
			}
			MicroItem mi = child.GetComponent<MicroItem>();
			if(mi != null){
				mi.OnComplete();
			}
			child.gameObject.SetActive(false);
		}
		Debug.Log(x);
		OnComplete();
		playing = false;

		if(won){ //This should always be called last
			MicroMixManager.Instance.WinMicro();
		}
		else{
			MicroMixManager.Instance.LoseMicro();
		}
	}

	void OnGUI(){
		if(!playing){
			return;
		}
		GUI.Box(new Rect(0, 0, 100, 100), seconds.ToString());
	}
}
