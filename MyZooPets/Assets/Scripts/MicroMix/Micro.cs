using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/* MicroMixManager calls StartMicro on this, which calls _StartMicro and then afterward sets up the MicroItems and finally calls Timer
 * TimeMicro waits for four seconds, then call EndMicro, which closes up all the items, and then calls _EndMicro
 * Individual micros that override this must hanlde _StartMicro and _EndMicro, as well as instantiating any objects they need
 * This class will handle resetting positions, setting MicroItem.parent and calling StartItem and EndItem
 * Though the individual MicroItem.parent is public and can be set manually in the inspector, this overwrites that setting
 */
public abstract class Micro : MonoBehaviour{
	protected abstract void _StartMicro(int difficulty, bool randomize);

	protected abstract void _EndMicro();

	protected abstract IEnumerator _Tutorial();

	private bool won = false;
	private bool playing = false;
	private int seconds = 0;
	//private Dictionary<Transform, LgTuple<Vector3, bool>> positions = new Dictionary<Transform, LgTuple<Vector3, bool>>();
	private Dictionary<Transform, Vector3> positions = new Dictionary<Transform, Vector3>();

	public abstract string Title{
		get;
	}

	public abstract int Background{
		get;
	}

	public void SetWon(bool won){
		this.won = won;
	}

	public void StartMicro(int difficulty, bool randomize=true){
		won = false;
		MicroMixManager.Instance.IsTutorial = false;
		if(!DataManager.Instance.GameData.MicroMix.MicrosCompleted.Contains(Title)){
			StartCoroutine(Tutorial(difficulty));
			return; //Do not continue on
		}
		else{
			_StartMicro(difficulty, randomize); //Have them instantiate everything they need, and then we handle setup for them
		}
		playing = true; //Now we set up our own stuff
		positions.Clear();
		foreach(Transform child in GetComponentsInChildren<Transform>(true)){ //And set up all the MicroItems
			if(child == transform){
				continue;
			}
			positions.Add(child, child.transform.position);
			MicroItem mi = child.GetComponent<MicroItem>();
			if(mi != null){
				mi.StartItem();
				mi.SetParent(this);
			}
		}
		foreach(Transform child in transform){
			//child.gameObject.SetActive(true);
		}

		StartCoroutine(WaitTimer());

	}

	private IEnumerator Tutorial(int difficulty){
		MicroMixManager.Instance.IsTutorial = true;
		yield return StartCoroutine(_Tutorial());
		DataManager.Instance.GameData.MicroMix.MicrosCompleted.Add(Title);
		yield return 0; //Wait for them to destroy their objects
		StartMicro(difficulty, false); //This is only called after we have told everyone who our parent is. We should return after this is called ABOVE
		//and then go back. Or maybe yield?
	}

	private void EndMicro(){
		foreach(Transform child in positions.Keys){ //Here is where we need the transforms
			MicroItem mi = child.GetComponent<MicroItem>();
			if(mi != null){
				mi.OnComplete();
			}
			child.transform.position = positions[child];
		}
		foreach(Transform child in transform){ //Turn of all parents
			//child.gameObject.SetActive(false);
		}
		playing = false;

		_EndMicro(); //We have cleaned everything up for them, let them handle the rest
	}

	private IEnumerator WaitTimer(){
		//Used for deactivating and closing off the micro, and alerting the manager
		for(seconds = 4; seconds > 0; seconds--){
			yield return WaitSecondsPause(1f);
		}
		EndMicro();
		yield return 0; //Give everything that needs to be destroyed a second...
		if(won){ //This should always be called last
			MicroMixManager.Instance.WinMicro();
		}
		else{
			MicroMixManager.Instance.LoseMicro();
		}

	}

	private IEnumerator WaitThenHide(){
		yield return new WaitForSeconds(.1f);

	}

	protected IEnumerator WaitSecondsPause(float time){ //Like wait for seconds, but pauses w/ MicroMixManager
		for(float i = 0; i <= time; i += .1f){
			yield return new WaitForSeconds(.1f);
			while(MicroMixManager.Instance.IsPaused){
				yield return new WaitForEndOfFrame();
			}
		}
	}

	void OnGUI(){
		if(!playing){
			return;
		}
		GUI.Box(new Rect(0, 0, 100, 100), seconds.ToString());
	}
}
