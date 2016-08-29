using UnityEngine;
using System.Collections;

public class CampSmokeItem : MicroItem{
	private Vector3 aimPos;
	private IEnumerator moveIEnum;
	private float moveTime = .25f;
	private float hangTime = .5f;

	public bool Active{
		get{
			return moveIEnum != null;
		}
	}

	public float GetCurrentRadians(){
		return Mathf.Atan2(aimPos.y, aimPos.x);
	}

	public void Setup(float angle){
		aimPos = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle)) * 5;
		moveIEnum = MoveStayLeave();
		StartCoroutine(moveIEnum);
	}

	public void Cancel(){
		if(moveIEnum != null){
			StopCoroutine(moveIEnum);
			moveIEnum = null;
			LeanTween.move(gameObject, aimPos * 3, moveTime);
		}
	}

	public override void StartItem(){
		
	}

	public override void OnComplete(){
		
	}

	private IEnumerator MoveStayLeave(){
		//Move
		LeanTween.move(gameObject, aimPos, moveTime);
		yield return MicroMixManager.Instance.WaitSecondsPause(moveTime);
		//Stay
		yield return MicroMixManager.Instance.WaitSecondsPause(hangTime);
		//Leave
		LeanTween.move(gameObject, aimPos * 3, moveTime);
		yield return MicroMixManager.Instance.WaitSecondsPause(moveTime);

		moveIEnum = null;
	}
}
