using UnityEngine;
using System.Collections;

public class ExhaleMicro : Micro{
	public GameObject petPrefab;
	private GameObject petInstance;
	private GameObject neckObject;
	private bool complete;

	public override string Title{
		get{
			return "Exhale";
		}
	}

	public override int Background{
		get{
			return 0;
		}
	}
	// Use this for initialization
	protected override void _StartMicro(int difficulty, bool randomize){
		complete = false;
		if(randomize){
			petInstance = (GameObject)Instantiate(petPrefab, Vector3.zero, Quaternion.identity);
			petInstance.transform.SetParent(transform);
			petInstance.transform.position = CameraUtils.RandomWorldPointOnScreen(Camera.main, .2f, .2f, 50);
		}
		neckObject = petInstance.GetComponent<MicroMixAnatomy>().neck;
	}

	protected override void _EndMicro(){
		Destroy(petInstance);
	}


	protected override void _Pause(){
		petInstance.GetComponentInChildren<Animator>().enabled=false;
	}

	protected override void _Resume(){
		petInstance.GetComponentInChildren<Animator>().enabled=true;
	}

	protected override IEnumerator _Tutorial(){
		petInstance = (GameObject)Instantiate(petPrefab, Vector3.zero, Quaternion.identity);
		petInstance.transform.SetParent(transform);

		MicroMixFinger finger = MicroMixManager.Instance.finger;
		finger.gameObject.SetActive(true);
		Vector3 mouthPos = petInstance.GetComponent<MicroMixAnatomy>().mouth.transform.position;
		yield return finger.MoveTo(mouthPos, mouthPos + Vector3.right * 5, delay: .5f, time: 1f);

		finger.gameObject.SetActive(false);
		complete = false;
	}

	void OnDrag(DragGesture gesture){
		if(gesture.StartSelection != neckObject || MicroMixManager.Instance.IsPaused || MicroMixManager.Instance.IsTutorial || complete){
			return;
		}
		Vector3 startPos = CameraUtils.ScreenToWorldPointZero(Camera.main, gesture.StartPosition, 50);
		Vector3 currentPos = CameraUtils.ScreenToWorldPointZero(Camera.main, gesture.Position, 50);
		Vector3 deltaPos = currentPos - startPos;
		if(deltaPos.x > 2){
			petInstance.GetComponentInChildren<Animator>().SetTrigger("BreatheOut");
			complete = true;
			SetWon(true);
		}
	}
}
