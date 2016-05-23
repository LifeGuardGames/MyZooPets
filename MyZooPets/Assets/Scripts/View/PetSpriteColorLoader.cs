using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
public class PetSpriteColorLoader : MonoBehaviour {

	public string spriteSetPrefix;
	public SpriteRenderer[] spriteRendererList;
	public bool isDebug = false;
	private bool isDebugInternal = false;

	void Start(){
		try{
			if(Application.isPlaying){	// Exception for execute in edit mode
				if(isDebug){
					Debug.LogError("Debug for pet color is currently on! Make sure to uncheck!");
				}
				else{
					if(Application.loadedLevelName == SceneUtils.MENU){
						MutableDataPetInfo petMenuInfo = SelectionManager.Instance.PetMenuInfo;
						LoadAndSetColor(petMenuInfo.PetColor);
					}
					else{
						LoadAndSetColor(DataManager.Instance.GameData.PetInfo.PetColor);
					}
				}
			}
		}
		catch(Exception e){
			Debug.LogError("Exception caught in PetSpriteColorLoader with message: " + e.Message);
		}
	}

	private void LoadAndSetColor(string petColor){
//		Debug.Log("Loading Colors...");
		Sprite[] sprites = Resources.LoadAll<Sprite>(spriteSetPrefix + petColor);

		// Loop through all the body parts that needs color assignment
		foreach(SpriteRenderer spriteRenderer in spriteRendererList){
			// Load their index metadata
			int atlasIndex = spriteRenderer.GetComponent<PetSpriteColorMetadata>().spriteSetIndexToLoad;
			// Set their sprites according to index from metadata
			spriteRenderer.sprite = sprites[atlasIndex];
		}

//		Debug.Log("Loading Colors done...");
	}

	private void ClearDebug(){
		foreach(SpriteRenderer renderer in spriteRendererList){
			renderer.sprite = null;
		}
	}

	void Update(){
		// Debug has been flipped on, do changes
		if(isDebug != isDebugInternal){
			if(isDebug){
				LoadAndSetColor("PurpleLime");	// Sample
			}
			else{
				ClearDebug();
			}
			isDebugInternal = isDebug;
		}
	}
	// For Salon Manager
	public void ChangeStyle(string colorName) {
		LoadAndSetColor(colorName);
	}

}
