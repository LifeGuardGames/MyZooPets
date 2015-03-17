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
	public GameObject itemSpritePrefab;

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
	public void BuyItem(){
		buyButton.gameObject.SetActive( false);
		InventoryLogic.Instance.AddItem(itemId, 1);
		StatsController.Instance.ChangeStats(deltaStars: (int)secItem.Cost * -1);
		OnBuyAnimation(secItem, sprite.gameObject);
	}
	public void OnBuyAnimation(Item itemData, GameObject sprite){
		Vector3 origin = new Vector3(sprite.transform.position.x, sprite.transform.position.y, -0.1f);
		string itemID = itemId;
		Vector3 itemPosition = origin;
		
		//-0.22
		// depending on what type of item the user bought, the animation has the item going to different places
		ItemType eType = itemData.Type;
		switch(eType){
		case ItemType.Decorations:
			itemPosition = DecoInventoryUIManager.Instance.GetPositionOfDecoInvItem(itemID);
			break;
		default:
			itemPosition = InventoryUIManager.Instance.GetPositionOfInvItem(itemID);
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
		GameObject animationSprite = NGUITools.AddChild(sprite, itemSpritePrefab);
		
		// hashtable for completion params for the callback (stash the icon we are animating)
		Hashtable completeParamHash = new Hashtable();
		completeParamHash.Add("Icon", animationSprite);		
		
		optional.Add("ease", LeanTweenType.easeOutQuad);
		optional.Add("onComplete", "DestroySprite");
		optional.Add("onCompleteTarget", gameObject);
		optional.Add("onCompleteParam", completeParamHash);
		animationSprite.transform.position = origin;
		animationSprite.transform.localScale = new Vector3(90, 90, 1);
		animationSprite.GetComponent<UISprite>().spriteName = sprite.GetComponent<UISprite>().spriteName;
		LeanTween.move(animationSprite, path, speed, optional);
	}

}
