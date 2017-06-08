using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MutableDataMiniGames {
	public int chanceOfMinigameMode{ get; set; }
	public MiniGameModes mode { get; set; }
	public string minGame { get; set; }
	
	public void Init() {
		chanceOfMinigameMode = 0;
		mode = MiniGameModes.None;
		minGame = "";
	}
}
