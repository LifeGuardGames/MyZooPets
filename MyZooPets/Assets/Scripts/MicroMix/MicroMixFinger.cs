using UnityEngine;
using System.Collections;

public class MicroMixFinger : MonoBehaviour{
	
	public IEnumerator MoveTo(Vector3 startPos, Vector3 endPos, float delay, float time){
		transform.position = startPos;
		yield return WaitSecondsPause(delay);

		LeanTween.move(gameObject, endPos, time).setEase(LeanTweenType.easeInOutQuad);
		yield return WaitSecondsPause(time);
	}

	public IEnumerator ShakeToBack(Vector3 startPos, Vector3 endPos, float delay, float time){ //Time for the complete motion, move time is half that, twice
		transform.position = startPos;
		yield return WaitSecondsPause(delay);

		LeanTween.move(gameObject, endPos, time / 2).setEase(LeanTweenType.easeInOutQuad);
		yield return WaitSecondsPause(time / 2);

		LeanTween.move(gameObject, startPos, time / 2).setEase(LeanTweenType.easeInOutQuad);
		yield return WaitSecondsPause(time / 2);
	}

	private IEnumerator WaitSecondsPause(float time){ //Like wait for seconds, but pauses w/ MicroMixManager. Also pauses our tween
		for(float i = 0; i <= time; i += .1f){
			yield return new WaitForSeconds(.1f);
			if(MicroMixManager.Instance.IsPaused){
				LeanTween.pause(gameObject);
				while(MicroMixManager.Instance.IsPaused){
					yield return new WaitForEndOfFrame();
				}
				LeanTween.resume(gameObject);
			}
		}
	}
}
