using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Merchant : MiniPet {

	private int timesVisited = 0;
	private GameObject blackStoreButton;
	public int itemsInList = 0;
	private List<string> items;

	void Awake(){
		//temp
		items = new List<string>();
		timesVisited = PlayerPrefs.GetInt("TimesVisited");
		name = "Merchant";
		blackStoreButton = GameObject.Find("BlackStoreButton");
		//if(itemsInList != PlayerPrefs.GetInt("merchantItemCount")){
			items = DataLoaderMerchantItem.getMerchantList();
			itemsInList = items.Count;
		//}
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
		has[0] = itemData.ItemId;
		has[1] = itemData.Type;
		MiniPetHUDUIManager.Instance.OpenUIMinipetType(MiniPetTypes.Merchant,has); 
	}
	

}
