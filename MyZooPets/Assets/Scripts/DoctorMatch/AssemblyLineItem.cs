using UnityEngine;

public class AssemblyLineItem : MonoBehaviour {
	public SpriteRenderer itemSprite;

	private DoctorMatchManager.DoctorMatchButtonTypes itemType;
	public DoctorMatchManager.DoctorMatchButtonTypes ItemType{
		get{
			return itemType;
		}
	}

	private int index;

	public void Init(int currentIndex){
		index = currentIndex;

		// Generate random type and populate everything
		int randomIndex = UnityEngine.Random.Range(0, 3);
		if(randomIndex == 1){
			itemType = DoctorMatchManager.DoctorMatchButtonTypes.Green;
		}
		else if(randomIndex == 2){
			itemType = DoctorMatchManager.DoctorMatchButtonTypes.Yellow;
		}
		else{
			itemType = DoctorMatchManager.DoctorMatchButtonTypes.Red;
		}
		itemSprite.sprite = LoadSpriteZoneType(itemType);
	}

	public void Activate(){
		Destroy(gameObject);
	}

	public int GetIncrementIndex(){
		index--;
		return index;
	}
	public void Complete(Vector3 endPosition, Vector3 offset){
		LeanTween.cancel(gameObject);
		LeanTween.move(gameObject, endPosition + offset*index, 1.5f);
		Debug.Log(endPosition + offset*index);
	}
	private Sprite LoadSpriteZoneType(DoctorMatchManager.DoctorMatchButtonTypes type){
		int randomIndex = UnityEngine.Random.Range(1, 6);
		Sprite spriteData = null;
		switch(type){
		case DoctorMatchManager.DoctorMatchButtonTypes.Green:
			spriteData = Resources.Load<Sprite>("happy_" + randomIndex);
			break;
		case DoctorMatchManager.DoctorMatchButtonTypes.Yellow:
			spriteData = Resources.Load<Sprite>("sick1_" + randomIndex);
			break;
		case DoctorMatchManager.DoctorMatchButtonTypes.Red:
			spriteData = Resources.Load<Sprite>("sick2_" + randomIndex);
			break;
		default:
			Debug.LogError("Invalid type " + type.ToString());
			break;
		}
		return spriteData;
	}
	public void CompareVisible(int toCompare){
		if (index>=toCompare){
			itemSprite.enabled=false;
		} else {
			itemSprite.enabled=true;
		}
	}
}
