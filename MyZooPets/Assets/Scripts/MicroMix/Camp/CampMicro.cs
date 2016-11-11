using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CampMicro : Micro {
	public static readonly float distance = 3f;
	public CampFireItem campfire;
	public CampPlayerItem player;
	public CampCollectItem[] mallows;
	public GameObject phone;
	public Text tutorialText;
	private int toCollect;
	public Animation headPlayer;

	public override string Title {
		get { return "Avoid Smoke"; }
	}

	public override int Background {
		get { return 4; }
	}

	public void Collect(GameObject mallow) {
		mallow.SetActive(false);
		toCollect--;
		if(toCollect == 0) {
			SetWon(true);
			campfire.Stop();
		}
	}

	protected override void _StartMicro(int difficulty, bool randomize) {
		float fireAngle = Random.value * Mathf.PI * 2;
		campfire.SetAngle(fireAngle);
		player.SetAngle(fireAngle - Mathf.PI);
		toCollect = mallows.Length;
		if(randomize) {
			campfire.SetupLogs();
			for(int i = 0; i < mallows.Length; i++) {
				mallows[i].Randomize();
			}
		}
		headPlayer.Play("MicroMixHeadHappy");
	}

	protected override void _SetWon(bool won) {
		if(!won) {
			
		}
	}

	protected override void _EndMicro() {
		for(int i = 0; i < mallows.Length; i++) {
			mallows[i].gameObject.SetActive(true);
		}
	}

	protected override void _Pause() {

	}

	protected override void _Resume() {

	}

	protected override IEnumerator _Tutorial() {
		phone.SetActive(true);
		player.SetAngle(Mathf.PI);
		campfire.SetAngle(0);
		campfire.Stop();
		campfire.SetupLogs();

		float mallowAngle = Mathf.PI;
		for(int i = 0; i < mallows.Length; i++) {
			mallowAngle += Mathf.PI / 4;
			mallows[i].transform.position = new Vector3(Mathf.Cos(mallowAngle), Mathf.Sin(mallowAngle)) * distance;
			mallows[i].Randomize();
		}

		LeanTween.textAlpha(tutorialText.rectTransform, 1f, .25f).setEase(LeanTweenType.easeInOutQuad);
		tutorialText.text = "Tilt your device";
		campfire.RotateTowards(Mathf.PI, 2f);
		yield return MicroMixManager.Instance.WaitSecondsPause(1f);

		LeanTween.rotateZ(phone, 90 + 45, .5f);
		player.RotateTowards(2.5f * Mathf.PI, 1.5f);
		yield return MicroMixManager.Instance.WaitSecondsPause(2f);

		LeanTween.rotateZ(phone, 90 - 45, .5f);
		player.RotateTowards(1.5f * Mathf.PI, 1f);
		LeanTween.textAlpha(tutorialText.rectTransform, 0f, .25f).setEase(LeanTweenType.easeInOutQuad);
		yield return MicroMixManager.Instance.WaitSecondsPause(1f);

		for(int i = 0; i < mallows.Length; i++) {
			mallows[i].gameObject.SetActive(true);
		}
		phone.SetActive(false);
	}
}
