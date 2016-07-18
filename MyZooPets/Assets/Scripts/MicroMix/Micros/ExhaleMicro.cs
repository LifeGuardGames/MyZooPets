using UnityEngine;
using System.Collections;

public class ExhaleMicro : Micro {
	//public LgInhalerAnimationEventHandler animHandler;
	//public Animator petAnimator;
	public GameObject petPrefab;
	private GameObject petInstance;
	public override string Title{
		get{
			return "Exhale";
		}
	}
	// Use this for initialization
	public override void StartMicro(int difficulty){
		base.StartMicro(difficulty);
		petInstance = (GameObject) Instantiate(petPrefab,Vector3.zero,Quaternion.identity);
		petInstance.transform.SetParent(transform);
		petInstance.GetComponentInChildren<ExhaleItem>().parent=this;
	}
	protected override void OnComplete(){
		base.OnComplete();
		Destroy(petInstance);
	}
	void Update(){
	}
}
