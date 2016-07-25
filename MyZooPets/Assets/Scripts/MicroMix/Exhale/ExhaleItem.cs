using UnityEngine;
using System.Collections;

public class ExhaleItem : MicroItem{
	private bool complete = false;

	public override void StartItem(){
		GetComponent<ScreenRaycaster>().Cameras = new Camera[1] { Camera.main };
	}

	void OnDrag(DragGesture gesture){
		if(gesture.StartSelection == null || MicroMixManager.Instance.IsPaused || MicroMixManager.Instance.IsTutorial){
			return;
		}
		Vector3 startPos = CameraUtils.ScreenToWorldPointZero(Camera.main, gesture.StartPosition);
		Vector3 currentPos = CameraUtils.ScreenToWorldPointZero(Camera.main, gesture.Position);
		Vector3 deltaPos = currentPos - startPos;
		if(deltaPos.x > 2 && gesture.StartSelection.Equals(gameObject)){
			GetComponentInParent<Animator>().SetTrigger("BreatheOut");
			complete = true;
			parent.SetWon(true);
		}
	}
}
