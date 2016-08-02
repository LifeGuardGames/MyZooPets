using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MiniPetMerchantUIController : MonoBehaviour {
	public Text itemNameLabel;
	public Text descriptionLabel;
	public Button buyButton;
	public Text cost;
	public Image sprite;
	public GameObject itemSpritePrefab;
	public TweenToggle tweenToggle;
	private Item secretItem;

	private MiniPetMerchant merchantScript;		// Reference to minipet logic

	public void InitializeContent(string itemID, bool isBoughtAlready, ItemType itemType, MiniPetMerchant merchantScript){
		this.merchantScript = merchantScript;
		secretItem = DataLoaderItems.GetItem(itemID);

		itemNameLabel.text = secretItem.Name;
		descriptionLabel.text = secretItem.Description;
		//TODO: Fix this - dylan
		//sprite.spriteName = DataLoaderItems.GetItemTextureName(itemID);
		
		cost.text = secretItem.Cost.ToString();

		if(isBoughtAlready){						// Enable some game components here
			buyButton.gameObject.SetActive(false);
		}

		if(itemType == ItemType.Decorations){
			Invoke("ShowDecoInventoryHelper", 1f);	// NOTE: Special invoke delay needed for update position
		}

		HUDUIManager.Instance.ShowPanel();			// Show the hud because we are buying stuff
	}

	/// <summary>
	/// This needs a helper because the inventory needs to
	/// update its new position (after eating food) before it should be hidden
	/// </summary>
	public void ShowDecoInventoryHelper(){
		DecoInventoryUIManager.Instance.ShowPanel();
		InventoryUIManager.Instance.HidePanel();
	}

	public void BuyItem(){
		if(DataManager.Instance.GameData.Stats.Stars >= secretItem.Cost){
			merchantScript.BuyItem();
			buyButton.gameObject.SetActive(false);
			OnBuyAnimation(secretItem, sprite.gameObject);
		}
		else{
			HUDUIManager.Instance.PlayNeedCoinAnimation();
			AudioManager.Instance.PlayClip("buttonDontClick");
		}
	}

	public void OnBuyAnimation(Item itemData, GameObject sprite){
		Vector3 origin = new Vector3(sprite.transform.position.x, sprite.transform.position.y, -0.1f);
		Vector3 itemPosition = origin;
		
		//-0.22
		// depending on what type of item the user bought, the animation has the item going to different places
		switch(itemData.Type){
		case ItemType.Decorations:
			itemPosition = DecoInventoryUIManager.Instance.itemFlyToTransform.position;
			break;
		case ItemType.Accessories:
			 Debug.LogError("Not implemented yet!");
			break;
		default:	// Everything else
			itemPosition = InventoryUIManager.Instance.itemFlyToTransform.position;
			break;
		}
		
		//adjust moving speed here
		float speed = 1f;
		
		//Change the 3 V3 to where icon should move
		Vector3[] path = new Vector3[4];
		path[0] = origin;
		path[1] = origin + new Vector3(0f, 1.5f, -0.1f);
		path[2] = origin;
		path[3] = itemPosition;
		
		Hashtable optional = new Hashtable();
		GameObject animationSprite = NGUITools.AddChild(sprite.transform.parent.gameObject, itemSpritePrefab);
		
		animationSprite.transform.position = origin;
		animationSprite.transform.localScale = new Vector3(90, 90, 1);
		animationSprite.GetComponent<UISprite>().spriteName = sprite.GetComponent<UISprite>().spriteName;

		Debug.LogWarning("Tween sprite test start");
		LeanTween.move(animationSprite, path, speed)
			.setEase(LeanTweenType.easeOutQuad)
			.setOnComplete(DestroySprite)
			.setOnCompleteParam(animationSprite);
	}

	// delete the icon we moved
	public void DestroySprite(System.Object obj){
		Debug.LogWarning("Tween sprite test end");
		if(obj != null) {
			Destroy((GameObject)obj);
		}
	}
}
