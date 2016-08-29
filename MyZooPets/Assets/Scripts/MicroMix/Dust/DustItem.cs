using UnityEngine;
using System.Collections;

public class DustItem : MicroItem{
	public ParticleSystem tapParticle;
	public bool rotate;
	private bool complete = false;
	private float size = 1;
	private int tapCount = 3;
	private float maxDist = .5f;
	// Use this for initialization
	public override void StartItem(){
		complete = false;
		tapCount = 3;
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
				spriteRend.gameObject.transform.rotation = Quaternion.Euler(new Vector3(0,0,Random.value*360));
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
		/*size -= .1f;
		transform.localScale = new Vector3(size, size, size);
		tapCount--;
		if(tapCount == 0){
			DustMicro dm = (DustMicro)parent;
			dm.Cleaned();
			complete = true;
			GetComponent<Renderer>().enabled = false;
			tapParticle.Play();
		}*/
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
		/*size -= .3f;
		transform.localScale = new Vector3(size, size, size);
		complete = true;
		DustMicro dm = (DustMicro)parent;
		dm.Cleaned();
		complete = true;
		GetComponent<Renderer>().enabled = false;
		tapParticle.Play();*/

	}
}
