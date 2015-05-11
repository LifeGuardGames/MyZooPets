using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class CustomizationUIManager:SingletonUI<CustomizationUIManager>{
	public TweenToggle colorTweenParent;	// Part 1 of the selection process
	public TweenToggle nameTweenParent;		// Part 2 of the selection process
    public UILabel nameField;
	public Camera NGUICamera;
	public ParticleSystemController leafParticle;
	public Animation requireNameAnimation;
	public Animation requireColorAnimation;
	public ParticleSystem poofParticle;
	public TweenToggle logoTitleTween;
	public bool canClickHatchPet = false;

    private string petColor = null;
    private string petName; //Default pet name
    private Color currentRenderColor;

	private int hatchClicksCount = 7;
	
    protected override void Awake(){
		base.Awake();
        eModeType = UIModeTypes.CustomizePet;
    }
	
	protected override void _OpenUI(){
		logoTitleTween.Hide();
        ShowFirstChooseUI();
	}
	
	// Used when pressing back button in the panel
	protected override void _CloseUI(){
		nameTweenParent.Hide();
	}
    
    public void ChangeEggColor(string spriteName, string petColor){
		poofParticle.Play();
		EggController.Instance.ChangeColor(spriteName);
		this.petColor = petColor;     
    }

	private void ShowFirstChooseUI(){
		EggController.Instance.ToggleEggIdleAnimation(false);
		EggController.Instance.EggCrack(1);
		leafParticle.Stop();

		colorTweenParent.Show();
	}

	public void FirstFinishClicked(){
		if(petColor != null){
			colorTweenParent.Hide();
			EggController.Instance.EggCrack(2);
		}
		else{
			requireColorAnimation.Play();
		}
	}

	/// <summary>
	/// Shows the second choose UI, called from first finish callback
	/// </summary>
	public void ShowSecondChooseUI(){
		nameTweenParent.Show();
	}

	public void SecondFinishClicked(){
		if(!String.IsNullOrEmpty(nameField.text)){
			petName = nameField.text;
			CloseUI();

			Analytics.Instance.PetColorChosen(this.petColor);

			
			// Initialize data for new pet
			DataManager.Instance.ModifyBasicPetInfo(petName:petName, petSpecies:"Basic", petColor:this.petColor);
		}
		else{
			// Play the animation to prompt user to enter name
			requireNameAnimation.Play();
		}
	}

	/// <summary>
	/// Shows the third choose UI, called from second finish callback
	/// </summary>
	public void ShowThirdChooseUI(){
		canClickHatchPet = true;

		// Show the finger pointer here
		EggController.Instance.ToggleFingerHint(true);
	}

	public void ThirdUIEggTapped(){
		EggController.Instance.EggHatchingTapped();
		hatchClicksCount--;
		if(hatchClicksCount <= 0){	// Hatch the pet
			SelectionUIManager.instance.PlayMovie(petColor);
		}
	}
}
