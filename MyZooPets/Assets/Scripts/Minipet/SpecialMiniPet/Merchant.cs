using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Merchant : MiniPet {

	private int timesVisited = 0;
	private GameObject blackStoreButton;
	public int itemsInList = 0;
	private List<string> items;
	private ImmutableDataMerchantItem secItem;
	private bool isItemBought;


	void Awake(){
		//temp
		items = new List<string>();
		timesVisited = PlayerPrefs.GetInt("TimesVisited");
		name = "Merchant";
		blackStoreButton = GameObject.Find("BlackStoreButton");
		if(PlayerPrefs.GetInt("merchantItemCount")==0){
			items = DataLoaderMerchantItem.getMerchantList();
			//itemsInList = items.Count;
			PlayerPrefs.SetInt("merchantItemCount",1);
		}
	}

	protected override void OnTap(TapGesture gesture){	
		base.OnTap(gesture);
		if (isFinishEating && !isItemBought){
			Hashtable has = new Hashtable();
			has[0] = secItem.ItemId;
			has[1] = secItem.Type;
			MiniPetHUDUIManager.Instance.OpenUIMinipetType(MiniPetTypes.Merchant,has); 
		}
	}

	public override void FinishEating(){
		base.FinishEating();
		MiniPetManager.Instance.canLevel = true;
		isFinishEating = true; 
		miniPetSpeechAI.ShowTipMsg();
		timesVisited++;
		PlayerPrefs.SetInt("TimesVisited", timesVisited);
		//ShowStoreButton();
		OpenStore();
	}

	public void ShowStoreButton(){
		blackStoreButton.SetActive(true);
	}

	public void OpenStore(){
		Hashtable has = new Hashtable();
		int max = items.Count;
		int rand = Random.Range (0,max);
		ImmutableDataMerchantItem itemData = DataLoaderMerchantItem.GetData(items[rand]);
		secItem = itemData;
		has[0] = itemData.ItemId;
		has[1] = itemData.Type;
		MiniPetHUDUIManager.Instance.OpenUIMinipetType(MiniPetTypes.Merchant,has); 
	}

	public void removeItem(){
		items.Remove(secItem.ItemId);
		isItemBought = true;
	}

}
