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
		secretMerchantItem = DataManager.Instance.GameData.MiniPets.GetItem(MinipetId);
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
			isPetCanGainXP = true;
			isFinishEating = true; 
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
		InventoryLogic.Instance.AddItem(secretMerchantItem.ItemId, 1);

		int cost = DataLoaderItems.GetCost(secretMerchantItem.ItemId);
		StatsController.Instance.ChangeStats(deltaStars: cost * -1);

		MiniPetManager.Instance.IncreaseXp(MinipetId);
	}
}
