using UnityEngine;
using System.Collections;

public class CampMicro : Micro{
	public static readonly float distance = 3.5f;
	public CampFireItem campfire;
	public CampPlayerItem player;
	public CampCollectItem[] mallows;
	public GameObject phone;
	private int toCollect;

	public override string Title{
		get{
			return "Avoid Smoke";
		}
	}

	public override int Background{
		get{
			return 4;
		}
	}

	public void Collect(GameObject mallow){
		mallow.SetActive(false);
		toCollect--;
		if(toCollect == 0){
			SetWon(true);
			campfire.Stop();
		}
	}

	protected override void _StartMicro(int difficulty, bool randomize){
		float fireAngle = Random.value * Mathf.PI * 2;
		campfire.SetAngle(fireAngle);
		player.SetAngle(fireAngle - Mathf.PI);
		toCollect = mallows.Length;
	}

	protected override void _EndMicro(){
		for(int i = 0; i < mallows.Length; i++){
			mallows[i].gameObject.SetActive(true);
		}
	}

	protected override void _Pause(){
		
	}

	protected override void _Resume(){
		
	}

	protected override IEnumerator _Tutorial(){
		phone.SetActive(true);
		player.SetAngle(Mathf.PI);
		campfire.SetAngle(0);
		campfire.Stop();
		float mallowAngle = Mathf.PI;
		for(int i = 0; i < mallows.Length; i++){
			mallowAngle += Mathf.PI / 4;
			mallows[i].transform.position = new Vector3(Mathf.Cos(mallowAngle), Mathf.Sin(mallowAngle)) * distance;
		}

		campfire.RotateTowards(Mathf.PI, 2f);
		yield return MicroMixManager.Instance.WaitSecondsPause(1f);

		LeanTween.rotateZ(phone, 90 + 45, 1f);
		player.RotateTowards(2.5f * Mathf.PI, 1.5f);
		yield return MicroMixManager.Instance.WaitSecondsPause(2f);

		//campfire.RotateTowards(Mathf.PI*2, 1f);

		LeanTween.rotateZ(phone, 90 - 45, 1f);
		player.RotateTowards(1.5f * Mathf.PI, 1f);
		yield return MicroMixManager.Instance.WaitSecondsPause(1f);


		phone.SetActive(false);
	}
}
