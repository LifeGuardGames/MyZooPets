using UnityEngine;
using System.Collections;

public class RemoveCapMicro : Micro {
	public GameObject inhaler;
	public GameObject[] fakes;

	protected override string Title{
		get{
			return "Remove Cap";
		}
	}
	public override void StartMicro(int difficulty){
		base.StartMicro(difficulty);
		inhaler.SetActive(true);
		inhaler.transform.position=CameraUtils.RandomWorldPointOnScreen(Camera.main);
		fakes[0].SetActive(true);
		fakes[0].transform.position=CameraUtils.RandomWorldPointOnScreen(Camera.main);
	}
	public override void EndMicro(bool won){
		base.EndMicro(won);
		inhaler.SetActive(false);
		foreach (GameObject fake in fakes){
			fake.SetActive(false);
		}
	}
}
