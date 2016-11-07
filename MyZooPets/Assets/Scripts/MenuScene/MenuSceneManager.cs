using UnityEngine;
using System;
using System.Collections;

public class MenuSceneManager : Singleton<MenuSceneManager> {

    public MutableDataPetInfo PetMenuInfo{
        get{ return DataManager.Instance.GameData.PetInfo;}
    }

	public bool IsFirstTime{
		get{ return DataManager.Instance.IsFirstTime;}
	}

	public GameObject PetSelectionParent;

	/// <summary>
	/// Gets or sets the selected pet.
	/// </summary>
	/// <value>The selected pet.</value>
	public GameObject SelectedPet { get; set; }

	void Awake() {
		Input.multiTouchEnabled = false;
	}

	void Start() {
		// Remove old data
		foreach(Transform petSelectionTransform in PetSelectionParent.transform) {
			Destroy(petSelectionTransform.gameObject);
		}

		// Choose whether you see egg or pet. Only eggs for now (loading screen skips to bedroom)
		if(!PetMenuInfo.IsHatched) {
			GameObject menuSceneEggPrefab = Resources.Load("MenuSceneEgg") as GameObject;
			GameObject menuSceneEggGO = GameObjectUtils.AddChildWithPositionAndScale(PetSelectionParent, menuSceneEggPrefab);

			menuSceneEggGO.name = "MenuSceneEgg";
			menuSceneEggGO.GetComponent<EggController>().Init();
		}
		else {
			GameObject menuScenePetPrefab = Resources.Load("MenuScenePet") as GameObject;
			GameObject menuScenePetGO = GameObjectUtils.AddChildWithPositionAndScale(PetSelectionParent, menuScenePetPrefab);

			menuScenePetGO.name = "MenuScenePet";
			TextMesh petNameLabel = menuScenePetGO.transform.Find("Label_PetName").GetComponent<TextMesh>();
			petNameLabel.text = PetMenuInfo.PetName;

			menuScenePetGO.GetComponent<LgWorldButtonMessage>().target = this.gameObject;
			menuScenePetGO.GetComponent<LgWorldButtonMessage>().functionName = "PetSelected";
		}
	}

	public void PetSelected(GameObject selectedPetGO) {
		SelectedPet = selectedPetGO;

		if(!PetMenuInfo.IsHatched) {
			//Open CustomizationUIManager to create/initiate new pet game data
			CustomizationUIManager.Instance.ShowColorChooseUI();
		}
		else {
			//open up pet start panel
			AudioManager.Instance.PlayClip("petStart");
			LoadToBedroom();
		}
	}

	#region Cutscene movie playing code
	public void PlayMovie(string petColor) {
		StartCoroutine(ShowIntroMovie(petColor));
	}

	private IEnumerator ShowIntroMovie(string petColor) {
		yield return new WaitForSeconds(.3f);

		if(DataManager.Instance.GameData.Cutscenes.ListViewed.Contains("Comic_Intro")) {
			LoadToBedroom();
		}

		GameObject comicPlayerPrefab = Resources.Load("IntroComicPlayer") as GameObject;
		GameObject goComicPlayer = GameObjectUtils.AddChildWithPositionAndScale(null, comicPlayerPrefab) as GameObject;
		goComicPlayer.GetComponent<ComicPlayer>().Init(petColor);
		ComicPlayer.OnComicPlayerDone += IntroComicDone;

		// Play the comic music
		AudioManager.Instance.FadeOutPlayNewBackground("bgComic", isLoop: false);
	}

	private void IntroComicDone(object sender, EventArgs args) {
		DataManager.Instance.GameData.Cutscenes.ListViewed.Add("Comic_Intro");
		ComicPlayer.OnComicPlayerDone -= IntroComicDone;
		Analytics.Instance.StartGame();
		LoadToBedroom();
	}
	#endregion

	public void LoadToBedroom() {
		LoadLevelManager.Instance.StartLoadTransition(SceneUtils.BEDROOM);
	}
}
