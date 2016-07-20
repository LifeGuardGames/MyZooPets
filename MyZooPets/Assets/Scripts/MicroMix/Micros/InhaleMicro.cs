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
		GetComponentInChildren<Animator>().Play("Breathe Out", 0, 1);
		InhaleItem item = petInstance.GetComponentInChildren<InhaleItem>();
		item.inhaler = inhaler;
		inhaler.transform.position = petInstance.transform.position + new Vector3(3.2f, 2f);
	}

	protected override void _EndMicro(){
		Destroy(petInstance);
	}
}
