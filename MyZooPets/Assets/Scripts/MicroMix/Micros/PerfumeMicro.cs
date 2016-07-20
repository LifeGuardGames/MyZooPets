using UnityEngine;
using System.Collections;

public class PerfumeMicro : Micro{
	private int count;
	private PerfumeItem[] perfumes;

	public override string Title{
		get{
			return "Avoid Perfume";
		}
	}

	public override int Background{
		get{
			return 2;
		}
	}

	protected override void _StartMicro(int difficulty){
		perfumes = GetComponentsInChildren<PerfumeItem>(true);
		SetWon(true);
		foreach(PerfumeItem perf in perfumes){
			perf.GetComponent<ParticleSystem>().Stop();
			perf.GetComponent<Collider>().enabled=false;
		}
		StartCoroutine(SpawnPerfume());

	}

	protected override void _EndMicro(){
		
	}
	private IEnumerator SpawnPerfume(){
		yield return new WaitForSeconds(.3f);
		foreach(PerfumeItem perf in perfumes){
			Vector3 startPos = GetRandomPositionOnEdge();
			Vector3 aim = CameraUtils.RandomWorldPointOnScreen(Camera.main,.25f,.25f);
			perf.GetComponent<ParticleSystem>().Play();
			perf.GetComponent<Collider>().enabled=true;
			perf.Setup(startPos,aim);
			yield return new WaitForSeconds(.9f);
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
		return CameraUtils.ScreenToWorldPointZero(Camera.main,new Vector2(x,y));
	}
}
