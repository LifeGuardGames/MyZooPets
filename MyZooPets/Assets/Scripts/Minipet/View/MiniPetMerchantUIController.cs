using UnityEngine;
using System.Collections;

public class MiniPetMerchantUIController : MonoBehaviour {

	public UILocalize itemNameLocalize;
	public UILocalize descriptionLocalize;
	public UIImageButton buyButton;
	public UILabel cost;

	public void Initialize(string itemID, bool isBoughtAlready){
		Item secretItem = ItemLogic.Instance.GetItem(itemID);
		itemNameLocalize.key = secretItem.Name;
		itemNameLocalize.Localize();

		descriptionLocalize.key = secretItem.Description;
		descriptionLocalize.Localize();

		cost.text = secretItem.Cost.ToString();

		if(isBoughtAlready){
			// Enable some game components here

			buyButton.enabled = false;
		}
	}
}
