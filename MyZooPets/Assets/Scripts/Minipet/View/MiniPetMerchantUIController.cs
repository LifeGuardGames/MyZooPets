using UnityEngine;
using System.Collections;

public class MiniPetMerchantUIController : MonoBehaviour {

	public UILocalize itemNameLocalize;
	public UILocalize descriptionLocalize;
	public UIImageButton buyButton;
	public UILabel cost;
	public UISprite sprite;
	private string itemId;
	private Item secItem;

	public void Initialize(string itemID, bool isBoughtAlready){
		Item secretItem = ItemLogic.Instance.GetItem(itemID);
		secItem = secretItem;
		itemId = itemID;
		itemNameLocalize.key = secretItem.Name;
		itemNameLocalize.Localize();
		sprite.spriteName = DataLoaderItems.GetItemTextureName(itemID);

		descriptionLocalize.key = secretItem.Description;
		descriptionLocalize.Localize();

		cost.text = secretItem.Cost.ToString();

		if(isBoughtAlready){
			// Enable some game components here

			buyButton.enabled = false;
		}
	}
	void BuyItem(){
		buyButton.enabled = false;
		InventoryLogic.Instance.AddItem(itemId, 1);
		StatsController.Instance.ChangeStats(deltaStars: (int)secItem.Cost * -1);
	}
}
