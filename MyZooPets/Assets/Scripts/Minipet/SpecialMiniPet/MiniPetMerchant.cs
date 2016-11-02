using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MiniPetMerchant : MiniPet{

	private ImmutableDataMerchantItem secretMerchantItem;
	private bool isItemBought;

	void Awake(){
		minipetType = MiniPetTypes.Merchant;
	}

	protected override void Start(){
		base.Start();
		List<ImmutableDataMerchantItem> merchantItemsList = DataLoaderMerchantItem.GetDataList();
		secretMerchantItem = DataLoaderMerchantItem.GetData(merchantItemsList[DataManager.Instance.GameData.MiniPets.GetItem(minipetId)].ID);
		isItemBought = DataManager.Instance.GameData.MiniPets.IsItemBoughtInPP(MinipetId);
	}

	protected override void OpenChildUI(){
		if(!MiniPetHUDUIManager.Instance.HasContent()){
			if(isFinishEating && !isItemBought){
				OpenMerchantContent();
			}
			else if(isItemBought){
				miniPetSpeechAI.ShowIdleMessage(MinipetType);
			}
		}
	}

	public override void FinishEating(){
		if(!isFinishEating){
			base.FinishEating();
			isFinishEating = true;
			miniPetSpeechAI.BeQuiet();
			miniPetSpeechAI.ShowMerchantShopMessage();
			OpenMerchantContent();
		}
		MiniPetHUDUIManager.Instance.CheckStoreButtonPulse();
	}

	private void OpenMerchantContent(){
		Hashtable hash = new Hashtable();
		hash[0] = secretMerchantItem.ItemId;
		hash[1] = secretMerchantItem.Type;
		MiniPetHUDUIManager.Instance.OpenUIMinipetType(MiniPetTypes.Merchant, hash, this); 
	}

	public void BuyItem(){
		isItemBought = true;
		DataManager.Instance.GameData.MiniPets.SetItemBoughtInPP(MinipetId, true);
		InventoryManager.Instance.AddItemToInventory(secretMerchantItem.ItemId);

		int cost = DataLoaderItems.GetCost(secretMerchantItem.ItemId);
		StatsManager.Instance.ChangeStats(coinsDelta: cost * -1);

		MiniPetManager.Instance.IncreaseXp(MinipetId);
	}
}
