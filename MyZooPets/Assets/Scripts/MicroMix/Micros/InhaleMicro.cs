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

	// Use this for initialization
	public override void StartMicro(int difficulty){
		petInstance = (GameObject)Instantiate(petPrefab, Vector3.zero, Quaternion.identity);
		petInstance.transform.SetParent(transform);
		GetComponentInChildren<Animator>().Play("Breathe Out", 0, 1);
		InhaleItem item = petInstance.GetComponentInChildren<InhaleItem>();
		item.parent = this;
		item.inhaler=inhaler;
		inhaler.transform.position=petInstance.transform.position+new Vector3(3.2f,2f);
		base.StartMicro(difficulty);
	}

	protected override void OnComplete(){
		base.OnComplete();
		Destroy(petInstance);
	}

	void Update(){
	}
}
