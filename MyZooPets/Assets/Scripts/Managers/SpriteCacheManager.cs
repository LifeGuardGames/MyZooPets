﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// Custom loader to cache all the sprite data from images
public class SpriteCacheManager : Singleton<SpriteCacheManager> {
	public static Sprite GetHudTweenIcon(StatType statType) {
		switch(statType) {
			case StatType.Xp:
				return Resources.Load<Sprite>("IconStarBlank");
            case StatType.Health:
				return Resources.Load<Sprite>("IconHealthBlank");
			case StatType.Hunger:
				return Resources.Load<Sprite>("IconHungerBlank");
			case StatType.Coin:
				return Resources.Load<Sprite>("IconCoinBlank");
			default:
				Debug.LogError("No icons for " + statType.ToString());
				return null;
		}
	}

	public static Sprite GetBadgeSprite(string badgeID) {
		if(string.IsNullOrEmpty(badgeID)) {
			return Resources.Load<Sprite>("BadgeBlank");
		}
		else {
			return Resources.Load<Sprite>(DataLoaderBadges.GetData(badgeID).TextureName);
		}
	}

	public static Sprite GetSprite(string spriteName) {
		return Resources.Load<Sprite>(spriteName);
	}

}
