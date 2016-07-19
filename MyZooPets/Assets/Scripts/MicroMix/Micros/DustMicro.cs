using UnityEngine;
using System.Collections;

public class DustMicro : Micro{
	public GameObject[] dustItems;
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
		GetComponent<GestureRecognizer>().enabled=true;
		for(int i = 0; i < dustItems.Length; i++){
			dustItems[i].transform.position= CameraUtils.RandomWorldPointOnScreen(Camera.main, .2f, .2f);
		}
		count=dustItems.Length;
	}

	protected override void _EndMicro(){
		//Nothing to do here
		GetComponent<GestureRecognizer>().enabled=false;

	}
	public void Cleaned(){ //Called when we clean up a single dust item
		count--;
		if (count==0){
			SetWon(true);
		}
	}
	void OnTap(TapGesture gesture){
		if(gesture.StartSelection == null){
			return;
		}
		gesture.StartSelection.GetComponent<DustItem>().Tap();
	}
}
