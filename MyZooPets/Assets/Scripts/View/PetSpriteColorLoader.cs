using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class PetSpriteColorLoader : MonoBehaviour {

	public string spriteSetPrefix;
	public SpriteRenderer[] spriteRendererList;
	public bool isDebug = false;
	private bool isDebugInternal = false;

	void Start(){
		if(Application.isPlaying){	// Exception for execute in edit mode
			if(isDebug){
				Debug.LogError("Debug for pet color is currently on! Make sure to uncheck!");
			}
			else{
				if(Application.loadedLevelName == "MenuScene"){
					LoadAndSetColor(DataManager.Instance.MenuSceneData.PetColor);
				}
				else{
					LoadAndSetColor(DataManager.Instance.GameData.PetInfo.PetColor);
				}
			}
		}
	}

	private void LoadAndSetColor(string petColor){
		Sprite[] sprites = Resources.LoadAll<Sprite>(spriteSetPrefix + petColor);

		// Loop through all the body parts that needs color assignment
		foreach(SpriteRenderer spriteRenderer in spriteRendererList){
			// Load their index metadata
			int atlasIndex = spriteRenderer.GetComponent<PetSpriteColorMetadata>().spriteSetIndexToLoad;
			// Set their sprites according to index from metadata
			spriteRenderer.sprite = sprites[atlasIndex];
		}
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
				LoadAndSetColor("OrangeYellow");	// Sample
			}
			else{
				ClearDebug();
			}
			isDebugInternal = isDebug;
		}
	}
}
