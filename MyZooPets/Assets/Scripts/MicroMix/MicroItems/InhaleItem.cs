using UnityEngine;
using System.Collections;
using UnityEditor;

public class InhaleItem : MicroItem{
	public Micro parent;
	public GameObject inhaler;
	private bool complete = false;
	private Vector3 inhaleDelta = new Vector3(-.4f, .4f);
	//How far we move during tween
	private Vector3 exhaleDelta = new Vector3(.2f, -.3f);

	public override void StartItem(){
		GetComponent<ScreenRaycaster>().Cameras = new Camera[1] { Camera.main };
	}

	public override void OnComplete(){
		LeanTween.cancel(inhaler,false);
	}

	void OnDrag(DragGesture gesture){
		if(gesture.StartSelection == null || gesture.Selection == null){
			return;
		}
		if(gesture.StartSelection.Equals(inhaler) && gesture.Selection.Equals(gameObject) && !complete){
			GetComponentInParent<Animator>().SetTrigger("BreatheIn");
			complete = true;
			parent.SetWon(true);
			LeanTween.move(inhaler, inhaler.transform.position + inhaleDelta, 1f).setEase(LeanTweenType.easeOutQuad).setOnComplete(MoveAwayTween);
		}
	}

	private void MoveAwayTween(){
		LeanTween.move(inhaler, inhaler.transform.position + exhaleDelta, 1f).setEase(LeanTweenType.easeInOutQuad);
	}
}
