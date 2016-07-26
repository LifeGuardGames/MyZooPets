using UnityEngine;
using System.Collections;

public class InhaleMicro : Micro{
	public GameObject petPrefab;
	public GameObject inhaler;
	private GameObject petInstance;

	public override string Title{
		get{
			return "Inhale";
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
			petInstance.GetComponentInChildren<Animator>().Play("Breathe Out", 0, 1);
			inhaler.transform.position = petInstance.GetComponent<MicroMixAnatomy>().mouth.transform.position;
		}
		InhaleItem item = inhaler.GetComponent<InhaleItem>();
		item.petInstance = petInstance;
	}

	protected override void _EndMicro(){
		Destroy(petInstance);
	}

	protected override void _Pause(){
	}

	protected override void _Resume(){
	}

	protected override IEnumerator _Tutorial(){
		petInstance = (GameObject)Instantiate(petPrefab, Vector3.zero, Quaternion.identity);
		petInstance.transform.SetParent(transform);
		petInstance.GetComponentInChildren<Animator>().Play("Breathe Out", 0, 1);

		inhaler.transform.position = petInstance.GetComponent<MicroMixAnatomy>().mouth.transform.position;

		MicroMixFinger finger = MicroMixManager.Instance.finger;
		finger.gameObject.SetActive(true);
		yield return finger.MoveTo(inhaler.transform.position + new Vector3(3f, .5f), inhaler.transform.position + new Vector3(.3f, -.3f), delay: .5f, time: 1f);
		finger.gameObject.SetActive(false);
	}
}
