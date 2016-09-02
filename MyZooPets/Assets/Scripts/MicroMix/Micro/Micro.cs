﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/* MicroMixManager calls StartMicro on this, which calls _StartMicro and then afterward sets up the MicroItems and finally calls Timer
 * WaitTimer waits for four seconds, then call EndMicro, which closes up all the items, and then calls _EndMicro
 * Individual micros that override this must hanlde _StartMicro and _EndMicro, as well as instantiating any objects they need
 * This class will handle resetting positions, setting MicroItem.parent and calling StartItem and EndItem
 * Though the individual MicroItem's parent field is public and can be set manually in the inspector, this overwrites that setting
 */
public abstract class Micro : MonoBehaviour{
	protected abstract void _Pause();

	protected abstract void _Resume();

	protected abstract void _StartMicro(int difficulty, bool randomize);

	protected abstract void _EndMicro();

	protected abstract IEnumerator _Tutorial();

	private bool won = false;
	private bool playing = false;
	private int seconds = 0;
	private Dictionary<Transform, Vector3> positions = new Dictionary<Transform, Vector3>();
	private MicroMixBossTimer timer;

	public abstract string Title{
		get;
	}

	public abstract int Background{
		get;
	}

	protected virtual bool ResetPositions{
		get{
			return true;
		}
	}

	public void SetWon(bool won){
		this.won = won;
		if(won){
			MicroMixManager.Instance.fireworksController.StartFireworks();
		}
	}

	public void StartTutorial(){
		StartCoroutine(Tutorial());
	}

	public void StartMicro(int difficulty, bool randomize){
		_StartMicro(difficulty, randomize); //Have them instantiate everything they need, and then we handle setup for them
		won = false;
		playing = true; //Now we set up our own stuff
		if(!timer){
			timer = FindObjectOfType<MicroMixBossTimer>();
		}
		timer.gameObject.SetActive(true);
		timer.StartTimer(this);
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

	}

	public void Pause(){
		_Pause();
		timer.Pause();
	}

	public void Resume(){
		_Resume();
		timer.Resume();
	}

	private IEnumerator Tutorial(){
		yield return StartCoroutine(_Tutorial());
		//They just completed the tutorial
		yield return 0; //Wait for them to destroy their objects
		MicroMixManager.Instance.CompleteTutorial();
	}

	public void EndMicro(){
		foreach(Transform child in positions.Keys){ //Here is where we need the transforms
			MicroItem mi = child.GetComponent<MicroItem>();
			if(mi != null){
				mi.OnComplete();
			}
			if(ResetPositions){
				child.transform.position = positions[child];
			}
		}
		playing = false;


		_EndMicro(); //We have cleaned everything up for them, let them handle the rest

		timer.gameObject.SetActive(false);
		MicroMixManager.Instance.fireworksController.StopFireworks();

		if(won){ //This should always be called last
			MicroMixManager.Instance.WinMicro();
		}
		else{
			MicroMixManager.Instance.LoseMicro();
		}
	}
}
