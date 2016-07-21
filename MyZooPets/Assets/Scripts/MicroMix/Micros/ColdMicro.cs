﻿using UnityEngine;
using System.Collections;

public class ColdMicro : Micro {
	public GameObject petPrefab;
	public GameObject scarfItem;
	private GameObject petInstance;
	public override string Title{
		get{
			return "Cover Up";
		}
	}
	public override int Background{
		get{
			return 5;
		}
	}
	protected override void _StartMicro(int difficulty){
		petInstance = (GameObject)Instantiate(petPrefab, Vector3.zero, Quaternion.identity);
		petInstance.transform.SetParent(transform);	
		do {
			scarfItem.transform.position = CameraUtils.RandomWorldPointOnScreen(Camera.main, .1f, .1f);
			petInstance.transform.position = CameraUtils.RandomWorldPointOnScreen(Camera.main, .2f, .2f);
		} while (Vector3.Distance(scarfItem.transform.position,petInstance.transform.position)<2f);

	}
	protected override void _EndMicro(){
		Destroy(petInstance);
	}
}