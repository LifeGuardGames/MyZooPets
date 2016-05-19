using UnityEngine;
using System;
using System.Collections.Generic;

public class InventoryItemStatsHintController : MonoBehaviour{
    public GameObject inventoryStatsHintPrefab;

    private List<GameObject> statsHints = new List<GameObject>();
    private float yPositionOfFirstHint = 50.0f; //the local position of the first hint

    //-----------------------------------------------------------------    
    // PopulateStatsHints()
    // Create the stats hints that show the user how the item will affect
    // the pet's stats
    //-----------------------------------------------------------------    
    public void PopulateStatsHints(StatsItem statsItem){
        Dictionary<StatType, int> statsDict = statsItem.Stats;
        int hintCounter = 1;

        foreach(KeyValuePair<StatType, int> stat in statsDict){
            string spriteName = "";
            int statEffect = stat.Value;

            switch(stat.Key){
                case StatType.Hunger:
                    spriteName = "iconHunger";
                break;
                case StatType.Health:
                    spriteName = "iconHeart";
                break;
				case StatType.Fire:
					spriteName = "iconFire";
				break;
            }

            //instantiate the prefab
            GameObject statHint = GameObjectUtils.AddChildWithPositionAndScale(this.gameObject, inventoryStatsHintPrefab);
            statHint.transform.localPosition = new Vector3(statHint.transform.localPosition.x, 
                                                        yPositionOfFirstHint * hintCounter,
                                                        statHint.transform.localPosition.z);

            //set value to UI element
            string modifier = statEffect > 0 ? "+" : "";
            statHint.transform.Find("Label").GetComponent<UILabel>().text = modifier + statEffect;
            statHint.transform.Find("Sprite").GetComponent<UISprite>().spriteName = spriteName;

            statHint.SetActive(false);
            statsHints.Add(statHint);

            hintCounter++;
        }
    }

    //-----------------------------------------------------------------    
    // OnItemPress()
    // turn on the stats hints when the item is pressed
    //-----------------------------------------------------------------    
    public void OnItemDrag(object sender, EventArgs args){
        foreach(GameObject hint in statsHints){
            hint.SetActive(true);
        }
    }

    //-----------------------------------------------------------------    
    // OnItemDrop()
    // turn off the stats hints when the item is dropped
    //-----------------------------------------------------------------    
    public void OnItemDrop(object sender, InventoryDragDrop.InvDragDropArgs e){
        foreach(GameObject hint in statsHints){
            hint.SetActive(false);
        }
    }
}