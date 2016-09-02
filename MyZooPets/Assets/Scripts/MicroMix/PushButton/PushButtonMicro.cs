using UnityEngine;
using System.Collections;

public class PushButtonMicro : Micro{
	public GameObject petPrefab;
	public GameObject inhaler;
	private GameObject petInstance;

	public override string Title{
		get{
			return "Push Button";
		}
	}

	public override int Background{
		get{
			return 0;
		}
	}

	protected override void _StartMicro(int difficulty, bool randomize){
		if(randomize){
			Setup();
		}
	}

	protected override void _EndMicro(){
		Destroy(petInstance);
	}

	protected override void _Pause(){
		petInstance.GetComponentInChildren<Animator>().enabled = false;
	}

	protected override void _Resume(){
		petInstance.GetComponentInChildren<Animator>().enabled = true;
	}

	protected override IEnumerator _Tutorial(){

		MicroMixFinger finger = MicroMixManager.Instance.finger;
		finger.gameObject.SetActive(true);

		Setup();
		PushButtonItem button = inhaler.GetComponentInChildren<PushButtonItem>();

		Vector3 offset = new Vector3(0, .5f);
		yield return finger.ShakeToBack(button.transform.position + offset, button.transform.position + button.animDelta + offset, delay: .5f, time: 1f);

		finger.gameObject.SetActive(false);
	}

	private void Setup(){
		petInstance = (GameObject)Instantiate(petPrefab, Vector3.zero, Quaternion.identity);
		petInstance.transform.SetParent(transform,true);
		petInstance.transform.position = inhaler.transform.position = CameraUtils.RandomWorldPointOnScreen(Camera.main, .4f, .33f, 0);
		inhaler.transform.position = petInstance.GetComponent<MicroMixAnatomy>().mouth.transform.position + new Vector3(.4f, -.2f);
		petInstance.GetComponentInChildren<Animator>().Play("Breathe Out", 0, 1);
		petInstance.GetComponentInChildren<Animator>().speed = 0;
	}
}
