using UnityEngine;
using System.Collections;

public class MiniPetAnimationEventListener : MonoBehaviour {
	private bool isVisibleInScene = false; //T: allow sound cilp to be played

	void Start(){
		CameraManager.Instance.PanScript.OnPartitionChanged += CheckMiniPetVisibleInPartition;

		int currentPartition = CameraManager.Instance.PanScript.currentPartition;
		string currentArea = GatingManager.Instance.currentArea;

		CheckMiniPetVisibleInPartition(currentArea, currentPartition);
	}

	void OnDestroy(){
//		CameraManager.Instance.PanScript.OnPartitionChanged -= CheckMiniPetVisibleInPartition;
	}

	public void PlaySoundClip(string clipName){
		if(!string.IsNullOrEmpty(clipName) && isVisibleInScene){
			Hashtable option = new Hashtable();
			option.Add("IsInterruptingRecurringClip", true);
			MiniPetAudioManager.Instance.PlayClip(clipName, option);
		}
	}

	public void PlayRecurringSoundClip(string clipName){
		if(!string.IsNullOrEmpty(clipName) && isVisibleInScene)
			MiniPetAudioManager.Instance.PlayRecurringClip(clipName);
	}

	public void PlayLoopingSoundClip(string clipName){
		if(!string.IsNullOrEmpty(clipName) && isVisibleInScene)
			MiniPetAudioManager.Instance.PlayLoopingClip(clipName);
	}

	private void CheckMiniPetVisibleInPartition(object sender, PartitionChangedArgs args){
		int currentPartition = args.newPartition;
		string currentArea = GatingManager.Instance.currentArea; 
		CheckMiniPetVisibleInPartition(currentArea,currentPartition);
	}

	private void CheckMiniPetVisibleInPartition(string currentArea ,int currentPartition){
		string miniPetID = this.transform.parent.parent.GetComponent<MiniPet>().MinipetId;
		// Camera treats the yard as partition 00 and partition 01 instead of 05 and 06.... this is a work around
		if( currentArea == "Yard"){
			currentPartition += 5;
		}
		if(MiniPetManager.Instance.GetPartitionNumberForMinipet(miniPetID) == currentPartition){
			isVisibleInScene = true;
		}
		else{
			isVisibleInScene = false;
		}
	}
}
