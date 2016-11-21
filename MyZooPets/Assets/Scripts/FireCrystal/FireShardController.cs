using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// Represents each particular fire shard that is spawned when the fire crystals fill up
/// Controls the tweening and what happens when it is complete
/// </summary>
public class FireShardController : MonoBehaviour {

	public ParticleSystem finishParticle;
	public Image sprite;
	private bool isFirstSprite = false;
	private float pitchCount;

	public void StartMoving(Vector3 endLocation, float time, float pitchCount, bool isFirstSprite = false){
		this.isFirstSprite = isFirstSprite;
		this.pitchCount = pitchCount;
		LeanTween.moveLocal(gameObject, Vector3.zero, .8f)
			.setEase(LeanTweenType.easeInQuad)
			.setOnComplete(MovingComplete);
	}

	private void MovingComplete(){
		sprite.enabled = false;
		// Play some particle here TODO
		finishParticle.Play();

		// Play shard sound with pitch override
		Hashtable hashOverride = new Hashtable();
		hashOverride["Pitch"] = pitchCount;
		AudioManager.Instance.PlayClip("fireShardGet", option: hashOverride);	// Need better sound...

		// If this object is the first shard, call the animate meter function
		if(isFirstSprite){
			FireCrystalUIManager.Instance.StartFillFireSprite();
		}

		Destroy(gameObject, 2f);
	}
}
