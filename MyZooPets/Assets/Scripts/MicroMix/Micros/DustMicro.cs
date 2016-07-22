using UnityEngine;
using System.Collections;

public class DustMicro : Micro{
	private DustItem[] dustItems;
	private int count;

	public override string Title{
		get{
			return "Vacuum";
		}
	}

	public override int Background{
		get{
			return 1;
		}
	}

	protected override void _StartMicro(int difficulty){
		GetComponent<GestureRecognizer>().enabled = true;
		dustItems = GetComponentsInChildren<DustItem>(true);
		for(int i = 0; i < dustItems.Length; i++){
			dustItems[i].transform.position = CameraUtils.RandomWorldPointOnScreen(Camera.main, .05f, .05f);
		}
		count = dustItems.Length;
	}

	protected override void _EndMicro(){
		//Nothing to do here
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
		if(gesture.Selection == null || MicroMixManager.Instance.IsPaused || MicroMixManager.Instance.IsTutorial){
			return;
		}
		gesture.Selection.GetComponent<DustItem>().Drag();
	}

	void OnTap(TapGesture gesture){
		if(gesture.StartSelection == null || MicroMixManager.Instance.IsPaused || MicroMixManager.Instance.IsTutorial){
			return;
		}
		gesture.StartSelection.GetComponent<DustItem>().Tap();
	}
}
