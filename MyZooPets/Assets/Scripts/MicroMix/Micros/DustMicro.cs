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
		Setup();
	}

	protected override void _EndMicro(){
	}

	protected override IEnumerator _Tutorial(){
		Setup();
		MicroMixFinger finger = MicroMixManager.Instance.finger;
		finger.gameObject.SetActive(true);

		for(int i = 0; i < dustItems.Length; i++){ //Now point them all towards the trash
			yield return finger.ShakeToBack(dustItems[i].transform.position + Vector3.down, dustItems[i].transform.position + Vector3.up, .1f, .3f);
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
		if(gesture.Selection == null || MicroMixManager.Instance.IsPaused || MicroMixManager.Instance.IsTutorial){
			return;
		}
		gesture.Selection.GetComponent<DustItem>().Drag();
	}

	private void Setup(){
		dustItems = GetComponentsInChildren<DustItem>(true);
		for(int i = 0; i < dustItems.Length; i++){
			dustItems[i].transform.position = CameraUtils.RandomWorldPointOnScreen(Camera.main, .05f, .05f);
		}
		count = dustItems.Length;
	}

	void OnTap(TapGesture gesture){
		if(gesture.StartSelection == null || MicroMixManager.Instance.IsPaused || MicroMixManager.Instance.IsTutorial){
			return;
		}
		gesture.StartSelection.GetComponent<DustItem>().Tap();
	}
}
