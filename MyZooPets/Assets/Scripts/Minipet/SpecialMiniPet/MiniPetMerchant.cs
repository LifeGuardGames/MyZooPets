using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MiniPetMerchant : MiniPet{
	public int itemsInList = 0;

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
				OpenMerchantStore();
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
			StartCoroutine(WaitASec());
		}
		MiniPetHUDUIManager.Instance.CheckStoreButtonPulse();
	}
	
	IEnumerator WaitASec(){
		yield return new WaitForSeconds(0.4f);
		OpenMerchantStore();
	}

	private void OpenMerchantStore(){
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
