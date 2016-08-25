using UnityEngine;
using System.Collections;

public class DustMicro : Micro{
	public GameObject vacuum;
	private DustItem[] dustItems;
	private int count;
	private float lastDragTime;
	private float intervalDragTime = .2f;
	//How long in seconds between each dust we make dissappear
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

	protected override void _StartMicro(int difficulty, bool randomize){
		Setup(randomize);
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
		for(int i = 0; i < dustItems.Length; i++){
			dustItems[i].Randomize();
		}
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
		if (MicroMixManager.Instance.IsPaused || MicroMixManager.Instance.IsTutorial){
			return;
		}
		vacuum.transform.position = CameraUtils.ScreenToWorldPointZ(Camera.main,gesture.Position,0);
		if(gesture.Selection == null){
			return;
		}
		if(lastDragTime < gesture.StartTime){
			lastDragTime = gesture.StartTime;
		}
		if(Time.time - lastDragTime > intervalDragTime){
			lastDragTime = Time.time;
			gesture.Selection.GetComponent<DustItem>().Drag();
		}
	}


	void OnTap(TapGesture gesture){
		if (MicroMixManager.Instance.IsPaused || MicroMixManager.Instance.IsTutorial){
			return;
		}
		vacuum.transform.position = CameraUtils.ScreenToWorldPointZ(Camera.main,gesture.Position,0);
		if(gesture.Selection == null){
			return;
		}
		gesture.StartSelection.GetComponent<DustItem>().Tap();
	}

	private void Setup(bool randomize){
		dustItems = GetComponentsInChildren<DustItem>(true);
		if(randomize){
			for(int i = 0; i < dustItems.Length; i++){
				dustItems[i].transform.position = CameraUtils.RandomWorldPointOnScreen(Camera.main, .2f, .2f, 0);
				dustItems[i].gameObject.SetActive(true);
				dustItems[i].Randomize();
			}
		}
		count = dustItems.Length;
	}

}
