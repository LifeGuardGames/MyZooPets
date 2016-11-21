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
			Setup();
		}
		InhaleItem item = inhaler.GetComponent<InhaleItem>();
		item.petInstance = petInstance;
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
		Setup();

		MicroMixFinger finger = MicroMixManager.Instance.finger;
		finger.gameObject.SetActive(true);
		yield return finger.MoveTo(inhaler.transform.position + new Vector3(3f, .5f), inhaler.transform.position + new Vector3(.3f, -.3f), delay: .5f, time: 1f);
		finger.gameObject.SetActive(false);
	}

	private void Setup(){
		petInstance = (GameObject)Instantiate(petPrefab, Vector3.zero, Quaternion.identity);
		petInstance.GetComponentInChildren<Animator>().Play("Breathe Out", 0, 1); //Already played during tutorial
		petInstance.transform.SetParent(transform);
		inhaler.transform.position = petInstance.GetComponent<MicroMixAnatomy>().mouth.transform.position;
	}
}
