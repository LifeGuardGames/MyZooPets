using UnityEngine;
using System.Collections;

public class RoachMicro : Micro {
	public GameObject cockroach;
	public GameObject trap;
	public override string Title{
		get{
			return "Catch Cockroach";
		}
	}
	public override int Background{
		get{
			return 1;
		}
	}

	protected override void _StartMicro(int difficulty){
		cockroach.transform.position=CameraUtils.RandomWorldPointOnScreen(Camera.main,.2f,.2f);

	}
	protected override void _EndMicro(){
	}

}
