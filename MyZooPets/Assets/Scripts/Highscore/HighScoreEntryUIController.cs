using UnityEngine;
using System.Collections;

public class HighScoreEntryUIController : MonoBehaviour {
    public UILabel miniGameName;
    public UILabel miniGameHighScore;

    public void Init(string miniGameKey, int score){
        string titleKey = "HIGHSCORE_BOARD_" + miniGameKey;
        miniGameName.text = Localization.Localize(titleKey);
        miniGameHighScore.text = score.ToString();
    }
}
