using UnityEngine;
using System.Collections;

public class FireShardController : MonoBehaviour {

	public ParticleSystem finishParticle;
	public UISprite sprite;
	private bool isFirstSprite = false;

	public void StartMoving(Vector3 endLocation, float time, bool isFirstSprite = false){
		this.isFirstSprite = isFirstSprite;
		LeanTween.moveLocal(gameObject, Vector3.zero, .8f)
			.setEase(LeanTweenType.easeInQuad)
			.setOnComplete(MovingComplete);
	}

	private void MovingComplete(){
		sprite.enabled = false;

		// Play some particle here TODO
		finishParticle.Play();

		// If this object is the first shard, call the animate meter function
		if(isFirstSprite){
			FireCrystalUIManager.Instance.StartFillFireSprite();
		}
		Destroy(gameObject, 2f);
	}
}
