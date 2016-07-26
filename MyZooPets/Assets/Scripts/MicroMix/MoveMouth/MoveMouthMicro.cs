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
			petInstance = (GameObject)Instantiate(petPrefab, Vector3.zero, Quaternion.identity);
			petInstance.transform.SetParent(transform);	
			do{
				inhalerItem.transform.position = CameraUtils.RandomWorldPointOnScreen(Camera.main, .1f, .1f);
				petInstance.transform.position = CameraUtils.RandomWorldPointOnScreen(Camera.main, .2f, .2f);
			} while (Vector3.Distance(inhalerItem.transform.position, petInstance.transform.position) < 2f);
		}
		inhalerItem.GetComponent<MoveMouthItem>().pet = petInstance;
	}

	protected override void _EndMicro(){
		Destroy(petInstance);
	}

	protected override void _Pause(){
	}

	protected override void _Resume(){
	}

	protected override IEnumerator _Tutorial(){
		MicroMixFinger finger = MicroMixManager.Instance.finger;
		finger.gameObject.SetActive(true);

		petInstance = (GameObject)Instantiate(petPrefab, Vector3.zero, Quaternion.identity);
		petInstance.transform.SetParent(transform);	
		do{
			inhalerItem.transform.position = CameraUtils.RandomWorldPointOnScreen(Camera.main, .1f, .1f);
			petInstance.transform.position = CameraUtils.RandomWorldPointOnScreen(Camera.main, .2f, .2f);
		} while (Vector3.Distance(inhalerItem.transform.position, petInstance.transform.position) < 2f);

		yield return finger.MoveTo(inhalerItem.transform.position, petInstance.GetComponent<MicroMixAnatomy>().mouth.transform.position, delay: .5f, time: 1f);

		finger.gameObject.SetActive(false);
	}
}
