using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Loops through all gameobjects for matches to the keyword tag and
/// enables all the objects that are stored in the respective animation states.
/// Doing so will diable the rest, so the animation is reset with all the body parts
/// that you want enabled and the ones that you dont want disabled.
/// 
/// NOTE: When disabling objects, only disable the renderer and NOT the gameObject itself!!
/// </summary>
public class LgResetAnimationNodes : MonoBehaviour {
	public GameObject parentObject;		// The parent that unity will traverse to find tags
	public string tagKeyword;			// The string to match in gameobject tags
	public LgAnimationState[] animationStatesList;

	private string lastState;
	private GameObject[] validTagList;
	private Dictionary<string, bool> validListCheck;

	void Start(){
		if(parentObject == null){
			parentObject = gameObject;
		}

		//ResetAnimation("test");
	}

	public void SetupList(string animationState){
		// Get list of all game objects that has the tag
		if(validTagList == null){
			//Debug.Log("Creating");
			validTagList = GameObject.FindGameObjectsWithTag(tagKeyword);
		}

		// Set up a hashtable to do the check with from animationStatesList
		if(animationState == lastState){		// Last hashtable already has this
			//Debug.Log("hash exists already");
			return;
		}
		else{								// Repopulate new list to check
			lastState = animationState;
			validListCheck = new Dictionary<string, bool>();	// Reset list
			bool errorCheck = true;
			foreach(LgAnimationState state in animationStatesList){
				if(state.stateName == animationState){
					errorCheck = false;
					foreach(GameObject go in state.gameObjectEnableList){
						validListCheck.Add(go.name, true);
					}
				}
			}

			if(errorCheck){
				Debug.LogError("No state in animatonStatesList found in animationStatesList!");
			}
		}
	}

	public void ResetAnimation(string animationState){
		SetupList(animationState);
		foreach(GameObject go in validTagList){
			if(go != null){
				SpriteRenderer spriteRenderer = go.GetComponent<SpriteRenderer>();
				if(spriteRenderer != null){
					// If the object in our list matches the hashtables' entries enable renderer, else disable
					spriteRenderer.enabled = (validListCheck.ContainsKey(go.name)) ? true : false;
				}
				else{
					Debug.LogError("No sprite renderer detected on " + go.name);
				}
			}
		}
	}

//	void OnGUI(){
//		if(GUI.Button(new Rect(100, 100, 100, 100), "test")){
//			ResetAnimation("Happy");
//		}
//		if(GUI.Button(new Rect(200, 100, 100, 100), "test2")){
//			ResetAnimation("test2");
//		}
//	}
}
