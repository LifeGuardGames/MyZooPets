using UnityEngine;
using System.Collections;

public class ExhaleMicro : Micro {
	public override string Title{
		get{
			return "Exhale";
		}
	}
	protected override bool ResetPosition{
		get{
			return false;
		}
	}
	// Use this for initialization
	public override void StartMicro(int difficulty){
		base.StartMicro(difficulty);

	}
}
