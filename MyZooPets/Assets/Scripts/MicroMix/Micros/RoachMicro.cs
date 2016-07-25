using UnityEngine;
using System.Collections;

public class RoachMicro : Micro{
	public GameObject cockroach;
	public GameObject trap;
	public GameObject dashedLine;
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

	protected override void _StartMicro(int difficulty, bool randomize){
		if(randomize){
			cockroach.transform.position = CameraUtils.RandomWorldPointOnScreen(Camera.main, .2f, .2f);
			cockroach.GetComponent<RoachItem>().Setup(Random.insideUnitCircle);
		} else {
			cockroach.GetComponent<RoachItem>().Setup(lastVelocity);
		}

	}

	protected override void _EndMicro(){
	}

	protected override IEnumerator _Tutorial(){
		cockroach.transform.position = CameraUtils.RandomWorldPointOnScreen(Camera.main, .3f, .3f);
		MicroMixFinger finger = MicroMixManager.Instance.finger;
		trap.transform.position = new Vector3(100, 100);
		finger.gameObject.SetActive(true);
		dashedLine.GetComponent<SpriteRenderer>().enabled = true;

		float angle = Random.Range(0, 360);
		cockroach.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
		dashedLine.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
		dashedLine.transform.position = cockroach.transform.position;

		Vector3 direction = new Vector3(Mathf.Cos((angle + 90) * Mathf.Deg2Rad), Mathf.Sin((angle + 90) * Mathf.Deg2Rad));
		yield return finger.ShakeToBack(cockroach.transform.position + direction * 4.2f, cockroach.transform.position + direction * 3.8f, .3f, .4f);
		lastVelocity = direction;
	
		finger.gameObject.SetActive(false);
		dashedLine.GetComponent<SpriteRenderer>().enabled = false;
	}

}
