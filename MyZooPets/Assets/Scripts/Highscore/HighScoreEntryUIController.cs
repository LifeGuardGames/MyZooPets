using UnityEngine;
using System.Collections;

public class HighScoreEntryUIController : MonoBehaviour {
    public UILabel miniGameName;
    public UILabel miniGameHighScore;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void Init(string miniGameKey, int score){
        string titleKey = "HIGHSCORE_BOARD_" + miniGameKey;
        miniGameName.text = Localization.Localize(titleKey);
        miniGameHighScore.text = score.ToString();
    }
}
