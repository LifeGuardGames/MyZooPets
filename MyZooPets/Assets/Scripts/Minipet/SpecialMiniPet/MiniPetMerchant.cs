using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MiniPetMerchant : MiniPet{
	
	private GameObject blackStoreButton;
	public int itemsInList = 0;
	private List<string> items;
	private ImmutableDataMerchantItem secItem;
	private bool isItemBought;

	protected override void Start(){
		base.Start();
		//temp
		items = new List<string>();
		name = "Merchant";
		blackStoreButton = GameObject.Find("BlackStoreButton");
		items = DataManager.Instance.GameData.MiniPets.getMerchList(id);
		if(items == null){
			items = DataLoaderMerchantItem.getMerchantList();
			//itemsInList = items.Count;
			DataManager.Instance.GameData.MiniPets.saveMerchList(items, id);
		}
	}

	protected override void OnTap(TapGesture gesture){	
		base.OnTap(gesture);
		if(!MiniPetHUDUIManager.Instance.HasContent()){
			if(isFinishEating && !isItemBought){
				Hashtable hash = new Hashtable();
				hash[0] = secItem.ItemId;
				hash[1] = secItem.Type;
				MiniPetHUDUIManager.Instance.OpenUIMinipetType(MiniPetTypes.Merchant, hash); 
			}
			else if (isItemBought){
				miniPetSpeechAI.ShowMerchantIdleMsg();
			}
		}
	}

	public override void FinishEating(){
		if(!isFinishEating){
		base.FinishEating();
		MiniPetManager.Instance.canLevel = true;
		isFinishEating = true; 
		miniPetSpeechAI.showBlackShopMessage();
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
		ImmutableDataMerchantItem itemData;
		if(DataManager.Instance.GameData.MiniPets.GetItem(id) == null){
			int max = items.Count;
			int rand = Random.Range(0, max);
			itemData = DataLoaderMerchantItem.GetData(items[rand]);
			DataManager.Instance.GameData.MiniPets.SetItem(id,itemData);
		}
		else{
			 itemData = DataManager.Instance.GameData.MiniPets.GetItem(id);
		}
		secItem = itemData;
		hash[0] = itemData.ItemId;
		hash[1] = itemData.Type;

		MiniPetHUDUIManager.Instance.OpenUIMinipetType(MiniPetTypes.Merchant, hash); 
	}

	public void RemoveItem(){
		items.Remove(secItem.ItemId);
		isItemBought = true;
		MiniPetManager.Instance.IncreaseXP(id);
	}

	public void ShowStoreButton(){
		blackStoreButton.SetActive(true);
	}
}
