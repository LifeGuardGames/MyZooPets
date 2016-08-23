using UnityEngine;
using System.Collections;

public class MicroMixFinger : MonoBehaviour{
	private float zPos = 0;

	public IEnumerator MoveTo(Vector3 startPos, Vector3 endPos, float delay, float time){
		transform.position = new Vector3(startPos.x, startPos.y, zPos);
		yield return WaitSecondsPause(delay);

		LeanTween.move(gameObject, new Vector3(endPos.x, endPos.y, zPos), time).setEase(LeanTweenType.easeInOutQuad);
		yield return WaitSecondsPause(time);
	}

	public IEnumerator ShakeToBack(Vector3 startPos, Vector3 endPos, float delay, float time){ //Time for the complete motion, move time is half that, twice
		transform.position = new Vector3(startPos.x, startPos.y, zPos);
		yield return WaitSecondsPause(delay);

		LeanTween.move(gameObject, new Vector3(endPos.x, endPos.y, zPos), time / 2).setEase(LeanTweenType.easeInOutQuad);
		yield return WaitSecondsPause(time / 2);

		LeanTween.move(gameObject, new Vector3(startPos.x, startPos.y, zPos), time / 2).setEase(LeanTweenType.easeInOutQuad);
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
