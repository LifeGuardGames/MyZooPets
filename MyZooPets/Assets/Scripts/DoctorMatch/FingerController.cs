using UnityEngine;
using System.Collections;

public class FingerController : MonoBehaviour{
	private float shakeTime = .15f;
	//private float shakeOffset = 20f;
	public IEnumerator Shake(Vector3 shakeOffset, float delay = 0f, bool destroyOnComplete = false){
		yield return new WaitForSeconds(delay);
		LeanTween.cancel(gameObject);
		TweenArgument argument = new TweenArgument();
		argument.destroyOnComplete = destroyOnComplete;
		argument.shakeOffset = shakeOffset;
		LeanTween.move(gameObject, transform.position + shakeOffset, shakeTime).setOnComplete(TweenBack).setEase(LeanTweenType.easeInOutSine).setOnCompleteParam(argument);
	}

	public void StopShake(Vector3 homePos){
		LeanTween.cancel(gameObject);
		transform.position = homePos;
	}

	private void TweenBack(object tweenArgument){
		TweenArgument argument = (TweenArgument)tweenArgument;
		LeanTween.move(gameObject, transform.position - argument.shakeOffset, shakeTime).setEase(LeanTweenType.easeInSine).setDestroyOnComplete(argument.destroyOnComplete);
	}

	public IEnumerator RepeatShake(int shakeCount, Vector3 shakeOffset, bool destroyOnComplete = false){
		for(int i = 0; i < shakeCount - 1; i++){
			StartCoroutine(Shake(shakeOffset));
			yield return new WaitForSeconds(shakeTime * 3);
		}
		StartCoroutine(Shake(shakeOffset, destroyOnComplete: destroyOnComplete));
	}

	public class TweenArgument{
		public bool destroyOnComplete;
		public Vector3 shakeOffset;
	}
}
