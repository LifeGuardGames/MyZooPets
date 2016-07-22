using UnityEngine;
using System.Collections;

public class InhaleMicro : Micro{
	//public LgInhalerAnimationEventHandler animHandler;
	//public Animator petAnimator;
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

	// Use this for initialization
	protected override void _StartMicro(int difficulty){
		petInstance = (GameObject)Instantiate(petPrefab, Vector3.zero, Quaternion.identity);
		petInstance.transform.SetParent(transform);
		petInstance.GetComponentInChildren<Animator>().Play("Breathe Out", 0, 1);

		InhaleItem item = petInstance.GetComponentInChildren<InhaleItem>();
		item.inhaler = inhaler;
		inhaler.transform.position = petInstance.transform.position + new Vector3(3.2f, 2f);
	}

	protected override void _EndMicro(){
		Destroy(petInstance);
	}

	protected override IEnumerator _Tutorial(){
		petInstance = (GameObject)Instantiate(petPrefab, Vector3.zero, Quaternion.identity);
		petInstance.transform.SetParent(transform);
		petInstance.GetComponentInChildren<Animator>().Play("Breathe Out", 0, 1);

		InhaleItem item = petInstance.GetComponentInChildren<InhaleItem>();
		item.inhaler = inhaler;
		inhaler.transform.position = petInstance.transform.position + new Vector3(3.2f, 2f);

		MicroMixFinger finger = MicroMixManager.Instance.finger;
		finger.gameObject.SetActive(true);
		Vector3 moveTo = new Vector3(.5f, .6f);
		Vector3 offset = new Vector3(0f, -.3f);
		yield return finger.MoveTo(inhaler.transform.position + offset, item.transform.position + moveTo + offset, delay: .5f, time: 1f);
		finger.gameObject.SetActive(false);
	}
}
