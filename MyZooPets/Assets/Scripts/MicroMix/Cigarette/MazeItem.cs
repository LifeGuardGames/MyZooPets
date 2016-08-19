using UnityEngine;
using System.Collections;

public class MazeItem : MicroItem{
	public GameObject cigarette;
	public GameObject startPosition;
	public GameObject finishPosition;
	public Sprite[] smokeSprites;
	private float lastRandomize;
	private float intervalRandomize = 1f;
	private bool hasSmoked = false;
	private float startTime;

	public override void StartItem(){
		lastRandomize = Time.time;
		SpriteRenderer[] spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
		for(int i = 0; i < spriteRenderers.Length; i++){
			if(spriteRenderers[i].gameObject == cigarette || spriteRenderers[i].gameObject == finishPosition){
				continue;
			}
			spriteRenderers[i].sprite = smokeSprites[Random.Range(0, smokeSprites.Length)];
		}
		startTime = Time.time;
		hasSmoked = false;
		cigarette.GetComponent<SpriteRenderer>().color=Color.clear;
		cigarette.GetComponent<Collider2D>().enabled=false;
		cigarette.GetComponentInChildren<ParticleSystem>().Stop();
	}

	public override void OnComplete(){
		
	}

	void Update(){
		if(MicroMixManager.Instance.IsPaused){
			return;
		}
		if(Time.time - startTime > .5f && !hasSmoked){ //0 to .5
			StartCoroutine(SmokeCigarette());
		}
	}
	private IEnumerator SmokeCigarette(){
		hasSmoked = true;
		int waitCount = 5;
		for(int i = 1; i <= waitCount; i++){ //.5 to 1
			cigarette.GetComponent<SpriteRenderer>().color = Color.white * i/waitCount;
			yield return WaitSecondsPause(.1f);
		}
		cigarette.GetComponent<Collider2D>().enabled=true;
		cigarette.GetComponentInChildren<ParticleSystem>().Play();
		yield return new WaitForSeconds(.5f); //1 to 1.5
		hasSmoked = false;
		cigarette.GetComponent<SpriteRenderer>().color=Color.clear;
		cigarette.GetComponent<Collider2D>().enabled=false;
		cigarette.GetComponentInChildren<ParticleSystem>().Stop();
		startTime = Time.time;
	}

	private IEnumerator WaitSecondsPause(float time){ //Like wait for seconds, but pauses w/ RunnerGameManager
		for(float i = 0; i <= time; i += .1f){
			yield return new WaitForSeconds(.1f);
			while(MicroMixManager.Instance.IsPaused){
				yield return new WaitForEndOfFrame();
			}
		}
	}
}
