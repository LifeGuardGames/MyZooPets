using UnityEngine;
using System.Collections;

public class PerfumeMicro : Micro{
	public GameObject dashedLine;
	private int count;
	private PerfumeItem[] perfumes;

	public override string Title{
		get{
			return "Dodge";
		}
	}

	public override int Background{
		get{
			return 2;
		}
	}

	protected override void _StartMicro(int difficulty, bool randomize){
		perfumes = GetComponentsInChildren<PerfumeItem>(true);
		SetWon(true);
		foreach(PerfumeItem perf in perfumes){
			perf.GetComponent<ParticleSystem>().Stop();
			perf.GetComponent<Collider>().enabled = false;
		}
		StartCoroutine(SpawnPerfume(randomize));

	}

	protected override void _EndMicro(){
		
	}

	protected override IEnumerator _Tutorial(){
		yield return 0;
		Vector3	startPos = GetRandomPositionOnEdge();
		Vector3 aim = CameraUtils.RandomWorldPointOnScreen(Camera.main, .25f, .25f);
		PerfumeItem perfume = GetComponentInChildren<PerfumeItem>();
		perfume.Setup(startPos,aim);
		/*
		PerfumeItem perfume = GetComponentInChildren<PerfumeItem>();
		perfume.transform.position = GetRandomPositionOnEdge();
		dashedLine.transform.position = perfume.transform.position;
		dashedLine.GetComponent<Renderer>().enabled = true;
		Vector3 aim = CameraUtils.RandomWorldPointOnScreen(Camera.main, .25f, .25f);
		Vector3 delta = aim - dashedLine.transform.position;
		float angle = Mathf.Atan2(delta.y, delta.x) * Mathf.Rad2Deg - 90;

		perfume.GetComponent<ParticleSystem>().Play();
		dashedLine.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));

		MicroMixFinger finger = MicroMixManager.Instance.finger;
		finger.gameObject.SetActive(true);
		Vector3 playerAim = CameraUtils.RandomWorldPointOnScreen(Camera.main, .25f, .25f);

		GameObject dodgeObject = GetComponentInChildren<DodgeItem>().gameObject;
		dodgeObject.transform.position = CameraUtils.RandomWorldPointOnScreen(Camera.main, .25f, .25f);
		yield return finger.MoveTo(dodgeObject.transform.position, playerAim, .5f, 1f);

		dashedLine.GetComponent<Renderer>().enabled = false;
		finger.gameObject.SetActive(false);
		*/
	}

	private IEnumerator SpawnPerfume(bool randomize){ //Not called during tutorial
		yield return WaitSecondsPause(.3f);
		foreach(PerfumeItem perf in perfumes){
			Vector3 startPos;
			Vector3 aim;
			if(randomize){
				startPos = GetRandomPositionOnEdge();
				aim = CameraUtils.RandomWorldPointOnScreen(Camera.main, .25f, .25f);
			}
			else{
				startPos = perf.transform.position;
				aim = perf.aim;
				randomize = true;
			}
			perf.GetComponent<ParticleSystem>().Play();
			perf.GetComponent<Collider>().enabled = true;
			perf.Setup(startPos, aim);
			yield return WaitSecondsPause(.9f);
		}
	}

	private Vector3 GetRandomPositionOnEdge(){
		float x;
		float y;
		if(Random.value > .5f){
			if(Random.value > .5f){
				x = 0;
			}
			else{
				x = Screen.width;
			}
			y = Random.Range(0, Screen.height);
		}
		else{
			if(Random.value > .5f){
				y = 0;
			}
			else{
				y = Screen.height;
			}
			x = Random.Range(0, Screen.width);

		}
		return CameraUtils.ScreenToWorldPointZero(Camera.main, new Vector2(x, y));
	}
}
