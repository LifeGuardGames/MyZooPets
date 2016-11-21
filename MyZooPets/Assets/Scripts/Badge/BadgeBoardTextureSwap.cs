using UnityEngine;
using System.Collections;

/// <summary>
/// Badge board texture swap.
/// This class takes the amount of badges that you have and shows the corrosponding texture
/// in the bedroom badge board to reflect its populated status
/// </summary>
public class BadgeBoardTextureSwap : MonoBehaviour {
	public Renderer badgeBoardTexture;

	private string texture1Name = "badgeboard1";
	private string texture2Name = "badgeboard2";
	private string texture3Name = "badgeboard3";

	private int badgeCount1Threshold = 10;
	private int badgeCount2Threshold = 24;

	void Start(){
		int badgeCount = BadgeManager.Instance.GetUnlockedBadgesCount();
		string selectedTexture = texture1Name;
		if(badgeCount < badgeCount1Threshold){
			selectedTexture = texture1Name;
		}
		else if(badgeCount < badgeCount2Threshold){
			selectedTexture = texture2Name;
		}
		else{
			selectedTexture = texture3Name;
		}
		Texture tex = Resources.Load(selectedTexture) as Texture;
		badgeBoardTexture.material.mainTexture = tex;
	}
}
