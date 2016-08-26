using UnityEngine;
using System.Collections;

public class MoldMicro : Micro{
	public GameObject trashItem;
	private MoldItem[] moldItems;
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

	protected override void _StartMicro(int difficulty, bool randomize){
		Setup(randomize);
		count = moldItems.Length;
		startTime = Time.time;
	}

	protected override void _EndMicro(){
	}

	protected override void _Pause(){
	}

	protected override void _Resume(){
	}

	protected override IEnumerator _Tutorial(){
		Setup(true);
		MicroMixFinger finger = MicroMixManager.Instance.finger;
		finger.gameObject.SetActive(true);
		foreach(MoldItem mold in moldItems){
			yield return finger.MoveTo(mold.transform.position, trashItem.transform.position, .2f, .4f);
		}
		finger.gameObject.SetActive(false);
	}

	public void Cleaned(){ //Called when we clean up a single dust item
		count--;
		if(count == 0){
			SetWon(true);
		}
	}

	void OnDrag(DragGesture gesture){
		if(gesture.StartSelection == null || gesture.StartTime < startTime || MicroMixManager.Instance.IsPaused || MicroMixManager.Instance.IsTutorial){ //If the gesture is older than the minigame, we have been holding over
			return;
		}

		Vector3 currentPos = CameraUtils.ScreenToWorldPointZ(Camera.main, gesture.Position, 0);
		gesture.StartSelection.transform.position = Vector3.MoveTowards(gesture.StartSelection.transform.position, currentPos, moldSpeed);
	}

	private void Setup(bool randomize){
		moldItems = GetComponentsInChildren<MoldItem>(true);
		if(randomize){
			for(int i = 0; i < moldItems.Length; i++){
				Vector3 spawnPos;
				do{
					spawnPos = CameraUtils.RandomWorldPointOnScreen(Camera.main, .2f, .2f, 0);
				} while (Vector3.Distance(spawnPos, trashItem.transform.position) < 3f); //Don't let them spawn right at the center;
				moldItems[i].transform.position = spawnPos;
				moldItems[i].gameObject.SetActive(true); //They are set inactive by our script, so this will bring them back
			}
		}
	}

}
