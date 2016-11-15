using UnityEngine;
using UnityEngine.UI;

public class MiniPetMerchantUIController : MonoBehaviour {
	public Text itemNameLabel;
	public Text descriptionLabel;
	public Button buyButton;
	public Text cost;
	public Image itemImage;
	//BroughtItemTween prefab
	public GameObject boughtItemTweenPrefab;

	private Item secretItem;
	private MiniPetMerchant merchantScript;     // Reference to minipet logic

	public void InitializeContent(string itemID, bool isBoughtAlready, ItemType itemType, MiniPetMerchant merchantScript) {
		this.merchantScript = merchantScript;
		secretItem = DataLoaderItems.GetItem(itemID);
		itemNameLabel.text = secretItem.Name;
		descriptionLabel.text = secretItem.Description;
		itemImage.sprite = SpriteCacheManager.GetItemSprite(secretItem.ID);

		cost.text = secretItem.Cost.ToString();

		if(isBoughtAlready) {                       // Enable some game components here
			buyButton.gameObject.SetActive(false);
		}

		if(itemType == ItemType.Decorations) {
			Invoke("ShowDecoInventoryHelper", 1f);  // NOTE: Special invoke delay needed for update position
		}

		HUDUIManager.Instance.ShowPanel();          // Show the hud because we are buying stuff
	}

	/// <summary>
	/// This needs a helper because the inventory needs to
	/// update its new position (after eating food) before it should be hidden
	/// </summary>
	public void ShowDecoInventoryHelper() {
		DecoInventoryUIManager.Instance.ShowPanel();
		InventoryUIManager.Instance.HidePanel();
	}

	public void BuyItem() {
		if(DataManager.Instance.GameData.Stats.Stars >= secretItem.Cost) {
			merchantScript.BuyItem();
			buyButton.gameObject.SetActive(false);
			OnBuyAnimation(secretItem, itemImage.gameObject);
		}
		else {
			HUDUIManager.Instance.PlayNeedCoinAnimation();
			AudioManager.Instance.PlayClip("buttonDontClick");
		}
	}

	public void OnBuyAnimation(Item itemData, GameObject sprite) {
		Vector3 origin = new Vector3(sprite.transform.position.x, sprite.transform.position.y, 0f);
		Vector3 endPosition = origin;
		// depending on what type of item the user bought, the animation has the item going to different places
		switch(itemData.Type) {
			case ItemType.Decorations:
				endPosition = DecoInventoryUIManager.Instance.itemFlyToTransform.position;
				break;
			case ItemType.Accessories:
				Debug.LogError("Not implemented yet!");
				break;
			default:    // Everything else
				endPosition = InventoryUIManager.Instance.itemFlyToTransform.position;
				break;
		}
		
		GameObject animationSprite = GameObjectUtils.AddChild(sprite.transform.parent.gameObject, boughtItemTweenPrefab);
		animationSprite.transform.position = origin;
		animationSprite.transform.localScale = new Vector3(90, 90, 1);
		animationSprite.GetComponentInChildren<Image>().sprite = SpriteCacheManager.GetItemSprite(secretItem.ID);
		animationSprite.name = secretItem.ID;

		LeanTween.move(animationSprite, endPosition, 0.666f)
			.setEase(LeanTweenType.easeOutQuad)
			.setOnComplete(OnBuyAnimationDone)
			.setOnCompleteParam(animationSprite);
	}

	// Show the updated inventory bar with new item and destroy the tweening sprite
	private void OnBuyAnimationDone(object obj) {
		string itemID = ((GameObject)obj).name;
		ItemType type = DataLoaderItems.GetItem(itemID).Type;
		if(type == ItemType.Foods || type == ItemType.Usables) {
			InventoryUIManager.Instance.PulseInventory();
			InventoryUIManager.Instance.RefreshPage();
		}
		else if(type == ItemType.Decorations) {
			DecoInventoryUIManager.Instance.PulseInventory();
			DecoInventoryUIManager.Instance.RefreshPage();
		}
		Destroy((GameObject)obj);
	}
}
