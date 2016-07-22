using UnityEngine;
using System.Collections;

public class TimeMicro : Micro{
	public GameObject inhalerButton;
	public GameObject petPrefab;
	private GameObject petInstance;

	public override string Title{
		get{
			return "Time Inhaler";
		}
	}

	public override int Background{
		get{
			return 0;
		}
	}

	protected override void _StartMicro(int difficulty){
		petInstance = (GameObject)Instantiate(petPrefab, Vector3.zero, Quaternion.identity);
		petInstance.transform.SetParent(transform);	
		inhalerButton.GetComponent<TimeItem>().petInstance = petInstance;
	}

	protected override void _EndMicro(){
		Destroy(petInstance);
	}

	protected override IEnumerator _Tutorial(){
		yield return 0;
	}
}
