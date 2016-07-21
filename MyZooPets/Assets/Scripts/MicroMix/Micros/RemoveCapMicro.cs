using UnityEngine;
using System.Collections;

public class RemoveCapMicro : Micro{
	public GameObject inhaler;

	public override string Title{
		get{
			return "Remove Cap";
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
}
