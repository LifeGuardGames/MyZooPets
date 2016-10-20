using UnityEngine;
using System.Collections;

public class CigMazeItem : MicroItem{
	public GameObject cigarette;
	public GameObject startPosition;
	public GameObject finishPosition;
	public Sprite[] smokeSprites;
	private IEnumerator smokeIEnum;
	private float startTime;
	private float currentTime;
	private bool hasSmoked = false;

	public override void StartItem(){
		SpriteRenderer[] spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
		for(int i = 0; i < spriteRenderers.Length; i++){
			if(spriteRenderers[i].gameObject == cigarette || spriteRenderers[i].gameObject == finishPosition){
				continue;
			}
			spriteRenderers[i].sprite = smokeSprites[Random.Range(0, smokeSprites.Length)];
		}
		startTime = 0;
		currentTime = 0;
		hasSmoked = false;
		cigarette.GetComponent<SpriteRenderer>().color = Color.clear;
		cigarette.GetComponent<Collider2D>().enabled = false;
		cigarette.GetComponentInChildren<ParticleSystem>().Stop();
		if(smokeIEnum != null){
			StopCoroutine(smokeIEnum);
		}
	}

	public override void OnComplete(){
		
	}

	void Update(){
		if(MicroMixManager.Instance.IsPaused){
			return;
		}
		currentTime += Time.deltaTime; //So that we can control our own timing
		if(currentTime - startTime > .5f && !hasSmoked){ //0 to .5
			smokeIEnum = SmokeCigarette();
			StartCoroutine(smokeIEnum);
		}
	}

	private IEnumerator SmokeCigarette(){
		hasSmoked = true;
		int waitCount = 5;
		for(int i = 1; i <= waitCount; i++){ //.5 to 1
			cigarette.GetComponent<SpriteRenderer>().color = Color.white * i / waitCount;
			yield return MicroMixManager.Instance.WaitSecondsPause(.1f);
		}
		cigarette.GetComponent<Collider2D>().enabled = true;
		cigarette.GetComponentInChildren<ParticleSystem>().Play();
		yield return new WaitForSeconds(.5f); //1 to 1.5 (then back to 0)
		hasSmoked = false;
		cigarette.GetComponent<SpriteRenderer>().color = Color.clear;
		cigarette.GetComponent<Collider2D>().enabled = false;
		cigarette.GetComponentInChildren<ParticleSystem>().Stop();
		startTime = currentTime;
		smokeIEnum = null;
	}
}
