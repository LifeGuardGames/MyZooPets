using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class EmergencyMicro : Micro{
	public GameObject canvasParent;
	public AssemblyLineItem assemblyItem;
	public Transform button1;
	public Transform button2;
	public Transform button3;
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
		foreach(DoctorMatchZone buttonZone in FindObjectsOfType<DoctorMatchZone>()){
			buttonZone.GetComponent<Image>().color = Color.grey;
		}
	}

	protected override void _StartMicro(int difficulty, bool randomize){
		canvasParent.SetActive(true);
		assemblyItem.Init(0);
		complete = false;
	}

	protected override void _EndMicro(){
		foreach(DoctorMatchZone buttonZone in FindObjectsOfType<DoctorMatchZone>()){
			buttonZone.GetComponent<Image>().color = Color.white;
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
		finger.blur.SetActive(false);
		Vector3 offset = Vector3.left;

		assemblyItem.Init(0, 0);
		yield return finger.ShakeToBack(button1.transform.position + offset, button1.transform.position, delay: .2f, time: .4f);
		assemblyItem.Activate(false);
		yield return WaitSecondsPause(AssemblyLineItem.FADE_TIME*2);

		assemblyItem.Init(0, 1);
		yield return finger.ShakeToBack(button2.transform.position + offset, button2.transform.position, delay: .2f, time: .4f);
		assemblyItem.Activate(false);
		yield return WaitSecondsPause(AssemblyLineItem.FADE_TIME*2);

		assemblyItem.Init(0, 2);
		yield return finger.ShakeToBack(button3.transform.position + offset, button3.transform.position, delay: .2f, time: .4f);
		assemblyItem.Activate(false);
		yield return WaitSecondsPause(AssemblyLineItem.FADE_TIME*2);
		/*
		finger.gameObject.SetActive(true);

		waiting = true;
		assemblyItem.Init(0, 0);
		yield return finger.ShakeToBack(button1.transform.position + offset, button1.transform.position, delay: .2f, time: .4f);
		assemblyItem.Activate();
		yield return WaitSecondsPause(AssemblyLineItem.FADE_TIME * 2);

		waiting = true;
		assemblyItem.Init(0, 1);
		assemblyItem.Activate();
		yield return WaitSecondsPause(AssemblyLineItem.FADE_TIME * 2);

		waiting = true;
		assemblyItem.Init(0, 2);
		yield return WaitSecondsPause(AssemblyLineItem.FADE_TIME * 2);*/
		finger.blur.SetActive(true);
		finger.gameObject.SetActive(false);
	}

	protected IEnumerator WaitSecondsPause(float time){ //Like wait for seconds, but pauses w/ MicroMixManager
		for(float i = 0; i <= time; i += .1f){
			yield return new WaitForSeconds(.1f);
			while(MicroMixManager.Instance.IsPaused){
				yield return 0;
			}
		}
	}
}
