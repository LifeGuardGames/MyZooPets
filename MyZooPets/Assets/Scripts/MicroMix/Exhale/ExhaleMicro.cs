using UnityEngine;
using System.Collections;

public class ExhaleMicro : Micro{
	public GameObject petPrefab;
	private GameObject petInstance;

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
		petInstance = (GameObject)Instantiate(petPrefab, Vector3.zero, Quaternion.identity);
		petInstance.transform.SetParent(transform);
		if (randomize){
			petInstance.transform.position=CameraUtils.RandomWorldPointOnScreen(Camera.main,.2f,.2f);
		}
	}

	protected override void _EndMicro(){
		Destroy(petInstance);
	}

	protected override IEnumerator _Tutorial(){
		petInstance = (GameObject)Instantiate(petPrefab, Vector3.zero, Quaternion.identity);
		petInstance.transform.SetParent(transform);
		ExhaleItem exhale = petInstance.GetComponentInChildren<ExhaleItem>();

		MicroMixFinger finger = MicroMixManager.Instance.finger;
		finger.gameObject.SetActive(true);
		Vector3 moveTo = new Vector3(2f, 0f);
		Vector3 offset = new Vector3(.5f, .5f);
		yield return finger.MoveTo(exhale.transform.position + offset, exhale.transform.position + offset + moveTo, delay: .5f, time: 1f);
		finger.gameObject.SetActive(false);
		Destroy(petInstance);

	}
}
