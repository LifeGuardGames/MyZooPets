using UnityEngine;
using System.Collections;

public class MoldMicro : Micro{
	public GameObject[] moldItems;
	private int count;
	private float moldSpeed = .8f;
	private float startTime;
	//Time which this minigame started;
	//Pretty fast, but not quite as fast as the gesture
	public override string Title{
		get{
			return "Toss";
		}
	}

	public override int Background{
		get{
			return 1;
		}
	}

	protected override void _StartMicro(int difficulty){
		GetComponent<GestureRecognizer>().enabled = true;

		for(int i = 0; i < moldItems.Length; i++){
			Vector3 spawnPos;
			do{
				spawnPos = CameraUtils.RandomWorldPointOnScreen(Camera.main, .2f, .2f);
			} while (Vector3.Distance(spawnPos, Vector3.zero) < 2f); //Don't let them spawn right at the center;
			moldItems[i].transform.position = spawnPos;
		}
		count = moldItems.Length;
		startTime = Time.time;
	}

	protected override void _EndMicro(){
		GetComponent<GestureRecognizer>().enabled = false;
	}

	protected override IEnumerator _Tutorial(){
		yield return 0;
	}

	public void Cleaned(){ //Called when we clean up a single dust item
		count--;
		if(count == 0){
			SetWon(true);
		}
	}

	void OnDrag(DragGesture gesture){
		if(gesture.StartSelection == null || gesture.StartTime < startTime || MicroMixManager.Instance.IsPaused  || IsTutorial){ //If the gesture is older than the minigame, we have been holding over
			return;
		}

		Vector3 currentPos = CameraUtils.ScreenToWorldPointZero(Camera.main, gesture.Position);
		gesture.StartSelection.transform.position = Vector3.MoveTowards(gesture.StartSelection.transform.position, currentPos, moldSpeed);
	}

}
