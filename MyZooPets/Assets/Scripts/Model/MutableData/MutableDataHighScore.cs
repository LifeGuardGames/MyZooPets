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

	// Version check
	public void UpdateHighScoreToNewVersion(){
		if(MinigameHighScore.ContainsKey("Runner")){
			if(!MinigameHighScore.ContainsKey("RUNNER")){
				MinigameHighScore.Add("RUNNER", MinigameHighScore["Runner"]);
			}
			MinigameHighScore.Remove("Runner");
		}
		if(MinigameHighScore.ContainsKey("Shooter")){
			if(!MinigameHighScore.ContainsKey("SHOOTER")){
				MinigameHighScore.Add("SHOOTER", MinigameHighScore["Shooter"]);
			}
			MinigameHighScore.Remove("Shooter");
		}
		if(MinigameHighScore.ContainsKey("Memory")){
			if(!MinigameHighScore.ContainsKey("MEMORY")){	
				MinigameHighScore.Add("MEMORY", MinigameHighScore["Memory"]);
			}
			MinigameHighScore.Remove("Memory");
		}
		if(MinigameHighScore.ContainsKey("Ninja")){
			if(!MinigameHighScore.ContainsKey("NINJA")){
				MinigameHighScore.Add("NINJA", MinigameHighScore["Ninja"]);
			}
			MinigameHighScore.Remove("Ninja");
		}
		if(MinigameHighScore.ContainsKey("Clinic")){
			if(!MinigameHighScore.ContainsKey("CLINIC")){		
				MinigameHighScore.Add("CLINIC", MinigameHighScore["Clinic"]);
			}
			MinigameHighScore.Remove("Clinic");
		}
	}
}
