using UnityEngine;

public class InhaleItem : MicroItem{
	public GameObject petInstance;
	private GameObject headObject;
	private bool complete = false;
	//How far we move during tween
	private Vector3 exhaleDelta = new Vector3(.2f, -.3f);

	public override void StartItem(){
		complete = false;
		headObject = petInstance.GetComponent<MicroMixAnatomy>().neck;
	}

	public override void OnComplete(){
		LeanTween.cancel(gameObject, false);
	}

	void OnDrag(DragGesture gesture){
		if(gesture.StartSelection != gameObject || gesture.Selection != headObject || MicroMixManager.Instance.IsPaused || MicroMixManager.Instance.IsTutorial || complete){
			return;
		}
		petInstance.GetComponentInChildren<Animator>().SetTrigger("BreatheIn");
		complete = true;
		parent.SetWon(true);
		//LeanTween.move(gameObject, transform.position + inhaleDelta, 1f).setEase(LeanTweenType.easeOutQuad).setOnComplete(MoveAwayTween);
	}

	void Update(){
		if(complete && petInstance){
			transform.position = petInstance.GetComponent<MicroMixAnatomy>().mouth.transform.position + new Vector3(-.2f, .2f);	
		}
	}

	private void MoveAwayTween(){
		LeanTween.move(gameObject, transform.position + exhaleDelta, 1f).setEase(LeanTweenType.easeInOutQuad);
	}
}
