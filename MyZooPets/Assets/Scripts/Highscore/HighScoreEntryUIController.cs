using UnityEngine;
using UnityEngine.UI;

public class HighScoreEntryUIController : MonoBehaviour {
    //public Text miniGameName;
	public Image miniGameImage;
    public Text miniGameHighScore;

    public void Init(string miniGameKey, int score){
        string titleKey = "HIGHSCORE_BOARD_" + miniGameKey;
		miniGameImage.sprite = SpriteCacheManager.GetMinigameEntranceSprite(miniGameKey);
        //miniGameName.text = Localization.Localize(titleKey);
        miniGameHighScore.text = score.ToString();
    }
}
