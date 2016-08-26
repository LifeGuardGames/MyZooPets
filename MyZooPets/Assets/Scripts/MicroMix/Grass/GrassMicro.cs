using UnityEngine;
using System.Collections;

public class GrassMicro : Micro{
	public GameObject mower;
	public GameObject[] grasses;
	public GameObject phone;
	private int grassCount;

	public override string Title{
		get{
			return "Mow";
		}
	}

	public override int Background{
		get{
			return 3;
		}
	}

	public void RemoveGrass(GameObject grass){
		grass.SetActive(false);
		grassCount--;
		if(grassCount == 0){
			SetWon(true);
		}
	}

	protected override void _StartMicro(int difficulty, bool randomize){
		grassCount = grasses.Length;
		for(int i = 0; i < grasses.Length; i++){
			do {
			grasses[i].transform.position = CameraUtils.RandomWorldPointOnScreen(Camera.main,.2f,.2f,0);
			} while (Vector2.Distance(grasses[i].transform.position,mower.transform.position)<2f);
		}
	}

	protected override void _EndMicro(){
		for(int i = 0; i < grasses.Length; i++){
			grasses[i].SetActive(true);
		}
	}

	protected override void _Pause(){
		LeanTween.pause(phone);
		LeanTween.pause(mower);
	}

	protected override void _Resume(){
		LeanTween.resume(phone);
		LeanTween.resume(mower);
	}

	protected override IEnumerator _Tutorial(){
		phone.SetActive(true);
		for(int i = 1; i < grasses.Length; i++){
			grasses[i].SetActive(false);
		}
		grasses[0].transform.position = mower.transform.position + new Vector3(3,3);

		yield return MicroMixManager.Instance.WaitSecondsPause(.15f);

		LeanTween.rotateZ(phone,45,1f);
		LeanTween.rotateZ(mower.gameObject,270-45,1f);
		yield return MicroMixManager.Instance.WaitSecondsPause(1f);

		LeanTween.move(mower,grasses[0].transform.position,.5f);
		yield return MicroMixManager.Instance.WaitSecondsPause(.3f);

		grasses[0].SetActive(false);

		yield return MicroMixManager.Instance.WaitSecondsPause(.2f);

		for(int i = 0; i < grasses.Length; i++){
			grasses[i].SetActive(true);
		}
		phone.SetActive(false);
	}
}
