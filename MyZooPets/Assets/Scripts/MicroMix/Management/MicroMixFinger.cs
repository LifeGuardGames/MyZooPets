using UnityEngine;
using System.Collections;

public class MicroMixFinger : MonoBehaviour{
	private float zPos = 0;

	public IEnumerator MoveTo(Vector3 startPos, Vector3 endPos, float delay, float time){
		transform.position = new Vector3(startPos.x, startPos.y, zPos);
		yield return MicroMixManager.Instance.WaitSecondsPause(delay);

		LeanTween.move(gameObject, new Vector3(endPos.x, endPos.y, zPos), time).setEase(LeanTweenType.easeInOutQuad);
		yield return MicroMixManager.Instance.WaitSecondsPause(time);
	}

	public IEnumerator ShakeToBack(Vector3 startPos, Vector3 endPos, float delay, float time){ //Time for the complete motion, move time is half that, twice
		transform.position = new Vector3(startPos.x, startPos.y, zPos);
		yield return MicroMixManager.Instance.WaitSecondsPause(delay);

		LeanTween.move(gameObject, new Vector3(endPos.x, endPos.y, zPos), time / 2).setEase(LeanTweenType.easeInOutQuad);
		yield return MicroMixManager.Instance.WaitSecondsPause(time / 2);

		LeanTween.move(gameObject, new Vector3(startPos.x, startPos.y, zPos), time / 2).setEase(LeanTweenType.easeInOutQuad);
		yield return MicroMixManager.Instance.WaitSecondsPause(time / 2);
	}
}
