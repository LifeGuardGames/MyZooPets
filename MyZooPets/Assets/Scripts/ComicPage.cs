using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class ComicPage : MonoBehaviour{
	public string clipName;

	// For color swapping, support up to 3 for now
	public SpriteRenderer sprite1;
	public string sprite1PrefixName;
	public SpriteRenderer sprite2;
	public string sprite2PrefixName;
	public SpriteRenderer sprite3;
	public string sprite3PrefixName;

	public TweenToggleDemux demux;
	public List<SpriteRenderer> allSpriteRenderers;
//	public LgButtonMessage buttonMessage;

	/// <summary>
	/// Populate the comic pages with the appropriate color
	/// </summary>
	/// <param name="color">Color.</param>
	public void Init(string color, ComicPlayer player){
//		switch(color){
//		case PetColor.OrangeYellow, PetColor.BlueYellow, PetColor.PurpleLime:
//			SwapColor(color.ToString());
//			break;
//		default:
//			Debug.LogError("Invalid color");
//		}

//		buttonMessage.target = player.gameObject;
//		buttonMessage.functionName = "NextPage";
		demux.HideTarget = player.gameObject;
		demux.HideFunctionName = "StartTransitionAndCallNextPage";
	}

	/// <summary>
	/// Given the pet color name, find the sprites that needs to be swapped and swap them
	/// </summary>
	/// <param name="petColor">Pet color.</param>
	private void SwapColor(string petColor){
		//TODO Swap here;
	}

	public void ToggleActive(bool isActive){
		foreach(SpriteRenderer renderer in allSpriteRenderers){
			renderer.enabled = isActive;
		}
		// Play is it is active
		if(isActive){
			Debug.Log("PLaying " + name + demux.IsShowing);
			demux.Hide();
//			StartCoroutine(StartHideNextFrame());
		}
	}
//
//	private IEnumerator StartHideNextFrame(){
//		yield return 0;
//		demux.Hide();
//	}

    public void PlaySound(){
        if(!String.IsNullOrEmpty(clipName))
            AudioManager.Instance.PlayClip(clipName);
    }
}
