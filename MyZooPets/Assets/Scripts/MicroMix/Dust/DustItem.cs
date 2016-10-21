using UnityEngine;

public class DustItem : MicroItem{
	public ParticleSystem tapParticle;
	public bool rotate;
	private bool complete = false;
	private float maxDist = .5f;

	public override void StartItem(){
		complete = false;
	}

	public override void OnComplete(){
		SpriteRenderer[] spriteRends = GetComponentsInChildren<SpriteRenderer>(true);
		foreach(SpriteRenderer spriteRenderer in spriteRends){
			spriteRenderer.gameObject.SetActive(true);
		}
	}

	public void Randomize(){
		foreach(SpriteRenderer spriteRend in GetComponentsInChildren<SpriteRenderer>(true)){
			spriteRend.gameObject.SetActive(true);
			spriteRend.transform.localPosition = new Vector3(Random.Range(-maxDist,maxDist),Random.Range(-maxDist,maxDist));
			if (rotate){
				spriteRend.gameObject.transform.rotation = Quaternion.Euler(0,0,Random.value*360);
			}
		}
	}

	public void Drag(){
		if(complete){
			return;
		}
		SpriteRenderer[] spriteRends = GetComponentsInChildren<SpriteRenderer>();
		spriteRends[Random.Range(0,spriteRends.Length)].gameObject.SetActive(false);
		if(spriteRends.Length == 1){ //We just got the last one
			DustMicro dm = (DustMicro)parent;
			dm.Cleaned();
			complete = true;
			tapParticle.Play();
		}
	}

	public void Tap(){
		if(complete){
			return;
		}
		SpriteRenderer[] spriteRends = GetComponentsInChildren<SpriteRenderer>();
		spriteRends[Random.Range(0,spriteRends.Length)].gameObject.SetActive(false);
		if(spriteRends.Length == 1){ //We just got the last one
			DustMicro dm = (DustMicro)parent;
			dm.Cleaned();
			complete = true;
			tapParticle.Play();
		}
	}
}
