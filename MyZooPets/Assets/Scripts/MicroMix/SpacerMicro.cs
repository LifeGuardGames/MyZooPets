using UnityEngine;
using System.Collections;

public class SpacerMicro : Micro{
	public GameObject inhaler;
	public GameObject spacer;

	public override string Title{
		get{
			return "Attach Spacer";
		}
	}

	public override int Background{
		get{
			return 0;
		}
	}

	protected override void _StartMicro(int difficulty, bool randomize){
		spacer.GetComponent<SpacerItem>().inhaler = inhaler;
		if(randomize){
			do{
				inhaler.transform.position = CameraUtils.RandomWorldPointOnScreen(Camera.main, .1f, .1f, 0);
				spacer.transform.position = CameraUtils.RandomWorldPointOnScreen(Camera.main, .2f, .2f, 0);
			} while (Vector3.Distance(inhaler.transform.position, spacer.transform.position) < 2f);
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
		yield return finger.MoveTo(spacer.transform.position, inhaler.transform.position, delay: .5f, time: 1f);
		finger.gameObject.SetActive(false);
	}
}
