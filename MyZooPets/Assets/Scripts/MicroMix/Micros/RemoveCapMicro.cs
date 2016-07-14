using UnityEngine;
using System.Collections;

public class RemoveCapMicro : Micro{
	public GameObject inhaler;

	public override string Title{
		get{
			return "Remove Cap";
		}
	}
	protected override bool ResetPosition{
		get{
			return true;
		}
	}

	public override void StartMicro(int difficulty){
		base.StartMicro(difficulty);
		inhaler.transform.position = CameraUtils.RandomWorldPointOnScreen(Camera.main, .2f, .25f);
	}
}
