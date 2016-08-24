using UnityEngine;
using System.Collections;

public class MoveMouthMicro : Micro{
	public GameObject petPrefab;
	public GameObject inhalerItem;
	private GameObject petInstance;

	public override string Title{
		get{
			return "Move to Mouth";
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
		inhalerItem.GetComponent<MoveMouthItem>().pet = petInstance;
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

		yield return finger.MoveTo(inhalerItem.transform.position, petInstance.GetComponent<MicroMixAnatomy>().mouth.transform.position, delay: .5f, time: 1f);

		finger.gameObject.SetActive(false);
	}

	private void Setup(){
		petInstance = (GameObject)Instantiate(petPrefab, Vector3.zero, Quaternion.identity);
		petInstance.transform.SetParent(transform);	
		do{
			inhalerItem.transform.position = CameraUtils.RandomWorldPointOnScreen(Camera.main, .2f, .2f, 0);
			petInstance.transform.position = CameraUtils.RandomWorldPointOnScreen(Camera.main, .33f, .33f, 0);
		} while (Vector3.Distance(inhalerItem.transform.position, petInstance.transform.position) < 1f);
	}
}
