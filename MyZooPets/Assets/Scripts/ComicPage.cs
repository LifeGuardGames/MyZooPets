using UnityEngine;
using System;
using System.Collections.Generic;

public class ComicPage : MonoBehaviour{
	public string audioClipName;

	// For color swapping, support up to 3 for now
	public SpriteRenderer sprite1;
	public string sprite1Prefix;

	public SpriteRenderer sprite2;
	public string sprite2Prefix;

	public SpriteRenderer sprite3;
	public string sprite3Prefix;

	public SpriteRenderer sprite4;
	public string sprite4Prefix;

	public SpriteRenderer sprite5;
	public string sprite5Prefix;

	public TweenToggleDemux demux;
	public List<SpriteRenderer> allSpriteRenderers;

	/// <summary>
	/// Populate the comic pages with the appropriate color
	/// </summary>
	/// <param name="color">Color.</param>
	public void Init(string color){
		SwapColor((PetColor)Enum.Parse(typeof(PetColor), color));
	}

	/// <summary>
	/// Given the pet color name, find the sprites that needs to be swapped and swap them
	/// </summary>
	/// <param name="petColor">Pet color.</param>
	private void SwapColor(PetColor color){
		if(color == PetColor.OrangeYellow){
			return;	// This is the default sprite already
		}
		else{
			if(sprite1 != null){
				Sprite spriteLoaded = Resources.Load<Sprite>(sprite1Prefix + color.ToString());
				sprite1.sprite = spriteLoaded;
			}
			if(sprite2 != null){
				Sprite spriteLoaded = Resources.Load<Sprite>(sprite2Prefix + color.ToString());
				sprite2.sprite = spriteLoaded;
			}
			if(sprite3 != null){
				Sprite spriteLoaded = Resources.Load<Sprite>(sprite3Prefix + color.ToString());
				sprite3.sprite = spriteLoaded;
			}
			if(sprite4 != null){
				Sprite spriteLoaded = Resources.Load<Sprite>(sprite4Prefix + color.ToString());
				sprite4.sprite = spriteLoaded;
			}
			if(sprite5 != null){
				Sprite spriteLoaded = Resources.Load<Sprite>(sprite5Prefix + color.ToString());
				sprite5.sprite = spriteLoaded;
			}
		}
	}

	public void ToggleActive(bool isActive){
		foreach(SpriteRenderer renderer in allSpriteRenderers){
			renderer.enabled = isActive;
		}
		// Play if it is active
		if(isActive){
			demux.Hide();
		}
	}

    public void PlaySound(){
        if(!String.IsNullOrEmpty(audioClipName))
            AudioManager.Instance.PlayClip(audioClipName);
    }
}
