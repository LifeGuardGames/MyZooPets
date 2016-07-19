using UnityEngine;
using System.Collections;

public class DustItem : MicroItem{
	public ParticleSystem tapParticle;
	private bool complete = false;
	private float size = 1;
	private int tapCount = 3;
	// Use this for initialization
	public override void StartItem(){
		complete = false;
		size = 1f;
		transform.localScale = new Vector3(size, size, size);
		tapCount = 3;
	}

	public void Tap(){
		size -= .1f;
		transform.localScale = new Vector3(size, size, size);
		tapCount--;
		tapParticle.Play();
		if(tapCount == 0){
			DustMicro dm = (DustMicro)parent;
			dm.Cleaned();
			complete = true;
			gameObject.SetActive(false);
		}
	}
}
