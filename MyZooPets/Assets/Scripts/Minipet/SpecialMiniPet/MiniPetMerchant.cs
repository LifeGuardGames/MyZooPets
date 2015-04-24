using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MiniPetMerchant : MiniPet{
	
	private GameObject blackStoreButton;
	public int itemsInList = 0;
	private List<string> items;
	private ImmutableDataMerchantItem secItem;
	private bool isItemBought;

	void Awake(){
		minipetType = MiniPetTypes.Merchant;
	}

	protected override void Start(){
		base.Start();
		//temp
		items = new List<string>();
		blackStoreButton = GameObject.Find("BlackStoreButton");
		items = DataManager.Instance.GameData.MiniPets.getMerchList(minipetId);
		if(items == null){
			items = DataLoaderMerchantItem.getMerchantList();
			//itemsInList = items.Count;
			DataManager.Instance.GameData.MiniPets.saveMerchList(items, minipetId);
		}
		if(DataManager.Instance.GameData.MiniPets.GetItem(MinipetId) == -1){
			int max = items.Count;
			int rand = Random.Range(0, max);
			secItem = DataLoaderMerchantItem.GetData(items[rand]);
			DataManager.Instance.GameData.MiniPets.SetItem(MinipetId, rand);
		}
		else{
			secItem = DataLoaderMerchantItem.GetData(items[DataManager.Instance.GameData.MiniPets.GetItem(MinipetId)]);
		}
	}

	protected override void OpenChildUI(){
		if(!MiniPetHUDUIManager.Instance.HasContent()){
			if(isFinishEating && !isItemBought){
				Hashtable hash = new Hashtable();
				hash[0] = secItem.ItemId;
				hash[1] = secItem.Type;
				MiniPetHUDUIManager.Instance.OpenUIMinipetType(MiniPetTypes.Merchant, hash); 
			}
			else if(isItemBought){
				miniPetSpeechAI.ShowIdleMessage(MinipetType);
			}
		}
	}

	public override void FinishEating(){
		if(!isFinishEating){
			base.FinishEating();
			isPetCanGainXP = true;
			isFinishEating = true; 
			miniPetSpeechAI.ShowBlackShopMessage();
			//ShowStoreButton();
			StartCoroutine(WaitASec());
		}
	}

	IEnumerator WaitASec(){
		yield return new WaitForSeconds(0.4f);
		OpenStore();
	}

	public void OpenStore(){
		Hashtable hash = new Hashtable();

		hash[0] = secItem.ItemId;
		hash[1] = secItem.Type;

		MiniPetHUDUIManager.Instance.OpenUIMinipetType(MiniPetTypes.Merchant, hash); 
	}

	public void RemoveItem(){
		items.Remove(secItem.ItemId);
		isItemBought = true;
		MiniPetManager.Instance.IncreaseXP(minipetId);
	}

	public void ShowStoreButton(){
		blackStoreButton.SetActive(true);
	}
}
