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


	protected override void Start(){
		base.Start();
		//temp
		items = new List<string>();
		timesVisited = PlayerPrefs.GetInt("TimesVisited");
		name = "Merchant";
		blackStoreButton = GameObject.Find("BlackStoreButton");
		items = DataManager.Instance.GameData.MiniPets.getMerchList(id);
		if(items == null){
			items = DataLoaderMerchantItem.getMerchantList();
			//itemsInList = items.Count;
			DataManager.Instance.GameData.MiniPets.saveMerchList(items,id);
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
		miniPetSpeechAI.showBlackShopMessage();
		timesVisited++;
		PlayerPrefs.SetInt("TimesVisited", timesVisited);
		//ShowStoreButton();
		StartCoroutine(WaitASec());
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
	IEnumerator WaitASec(){
		yield return new WaitForSeconds(0.4f);
		OpenStore();
	}

}
