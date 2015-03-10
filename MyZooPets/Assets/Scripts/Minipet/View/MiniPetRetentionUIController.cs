using UnityEngine;
using System.Collections;

public class MiniPetRetentionUIController : MonoBehaviour {

	public UILocalize missionLocalize;

	public void Initialize(string missionKey){
		missionLocalize.key = missionKey;
		missionLocalize.Localize();
	}
}
