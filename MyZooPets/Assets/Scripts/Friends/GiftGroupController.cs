﻿using UnityEngine;
using System;
using System.Collections;

public class GiftGroupController : MonoBehaviour {
	public UISprite gift1;
	public UISprite gift2;
	public UISprite gift3;
	public UISprite gift4;
	public ParticleSystem giftParticle;
	public Animation giftAnimation;
	public GameObject floatyParent;
	public UILabel addLabel;

	private int giftCountAux;
	private const string FullSpriteName = "friendsStarFull";
	private const string EmptySpriteName = "friendsStarBlank";

	public void Refresh(int starCount, int giftCount){
		addLabel.text = String.Format(Localization.Localize("FRIENDS_GIFT_GROUP"), 4 - (starCount % 4));

		if(starCount == 0 && giftCount != 0){
			gift1.spriteName = FullSpriteName;
			gift2.spriteName = FullSpriteName;
			gift3.spriteName = FullSpriteName;
			gift4.spriteName = FullSpriteName;
		}
		else if(starCount == 0 && giftCount == 0){
			gift1.spriteName = EmptySpriteName;
			gift2.spriteName = EmptySpriteName;
			gift3.spriteName = EmptySpriteName;
			gift4.spriteName = EmptySpriteName;
		}
		else if(starCount == 1){
			gift1.spriteName = FullSpriteName;
			gift2.spriteName = EmptySpriteName;
			gift3.spriteName = EmptySpriteName;
			gift4.spriteName = EmptySpriteName;
		}
		else if(starCount == 2){
			gift1.spriteName = FullSpriteName;
			gift2.spriteName = FullSpriteName;
			gift3.spriteName = EmptySpriteName;
			gift4.spriteName = EmptySpriteName;
		}
		else if(starCount == 3){
			gift1.spriteName = FullSpriteName;
			gift2.spriteName = FullSpriteName;
			gift3.spriteName = FullSpriteName;
			gift4.spriteName = EmptySpriteName;
		}

		if(giftCount != 0){
			ClaimReward(giftCount);
		}
	}

	public void ClaimReward(int giftCount){
		giftCountAux = giftCount;
		giftAnimation.Play();
	}

	public void PlayParticleEvent(){
		if(giftParticle != null){
			giftParticle.Play();
		}
	}

	public void SpawnFloatyEvent(){
		// Empty the stars
		gift1.spriteName = EmptySpriteName;
		gift2.spriteName = EmptySpriteName;
		gift3.spriteName = EmptySpriteName;
		gift4.spriteName = EmptySpriteName;
		
		// Reward Gems all together in one go
		Debug.LogError("REWARD WAS GEMS");
//		StatsController.Instance.ChangeStats(deltaGems:(2 * giftCountAux));
	}
}
