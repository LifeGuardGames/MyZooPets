using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class EmergencyMicro : Micro{
	public GameObject canvasParent;
	public AssemblyLineItem assemblyItem;
	public DoctorMatchZone[] buttons;
	private bool complete;

	public override int Background{
		get{
			return 3;
		}
	}

	public override string Title{
		get{
			return "Give Treatment";
		}
	}

	public void OnZoneClicked(DoctorMatchManager.DoctorMatchButtonTypes buttonType){
		if(complete || MicroMixManager.Instance.IsTutorial){
			return;
		}
		bool correct = buttonType == assemblyItem.ItemType;
		SetWon(correct);
		if(correct){
			Debug.Log("Insert fireworks here");
		}
		complete = true;
		assemblyItem.Activate(false);
		for(int i = 0; i < buttons.Length; i++){
			buttons[i].GetComponent<Image>().color = Color.grey;
			buttons[i].ToggleButtonInteractable(false);
		}
	}

	protected override void _StartMicro(int difficulty, bool randomize){
		canvasParent.SetActive(true);
		assemblyItem.Init(0);
		complete = false;
	}

	protected override void _EndMicro(){
		for(int i = 0; i < buttons.Length; i++){
			buttons[i].GetComponent<Image>().color = Color.white;
			buttons[i].ToggleButtonInteractable(true);
		}
		canvasParent.SetActive(false);
	}

	protected override void _Pause(){
		
	}

	protected override void _Resume(){
		
	}

	protected override IEnumerator _Tutorial(){
		canvasParent.SetActive(true);
		MicroMixFinger finger = MicroMixManager.Instance.finger;
		finger.gameObject.SetActive(true);
		Vector3 offset = Vector3.left * 1.5f;

		for(int i = 0; i < buttons.Length; i++){
			assemblyItem.Init(0, i);
			StartCoroutine(DelayParticlePlay(.4f,buttons[i].particle));
			yield return finger.ShakeToBack(buttons[i].transform.position + offset, buttons[i].transform.position, delay: .2f, time: .4f);
			assemblyItem.Activate(false);
			yield return WaitSecondsPause(AssemblyLineItem.FADE_TIME * 2);
		}

		yield return 0;
		finger.gameObject.SetActive(false);
	}

	private IEnumerator DelayParticlePlay(float delay, ParticleSystem toPlay){
		yield return WaitSecondsPause(delay);
		toPlay.Play();
	}
}
