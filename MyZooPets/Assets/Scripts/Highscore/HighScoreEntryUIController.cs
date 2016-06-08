using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class HighScoreEntryUIController : MonoBehaviour {
    public Text miniGameName;
    public Text miniGameHighScore;

    public void Init(string miniGameKey, int score){
        string titleKey = "HIGHSCORE_BOARD_" + miniGameKey;
        miniGameName.text = Localization.Localize(titleKey);
        miniGameHighScore.text = score.ToString();
    }
}
