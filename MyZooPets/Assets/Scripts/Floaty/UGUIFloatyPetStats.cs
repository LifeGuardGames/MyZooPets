using UnityEngine.UI;

public class UGUIFloatyPetStats : UGUIFloaty {

	public Image[] imageList;
	public Text[] textList;

	public void Init(int deltaPoints, int deltaHealth, int deltaMood, int deltaStars) {
		// Turn everything off
		foreach(Image image in imageList) {
			image.enabled = false;
		}

		foreach(Text text in textList) {
			text.enabled = false;
		}

		int currentListIndex = 0;

		if(deltaPoints != 0) {
			string strDeltaPoints = (deltaPoints > 0) ? "+" + deltaPoints : deltaPoints.ToString();
			textList[currentListIndex].enabled = true;
			imageList[currentListIndex].enabled = true;
            textList[currentListIndex].text = strDeltaPoints;
			imageList[currentListIndex].sprite = SpriteCacheManager.GetSprite("IconStarBlank");
			currentListIndex++;
		}
		if(deltaHealth != 0) {
			string strDeltaHealth = (deltaHealth > 0) ? "+" + deltaHealth : deltaHealth.ToString();
			textList[currentListIndex].enabled = true;
			imageList[currentListIndex].enabled = true;
			textList[currentListIndex].text = strDeltaHealth;
			imageList[currentListIndex].sprite = SpriteCacheManager.GetSprite("IconHealthBlank");
			currentListIndex++;
		}
		if(deltaMood != 0) {
			string strDeltaMood = (deltaMood > 0) ? "+" + deltaMood : deltaMood.ToString();
			textList[currentListIndex].enabled = true;
			imageList[currentListIndex].enabled = true;
			textList[currentListIndex].text = strDeltaMood;
			imageList[currentListIndex].sprite = SpriteCacheManager.GetSprite("IconHungerBlank");
			currentListIndex++;
		}
		if(deltaStars != 0) {
			string strDeltaStars = (deltaStars > 0) ? "+" + deltaStars : deltaStars.ToString();
			textList[currentListIndex].enabled = true;
			imageList[currentListIndex].enabled = true;
			textList[currentListIndex].text = strDeltaStars;
			imageList[currentListIndex].sprite = SpriteCacheManager.GetSprite("IconCoinBlank");
			currentListIndex++;
		}
	}
}
