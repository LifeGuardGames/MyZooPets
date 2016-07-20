using UnityEngine;
using UnityEngine.UI;

public class CustomizationUIManager : Singleton<CustomizationUIManager> {
	public TweenToggle colorTweenParent;    // Part 1 of the selection process
	public TweenToggle nameTweenParent;     // Part 2 of the selection process
	public Text nameField;
	public ParticleSystemController leafParticle;
	public Animation requireNameAnimation;
	public Animation requireColorAnimation;
	public ParticleSystem poofParticle;
	public TweenToggle logoTitleTween;

	private string petColor = null;
	private string petName;					//Default pet name
	private Color currentRenderColor;

	public bool isHatchingPet = false;      // Aux used for click logic
	public bool isComicSpawned = false;
	private int hatchClicksCount = 7;

	public void ShowColorChooseUI() {
		logoTitleTween.Hide();

		EggController.Instance.ToggleEggIdleAnimation(false);
		EggController.Instance.ToggleEggCollider(false);
		EggController.Instance.EggCrack(1);
		leafParticle.Stop();

		// Show egg color choosing UI
		colorTweenParent.Show();
	}

	public void ChangeEggColor(string petColor) {
		poofParticle.Play();
		EggController.Instance.ChangeColor("egg" + petColor);
		this.petColor = petColor;
	}

	public void OnEggColorDoneButton() {
		if(petColor != null) {
			colorTweenParent.Hide();
			EggController.Instance.EggCrack(2);
		}
		else {
			requireColorAnimation.Play();
		}
	}

	// TweenToggle callback
	public void OnEggColorUIHideFinished() {
		ShowNameEntryUI();
    }

	/// <summary>
	/// Shows the second choose UI, called from first finish callback
	/// </summary>
	public void ShowNameEntryUI() {
		nameTweenParent.Show();
	}

	public void OnNameEntryDoneButton() {
		if(!string.IsNullOrEmpty(nameField.text)) {
			petName = nameField.text;
			nameTweenParent.Hide();

			Analytics.Instance.PetColorChosen(this.petColor);
			
			// Initialize data for new pet
			DataManager.Instance.ModifyBasicPetInfo(petName: petName, petSpecies: "Basic", petColor: this.petColor);
		}
		else {
			// Play the animation to prompt user to enter name
			requireNameAnimation.Play();
		}
	}

	// TweenToggle callback
	public void OnNameEntryUIHideFinished() {
		ShowHatchEggUI();
	}

	/// <summary>
	/// Shows the third choose UI, called from second finish callback
	/// </summary>
	public void ShowHatchEggUI() {
		isHatchingPet = true;
        EggController.Instance.ToggleEggCollider(true);
		EggController.Instance.ToggleFingerHint(true);
	}

	public void HatchEggTap() {
		EggController.Instance.EggHatchingTapped();
		hatchClicksCount--;
		if(hatchClicksCount <= 0 && !isComicSpawned) { // Hatch the pet
			isComicSpawned = true;
			EggController.Instance.ToggleEggCollider(false);
            MenuSceneManager.Instance.PlayMovie(petColor);
		}
	}
}
