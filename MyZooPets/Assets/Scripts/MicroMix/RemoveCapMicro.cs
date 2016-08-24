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

	protected override void _StartMicro(int difficulty, bool randomize){
		if(randomize){
			inhaler.transform.position = CameraUtils.RandomWorldPointOnScreen(Camera.main, .2f, .25f, 0);
		}
	}

	protected override void _EndMicro(){
	}

	protected override void _Pause(){
	}

	protected override void _Resume(){
	}

	protected override IEnumerator _Tutorial(){
		MicroMixFinger finger = MicroMixManager.Instance.finger;
		finger.gameObject.SetActive(true);
		CapItem cap = inhaler.GetComponentInChildren<CapItem>();
		yield return finger.MoveTo(cap.transform.position, cap.transform.position + cap.animDelta, delay: .5f, time: 1f);
		finger.gameObject.SetActive(false);
	}
}
