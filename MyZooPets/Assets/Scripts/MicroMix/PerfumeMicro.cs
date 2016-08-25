using UnityEngine;
using System.Collections;

public class PerfumeMicro : Micro{
	public GameObject dashedLine;
	public GameObject perfumeBottle;
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
		foreach(PerfumeItem perf in perfumes){
			perf.GetComponent<ParticleSystem>().Stop();
			perf.GetComponent<Collider>().enabled = false;
		}
		StartCoroutine(SpawnPerfume(randomize));

	}

	protected override void _EndMicro(){
		SetWon(GetComponentInChildren<DodgeItem>().complete);
	}

	protected override void _Pause(){
		foreach(Animation anim in GetComponentsInChildren<Animation>()){
			anim.enabled = false;
		}
		foreach(ParticleSystem pSystem in GetComponentsInChildren<ParticleSystem>()){
			pSystem.Pause();
		}
	}

	protected override void _Resume(){
		foreach(Animation anim in GetComponentsInChildren<Animation>()){
			anim.enabled = true;
		}
		foreach(ParticleSystem pSystem in GetComponentsInChildren<ParticleSystem>()){
			pSystem.Play();
		}
	}

	protected override IEnumerator _Tutorial(){
		Vector3	startPos = GetRandomPositionOnEdge();
		Vector3 aim = CameraUtils.RandomWorldPointOnScreen(Camera.main, .25f, .25f, 0);
		PerfumeItem perfume = GetComponentInChildren<PerfumeItem>();
		perfume.Setup(startPos, aim);

		perfumeBottle.transform.position = perfume.transform.position;

		dashedLine.gameObject.SetActive(true);
		Vector3 deltaPos = aim - startPos;
		float angle = Mathf.Atan2(deltaPos.y, deltaPos.x) * Mathf.Rad2Deg;
		angle -= 90;
		dashedLine.transform.position = perfume.transform.position;
		dashedLine.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));

		yield return new WaitForSeconds(2f);

		dashedLine.gameObject.SetActive(false);
	}

	private IEnumerator SpawnPerfume(bool randomize){ //Not called during tutorial
		Vector3 startPos;
		Vector3 nextPos;
		Vector3 aim;

		if(randomize){
			nextPos = GetRandomPositionOnEdge();
			aim = CameraUtils.RandomWorldPointOnScreen(Camera.main, .25f, .25f, 0);
		}
		else{
			nextPos = perfumes[0].transform.position;
			aim = perfumes[0].aim;
			randomize = true;
		}

		perfumeBottle.transform.position = nextPos;

		for(int i = 0; i < perfumes.Length; i++){
			PerfumeItem perf = perfumes[i];
			startPos = nextPos;
			perf.GetComponent<ParticleSystem>().Play();
			perf.GetComponent<Collider>().enabled = true;
			perf.transform.position = startPos;

			yield return MicroMixManager.Instance.WaitSecondsPause(.3f);

			perf.Setup(startPos, aim);
			if(i < perfumes.Length - 1){
				nextPos = GetRandomPositionOnEdge();
				aim = CameraUtils.RandomWorldPointOnScreen(Camera.main, .25f, .25f, 0);
				perfumeBottle.transform.position = nextPos;
			}

			yield return MicroMixManager.Instance.WaitSecondsPause(.6f);


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
		return CameraUtils.ScreenToWorldPointZ(Camera.main, new Vector2(x, y), 0);
	}
}
