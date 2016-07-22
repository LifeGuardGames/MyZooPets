﻿using UnityEngine;
using System.Collections;

public class ExhaleMicro : Micro{
	//public LgInhalerAnimationEventHandler animHandler;
	//public Animator petAnimator;
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
	protected override void _StartMicro(int difficulty){
		petInstance = (GameObject)Instantiate(petPrefab, Vector3.zero, Quaternion.identity);
		petInstance.transform.SetParent(transform);
	}

	protected override void _EndMicro(){
		Destroy(petInstance);
	}

	protected override IEnumerator _Tutorial(){
		yield return 0;
	}
}
