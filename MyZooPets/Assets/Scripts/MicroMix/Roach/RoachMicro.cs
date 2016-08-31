using UnityEngine;
using System.Collections;

public class RoachMicro : Micro{
	public GameObject cockroach;
	public GameObject trap;
	public GameObject dashedLine;
	public GameObject circle;
	private Vector3 lastVelocity;

	public override string Title{
		get{
			return "Trap";
		}
	}

	public override int Background{
		get{
			return 1;
		}
	}

	protected override bool ResetPositions{
		get{
			return false;
		}
	}

	protected override void _StartMicro(int difficulty, bool randomize){
		if(randomize){
			cockroach.transform.position = CameraUtils.RandomWorldPointOnScreen(Camera.main, .2f, .2f, 0);
			cockroach.GetComponent<RoachItem>().Setup(Random.insideUnitCircle);
			cockroach.SetActive(true);
		}
		else{
			cockroach.GetComponent<RoachItem>().Setup(lastVelocity);
		}
	}

	protected override void _EndMicro(){
	}

	protected override void _Pause(){
	}

	protected override void _Resume(){
	}

	protected override IEnumerator _Tutorial(){
		circle.gameObject.SetActive(true);
		cockroach.transform.position = CameraUtils.RandomWorldPointOnScreen(Camera.main, .4f, .4f, 0);
		MicroMixFinger finger = MicroMixManager.Instance.finger;
		trap.GetComponent<TrapItem>().SetVisible(false);
		finger.gameObject.SetActive(true);
		dashedLine.GetComponentInChildren<SpriteRenderer>().enabled = true;

		float angle = Random.Range(0, 360);
		cockroach.GetComponent<Animator>().enabled = false;
		cockroach.transform.rotation = Quaternion.Euler(0, 0, angle);
		dashedLine.transform.rotation = Quaternion.Euler(0, 0, angle);
		dashedLine.transform.position = cockroach.transform.position;

		Vector3 direction = new Vector3(Mathf.Cos((angle + 90) * Mathf.Deg2Rad), Mathf.Sin((angle + 90) * Mathf.Deg2Rad));
		circle.transform.position = cockroach.transform.position + direction * 4;
		yield return finger.ShakeToBack(cockroach.transform.position + direction * 4 - Vector3.up * .2f, cockroach.transform.position + direction * 4 + Vector3.up * .2f, .3f, .4f);

		lastVelocity = direction;
		finger.gameObject.SetActive(false);
		circle.gameObject.SetActive(false);
		dashedLine.GetComponentInChildren<SpriteRenderer>().enabled = false;

	}

}
