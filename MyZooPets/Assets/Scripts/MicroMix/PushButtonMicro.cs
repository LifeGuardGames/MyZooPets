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

	protected override void _StartMicro(int difficulty, bool randomize){
		if(randomize){
			inhaler.transform.position = CameraUtils.RandomWorldPointOnScreen(Camera.main, .2f, .25f, 50);
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
		ButtonItem button = inhaler.GetComponentInChildren<ButtonItem>();
		Vector3 offset = new Vector3(0, .5f);
		yield return finger.ShakeToBack(button.transform.position + offset, button.transform.position + button.animDelta + offset, delay: .5f, time: 1f);
		finger.gameObject.SetActive(false);
	}
}
