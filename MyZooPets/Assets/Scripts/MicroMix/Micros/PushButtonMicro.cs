using UnityEngine;
using System.Collections;

public class PushButtonMicro : Micro{
	public GameObject inhaler;

	public override string Title{
		get{
			return "Push Button";
		}
	}

	public override int Background{
		get{
			return 0;
		}
	}

	protected override void _StartMicro(int difficulty){
		inhaler.transform.position = CameraUtils.RandomWorldPointOnScreen(Camera.main, .2f, .25f);
	}

	protected override void _EndMicro(){
		//Nothing to be done here
	}
	protected override IEnumerator _Tutorial(){
		yield return 0;
	}
}
