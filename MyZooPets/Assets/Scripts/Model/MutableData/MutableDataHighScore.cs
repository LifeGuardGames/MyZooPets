using System.Collections.Generic;

public class MutableDataHighScore{
	public Dictionary<string, int> MinigameHighScore { get; set; }		//Key: Minigame Key, Value: Minigame highscore

	public void UpdateMinigameHighScore(string miniGameKey, int highScore){
		//if exist a mini game high score compare them and store the highest one
		if(MinigameHighScore.ContainsKey(miniGameKey)){
			int serializedHighScore = MinigameHighScore[miniGameKey];
			if(highScore > serializedHighScore){
				MinigameHighScore[miniGameKey] = highScore;
			}
		}
		//if no mini game high score then add it
		else{
			MinigameHighScore.Add(miniGameKey, highScore);
		}
	}

	//============================Initialization===============================
	public MutableDataHighScore(){
		MinigameHighScore = new Dictionary<string, int>();
	}
}
