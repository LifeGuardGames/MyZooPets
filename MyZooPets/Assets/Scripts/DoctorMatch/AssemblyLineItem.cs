using UnityEngine;
using System.Collections;

public class AssemblyLineItem : MonoBehaviour{
	public static int SPRITE_COUNT = 6;
	public static float FADE_TIME = .3f;
	public SpriteRenderer itemSprite;
	private DoctorMatchManager.DoctorMatchButtonTypes itemType;

	public DoctorMatchManager.DoctorMatchButtonTypes ItemType{
		get{
			return itemType;
		}
	}

	private int index;

	public void Init(int currentIndex, int typeIndex = -1, int spriteIndex = -1){
		// Generate random type and populate everything
		LeanTween.alpha(gameObject, 1, 0); //Reset our visuals
		if(typeIndex == -1)
			typeIndex = UnityEngine.Random.Range(0, 3);
		index = currentIndex;
	
		if(typeIndex == 0){
			itemType = DoctorMatchManager.DoctorMatchButtonTypes.Green;
		}
		else if(typeIndex == 1){
			itemType = DoctorMatchManager.DoctorMatchButtonTypes.Yellow;
		}
		else{
			itemType = DoctorMatchManager.DoctorMatchButtonTypes.Red;
		}
		itemSprite.sprite = LoadSpriteZoneType(itemType, spriteIndex);
	}

	public void Activate(bool destroySelf = true){
		LTDescr fadeAlpha = LeanTween.alpha(gameObject, 0, FADE_TIME);
		if(destroySelf){
			fadeAlpha.setOnComplete(DestroySelf);
		}
	}

	public int GetIncrementIndex(){
		index--;
		return index;
	}

	private Sprite LoadSpriteZoneType(DoctorMatchManager.DoctorMatchButtonTypes type, int spriteIndex){ //Sprite index is already optional as above
		if(spriteIndex == -1)
			spriteIndex = UnityEngine.Random.Range(1, SPRITE_COUNT);
		Sprite spriteData = null;
		switch(type){
		case DoctorMatchManager.DoctorMatchButtonTypes.Green:
			spriteData = Resources.Load<Sprite>("happy_" + spriteIndex);
			break;
		case DoctorMatchManager.DoctorMatchButtonTypes.Yellow:
			spriteData = Resources.Load<Sprite>("sick1_" + spriteIndex);
			break;
		case DoctorMatchManager.DoctorMatchButtonTypes.Red:
			spriteData = Resources.Load<Sprite>("sick2_" + spriteIndex);
			break;
		default:
			Debug.LogError("Invalid type " + type.ToString());
			break;
		}
		return spriteData;
	}

	public void CompareVisible(int toCompare, bool compare){
		if((index >= toCompare && compare) || index >= 4){
			itemSprite.enabled = false;
		}
		else{
			itemSprite.enabled = true;
		}
	}

	private void DestroySelf(){
		Debug.Log("Desctruct");
		Destroy(gameObject);
	}
}
