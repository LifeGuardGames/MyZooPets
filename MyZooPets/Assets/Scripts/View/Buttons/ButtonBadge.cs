﻿using UnityEngine;
using System.Collections;

//---------------------------------------------------
// ButtonBadge
// Button for individual badges on the badge board.
//---------------------------------------------------

public class ButtonBadge : LgButton{
	protected override void ProcessClick(){
		if(!BadgeBoardUIManager.Instance.IsBadgeBoardUIAnimating){
			BadgeBoardUIManager.Instance.BadgeClicked(gameObject);
		}
	}
}