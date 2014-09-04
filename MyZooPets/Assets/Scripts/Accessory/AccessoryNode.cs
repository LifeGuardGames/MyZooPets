using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Accessory node on the pet to be controlled by AccessoryNodeController
/// </summary>
public class AccessoryNode : MonoBehaviour{

	public string accessoryKey;
	public List<GameObject> spriteNodes = new List<GameObject>();	// List because there might be different versions of the same body part

	void Start(){
		if(accessoryKey == null){
			Debug.LogError("Missing accessory key");
		}

		// Add self to the hash table
		AccessoryNodeController.Instance.accessoryNodeHash.Add(accessoryKey, this);
	}

	/// <summary>
	/// IMPORTANT: Only to be changed from AccessoryNodeController!
	/// </summary>
	/// <param name="atlas">Atlas.</param>
	/// <param name="spriteName">Sprite name.</param>
	public void ChangeAccessory(string atlas, string spriteName){
		foreach(GameObject spriteNode in spriteNodes){
			// TODO
		}
	}
}
