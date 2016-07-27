using UnityEngine;
using System.Collections;

public class FireMicro : Micro{
	public GameObject mover;
	private float moveTime = .8f;
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

	public void StartMover(){
		TweenUp();
	}

	public void StopMover(){
		LeanTween.cancel(mover);
	}

	protected override void _StartMicro(int difficulty, bool randomize){
		type = LeanTweenType.easeInOutSine;
	}

	protected override void _EndMicro(){
	}

	protected override IEnumerator _Tutorial(){
		Vector3 moverPos = mover.transform.position;
		MicroMixFinger finger = MicroMixManager.Instance.finger;
		finger.gameObject.SetActive(true);
		Vector3 button = GetComponentInChildren<FireItem>().gameObject.transform.position;
		Vector3 offset = new Vector3(0, 1);
		yield return finger.MoveTo(button + offset, button, delay: .3f, time: .3f);

		TweenUp();
		yield return WaitSecondsPause(1.08f);
		LeanTween.cancel(mover);

		yield return finger.MoveTo(button, button + offset, delay: 0, time: .3f);
		finger.gameObject.SetActive(false);
		mover.transform.position = moverPos;
	}

	protected override void _Pause(){
		LeanTween.pause(mover);
	}

	protected override void _Resume(){
		LeanTween.resume(mover);
	}

	private void TweenUp(){
		LeanTween.move(mover, new Vector3(0, 2.45f), moveTime).setEase(type).setOnComplete(TweenBack);
	}

	private void TweenBack(){
		LeanTween.move(mover, new Vector3(0, -2.45f), moveTime).setEase(type).setOnComplete(TweenUp);
	}
}
