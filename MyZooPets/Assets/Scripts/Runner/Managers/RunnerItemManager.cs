/* Sean Duane
 * ItemManager.cs
 * 8/27/2013   12:04
 * Description:
 * An item pool holds spawned items in storage to be re-used.
 * This cuts down on the runtime of creating/deleting gameobjcts a bit.
 * 
 * TODO:
 * This is a super simple implementation of a pool. There is much that could be improved if needed.
 */

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

using Random = UnityEngine.Random;

public class RunnerItemManager : Singleton<RunnerItemManager> {
    public int ItemPoolMaxSize = 50;
    public List<CoinItem> CoinItems;
    public List<RunnerItem> ItemPrefabs;
    public List<HazardItem> HazardPrefabs;
    public GameObject itemTutorialPrefab;
    public GameObject itemTutorialParent;
    public GameObject runnerItemsParent;

    //Item pool for Coin and Items
    private Dictionary<Type, Queue<RunnerItem>> mItemPool = new Dictionary<Type, Queue<RunnerItem>>();

    //Hazard Items are LevelGroup specific so a more specific item pool is required
    private Dictionary<LevelGroup.eLevelGroupID, Queue<HazardItem>> mHazardItemPool = 
        new Dictionary<LevelGroup.eLevelGroupID, Queue<HazardItem>>();

    //Key: LevelGroupID, Value: List of HazardItem specific to LevelGroup
    private Dictionary<LevelGroup.eLevelGroupID, List<HazardItem>> levelHazardItems = 
        new Dictionary<LevelGroup.eLevelGroupID, List<HazardItem>>();

	// Use this for initialization
	void Start () {
        //Sort hazard items into Levels that they belong to. Doing it here instead of in the 
        //inspector because we want to keep this game as modular as possible
        foreach(LevelGroup.eLevelGroupID levelGroupID in Enum.GetValues(typeof(LevelGroup.eLevelGroupID))){
            List<HazardItem> hazardItems = new List<HazardItem>();
            foreach(HazardItem hazardItem in HazardPrefabs){
                if(hazardItem.ApplicableGroup == levelGroupID){
                    hazardItems.Add(hazardItem);
                }
            }
            levelHazardItems.Add(levelGroupID, hazardItems);
        }
	}
	
    //Remove cached items when resetting 
    public void Reset(){
        if(mItemPool.ContainsKey(typeof(CoinItem))){
            Queue<RunnerItem> coinQueue = mItemPool[typeof(CoinItem)];
            while(coinQueue.Count > 0){
                print("destroy coin cache");
                RunnerItem coin = coinQueue.Dequeue();
                GameObject.Destroy(coin.gameObject);
            }
        }

        if(mItemPool.ContainsKey(typeof(RunnerItem))){
            Queue<RunnerItem> itemQueue = mItemPool[typeof(RunnerItem)];
            while(itemQueue.Count > 0){
                RunnerItem item = itemQueue.Dequeue();
                GameObject.Destroy(item.gameObject);
            }
        }
    }

    /*
        CurrentLevel is optional for CoinItem and RunnerItem. Required for Hazard Item because
        hazard item could be different for different levels
    */
    public RunnerItem GetRandomItemOfType(Type inItemType, LevelGroup.eLevelGroupID currentLevelGroup) {
        if(inItemType == typeof(HazardItem)){
            if(mHazardItemPool.ContainsKey(currentLevelGroup) && mHazardItemPool[currentLevelGroup].Count > 0){
				RunnerItem existingHazardItem = mHazardItemPool[currentLevelGroup].Dequeue();
				existingHazardItem.gameObject.SetActive(true);
                return existingHazardItem;
            }else{
                //Create new Hazard Item
                RunnerItem newItem = ItemFactory(inItemType, currentLevelGroup);
                return newItem;
            }
        }else{
            if (mItemPool.ContainsKey(inItemType) && mItemPool[inItemType].Count > 0) {
				RunnerItem existingPoolItem = mItemPool[inItemType].Dequeue();
				existingPoolItem.gameObject.SetActive(true);
                return existingPoolItem;
            } else {
                // Create a new item
                RunnerItem newItem = ItemFactory(inItemType, currentLevelGroup);
                return newItem;
            }
        }
    }

    //TO DO: need to fix caching bug
    public void StoreOrDisposeItem(RunnerItem inItem, LevelGroup.eLevelGroupID levelGroupID) {
		
		// Disable it. If its queued, then it will 'disapaear' off the map. If its deleted well who cares!
		if (inItem != null){
            inItem.gameObject.SetActive(false);
            Destroy(inItem.gameObject);
        }

        // Type itemType = inItem.GetType();

        // //HazardItem pool needs to be handled differently from the other items
        // if(itemType == typeof(HazardItem)){
        //    //  //Create cache queue
        //    // if(!mHazardItemPool.ContainsKey(levelGroupID))
        //    //      mHazardItemPool.Add(levelGroupID, new Queue<HazardItem>());

        //    // //Add to cache if cache size allows
        //    // if(mHazardItemPool[levelGroupID].Count < ItemPoolMaxSize)
        //    //      mHazardItemPool[levelGroupID].Enqueue((HazardItem)inItem);
        //    // else
        //         GameObject.Destroy(inItem.gameObject);
        // }else{
        //     //Create cache queue
        //     if (!mItemPool.ContainsKey(itemType))
        //         mItemPool.Add(itemType, new Queue<RunnerItem>());

        //    //Add to cache if cache size allows
        //     if (mItemPool[itemType].Count < ItemPoolMaxSize)
        //         mItemPool[itemType].Enqueue(inItem);
        //     else
        //         GameObject.Destroy(inItem.gameObject);
        // }
    }

    public void DisplayTutorial(string itemID, bool hazard){
        if(!DataManager.Instance.GameData.RunnerGame.RunnerItemCollided.Contains(itemID)){
            string hintMessage = Localization.Localize(itemID + "_HINT_MESSAGE");
            GameObject tutorialGO = NGUITools.AddChild(itemTutorialParent, itemTutorialPrefab);

            TutorialPopup.Callback button1Function = delegate(){
                Destroy(tutorialGO);
                StartCoroutine(ResumeGame());
				//PlayerController.Instance.StartCoroutine("FlashColor");
				if (hazard)
					PlayerController.Instance.FlashColor(Color.red);
					//StartCoroutine(PlayerController.Instance.FlashColor());
            };

            Hashtable option = new Hashtable();
            option.Add(TutorialPopupFields.Message, hintMessage);
            option.Add(TutorialPopupFields.Button1Callback, button1Function);

            TutorialPopup script = tutorialGO.GetComponent<TutorialPopup>();
            script.Init(option);

            RunnerGameManager.Instance.PauseGameWithoutPopup();
            DataManager.Instance.GameData.RunnerGame.RunnerItemCollided.Add(itemID);
        }
    }

    //Yield for a frame before unpausing the game. The only way to avoid OnTap being called when resume
    //button is clicked
    private IEnumerator ResumeGame(){
        yield return 0;
        RunnerGameManager.Instance.UnPauseGame();
    }

    private RunnerItem ItemFactory(Type inItemType, LevelGroup.eLevelGroupID levelGroupID) {
        RunnerItem spawnedItem = null;

        if (inItemType == typeof(HazardItem)) {
            List<HazardItem> hazardItems = levelHazardItems[levelGroupID];
            HazardItem randomHazard = hazardItems[Random.Range(0, hazardItems.Count)];
            spawnedItem = (HazardItem)Instantiate(randomHazard);
        } else if (inItemType == typeof(CoinItem)) {
            CoinItem randomCoin = CoinItems[Random.Range(0, CoinItems.Count)];
            spawnedItem = (CoinItem)Instantiate(randomCoin);
            
        } else if (inItemType == typeof(RunnerItem)) {
            RunnerItem randomItem = ItemPrefabs[Random.Range(0, ItemPrefabs.Count)];
            spawnedItem = (RunnerItem)Instantiate(randomItem);
            
        } else {
            Debug.LogError("No spawn logic set for item type " + inItemType);
        }
        
        spawnedItem.transform.parent = runnerItemsParent.transform;
        return spawnedItem;
    }
}
