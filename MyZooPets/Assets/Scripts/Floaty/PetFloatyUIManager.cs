using UnityEngine;

public class PetFloatyUIManager : Singleton<PetFloatyUIManager> {
	public Canvas petCanvas;
	public GameObject petStatsFloatyPrefab;

	/// <summary>
	/// Any floaties that has to do with the pets go here
	/// </summary>
	public void CreateStatsFloaty(int deltaPoints, int deltaHealth, int deltaMood, int deltaStars) {
		GameObject floaty = GameObjectUtils.AddChildGUI(petCanvas.gameObject, petStatsFloatyPrefab);

		UGUIFloatyPetStats floatyScript = floaty.GetComponent<UGUIFloatyPetStats>();
		floatyScript.Init(deltaPoints, deltaHealth, deltaMood, deltaStars);
		floatyScript.StartFloatyLocal(null, 1f, new Vector3(0, 100, 0));
	}
}
