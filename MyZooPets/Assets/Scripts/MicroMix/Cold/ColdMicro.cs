using UnityEngine;
using System.Collections;

public class ColdMicro : Micro{
	public GameObject petPrefab;
	public GameObject scarfItem;
	private GameObject petInstance;
	private Vector3 lastPos;

	public override string Title{
		get{
			return "Cover Up";
		}
	}

	public override int Background{
		get{
			return 5;
		}
	}

	protected override void _StartMicro(int difficulty, bool randomize){
		petInstance = (GameObject)Instantiate(petPrefab, Vector3.zero, Quaternion.identity);
		petInstance.transform.SetParent(transform);	
		if(randomize){
			do{
				scarfItem.transform.position = CameraUtils.RandomWorldPointOnScreen(Camera.main, .1f, .1f);
				petInstance.transform.position = CameraUtils.RandomWorldPointOnScreen(Camera.main, .2f, .2f);
			} while (Vector3.Distance(scarfItem.transform.position, petInstance.transform.position) < 2f);
		}
		else{
			petInstance.transform.position = lastPos;
		}
		scarfItem.GetComponent<ScarfItem>().pet=petInstance;
	}

	protected override void _EndMicro(){
		Destroy(petInstance);
	}

	protected override IEnumerator _Tutorial(){
		petInstance = (GameObject)Instantiate(petPrefab, Vector3.zero, Quaternion.identity);
		petInstance.transform.SetParent(transform);	
		do{
			scarfItem.transform.position = CameraUtils.RandomWorldPointOnScreen(Camera.main, .1f, .1f);
			petInstance.transform.position = CameraUtils.RandomWorldPointOnScreen(Camera.main, .2f, .2f);
		} while (Vector3.Distance(scarfItem.transform.position, petInstance.transform.position) < 4f);
		lastPos = petInstance.transform.position;
		MicroMixFinger finger = MicroMixManager.Instance.finger;
		finger.gameObject.SetActive(true);
		yield return finger.MoveTo(scarfItem.transform.position, petInstance.transform.position, delay: .5f, time: 1f);
		Destroy(petInstance);
		MicroMixManager.Instance.finger.gameObject.SetActive(false);
	}

}
