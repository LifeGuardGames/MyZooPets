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
		petInstance = (GameObject)Instantiate(petPrefab, Vector3.zero, Quaternion.identity);
		petInstance.transform.SetParent(transform);	
		if(randomize){
			do{
				inhalerItem.transform.position = CameraUtils.RandomWorldPointOnScreen(Camera.main, .1f, .1f);
				petInstance.transform.position = CameraUtils.RandomWorldPointOnScreen(Camera.main, .2f, .2f);
			} while (Vector3.Distance(inhalerItem.transform.position, petInstance.transform.position) < 2f);
		}
		inhalerItem.GetComponent<MoveMouthItem>().pet=petInstance;
	}

	protected override void _EndMicro(){
		Destroy(petInstance);
	}

	protected override IEnumerator _Tutorial(){
		yield return 0;
	}
}
