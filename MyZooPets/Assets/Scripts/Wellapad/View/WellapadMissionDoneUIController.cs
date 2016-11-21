using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

//---------------------------------------------------
// WellapadCountdown
// This script is on the Wellapad and, when the user has completed all available missions/tasks and the
// "Done" screen is showing, this script will update the time remaining until new missions are available.
//---------------------------------------------------
public class WellapadMissionDoneUIController : MonoBehaviour {
	public GameObject gridUnlockPredictions;
	public GameObject unlockPredictionEntryPrefab;

	void Start(){
		//WellapadUIManager.Instance.OnManagerOpen += RefreshLevelProgress;
		// HUDAnimator.OnLevelUp += RefreshLevelProgressOnLevelUp;
		HUDAnimator.OnLevelUp += RefreshUnlockPredictions;

		//RefreshLevelProgress();
		RefreshUnlockPredictions(this, EventArgs.Empty);

		PlayPeriodLogic.OnNextPlayPeriod += OnNextPlayPeriod;
	}

	void OnDestroy(){
		//if(WellapadUIManager.Instance)
		//	WellapadUIManager.Instance.OnManagerOpen -= RefreshLevelProgress;
			
		// HUDAnimator.OnLevelUp -= RefreshLevelProgressOnLevelUp;
		HUDAnimator.OnLevelUp -= RefreshUnlockPredictions;

		PlayPeriodLogic.OnNextPlayPeriod -= OnNextPlayPeriod;
	}

	private void OnNextPlayPeriod(object sender, EventArgs args){
		WellapadMissionController.Instance.RefreshCheck();
	}

	//----------------------------------------------
	// RefreshUnlockPredictions()
	// Update the items/badge/flame that will be unlocked for next level
	//----------------------------------------------
	private void RefreshUnlockPredictions(object sender, EventArgs args){
		foreach(Transform child in gridUnlockPredictions.transform){
			child.gameObject.SetActive(false);
			Destroy(child.gameObject);
		}

		ImmutableDataBadge badge = BadgeManager.Instance.GetBadgeUnlockAtNextLevel();
		if(badge != null){
			GameObject go = GameObjectUtils.AddChildWithPositionAndScale(gridUnlockPredictions, unlockPredictionEntryPrefab);
			Image sprite = go.GetComponent<Image>();
			sprite.sprite = SpriteCacheManager.GetBadgeSprite(badge.TextureName);
		}

		ImmutableDataSkill skill = FlameLevelLogic.Instance.GetSkillUnlockAtNextLevel();
		if(skill != null){
			GameObject go = GameObjectUtils.AddChildWithPositionAndScale(gridUnlockPredictions, unlockPredictionEntryPrefab);
			Image sprite = go.GetComponent<Image>();
			//Place Holder
			sprite.sprite = SpriteCacheManager.GetSprite(skill.TextureName);
		}

		List<Item> items = ItemManager.Instance.GetItemsUnlockAtNextLevel();
		foreach(Item item in items){
			GameObject go = GameObjectUtils.AddChildWithPositionAndScale(gridUnlockPredictions, unlockPredictionEntryPrefab);
			Image sprite = go.GetComponent<Image>();
			sprite.sprite = SpriteCacheManager.GetSprite(item.TextureName);
		}
	}
}
