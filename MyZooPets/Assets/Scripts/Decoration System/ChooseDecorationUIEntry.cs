using UnityEngine;
using System.Collections;

/// <summary>
/// Choose decoration user interface entry.
/// attach to the ChooseDecoEntry Prefab
/// </summary>
public class ChooseDecorationUIEntry : MonoBehaviour {
	public UILabel itemName;
	public UISprite itemTexture;
	public UILabel itemAmount;
	public GameObject placeButtonGO;
	public UISprite xMark;
	public UISprite itemBackground;

	// the ID of the decoration this UI entry represents
	private string id;
	
	public void SetDecoID(string id){
		this.id = id;	
	}
	
	public string GetDecoID(){
		return id;
	}
}
