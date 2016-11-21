using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Memory board controller.
/// This class instantiates the board of MemoryCards onto the game.
/// The reason why this class exists is for layout purposes.
/// In the future if we want to shuffle the card this is the place to do it.
/// </summary>
public class MemoryBoardController : MonoBehaviour{
	public static int ROW_COUNT = 4;
	public static int COLUMN_COUNT = 4;
	const float DISTANCE_UNIT = 150f;

	private List<GameObject> cardList = new List<GameObject>(); // Card array, there is 20 of them
	public GameObject memoryCardPrefab;

	public void ResetBoard(List<ImmutableDataMemoryTrigger> triggerList){
		foreach(GameObject go in cardList){
			Destroy(go);
		}
		cardList.Clear();

		foreach(ImmutableDataMemoryTrigger triggerData in triggerList){
			
			// Make the sprite card
			GameObject cardObject1 = Instantiate(memoryCardPrefab) as GameObject;
			cardObject1.transform.parent = transform;

			cardObject1.name = triggerData.Name + "Sprite";
			cardObject1.GetComponent<MemoryCard>().Initialize(triggerData, true);
			cardList.Add(cardObject1);

			// Make the word card
			GameObject cardObject2 = Instantiate(memoryCardPrefab) as GameObject;
			cardObject2.transform.parent = transform;

			cardObject2.name = triggerData.Name + "Word";
			cardObject2.GetComponent<MemoryCard>().Initialize(triggerData, false);
			cardList.Add(cardObject2);
		}
		cardList.Shuffle();
		for(int i=0; i<cardList.Count; i++){
			cardList[i].transform.localPosition = new Vector3(
					(i % COLUMN_COUNT) * DISTANCE_UNIT,
					-1f * (i / COLUMN_COUNT) * DISTANCE_UNIT,
					0f
				);
			cardList[i].transform.localScale = new Vector3(1, 1, 1);
		}
	}
}
