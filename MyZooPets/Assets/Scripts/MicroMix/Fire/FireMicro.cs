using UnityEngine;
using System.Collections;

public class FireMicro : Micro{
	public RectTransform innerBar;
	private float moveTime = .8f;
	private float minSize = -345f;
	private float maxSize = -4.3f;
	private LeanTweenType type;
	// Use this for initialization
	public override int Background{
		get{
			return 3;
		}
	}

	public override string Title{
		get{
			return "Blow Fire";
		}
	}

	public void StartBar(){
		TweenUp();
	}

	public void StopBar(){
		LeanTween.cancel(innerBar.gameObject);
	}

	public bool IsCorrect(){ //If you want to change the range, then make sure to change the UI elements too.
		return innerBar.offsetMax.x > -85f;
	}

	protected override void _StartMicro(int difficulty, bool randomize){
		if(randomize){
			Setup();
		}
	}

	protected override void _EndMicro(){
		StopBar();
	}

	protected override IEnumerator _Tutorial(){
		Setup();
		MicroMixFinger finger = MicroMixManager.Instance.finger;
		finger.gameObject.SetActive(true);
		FireItem fireItem = GetComponentInChildren<FireItem>();
		fireItem.GetComponent<Animation>().Play();
		Vector3 button = fireItem.gameObject.transform.position;
		Vector3 offset = new Vector3(0, 1);
		yield return finger.MoveTo(button + offset, button, delay: .3f, time: .3f);

		fireItem.Engage();
		yield return MicroMixManager.Instance.WaitSecondsPause(.72f);
		fireItem.Disengage();

		yield return finger.MoveTo(button, button + offset, delay: 0, time: .3f);
		finger.gameObject.SetActive(false);

		UpdateSize(minSize);
	}

	protected override void _Pause(){
		LeanTween.pause(innerBar.gameObject);
	}

	protected override void _Resume(){
		LeanTween.resume(innerBar.gameObject);
	}

	private void Setup(){
		type = LeanTweenType.linear;
		UpdateSize(minSize);
	}

	private void TweenUp(){
		LeanTween.value(innerBar.gameObject, minSize, maxSize, moveTime).setEase(type).setOnUpdate(UpdateSize).setOnComplete(TweenBack);
	}

	private void TweenBack(){
		LeanTween.value(innerBar.gameObject, maxSize, minSize, moveTime).setEase(type).setOnUpdate(UpdateSize).setOnComplete(TweenUp);
	}

	private void UpdateSize(float newSize){
		innerBar.offsetMax = new Vector2(newSize, innerBar.offsetMax.y);
	}
}
