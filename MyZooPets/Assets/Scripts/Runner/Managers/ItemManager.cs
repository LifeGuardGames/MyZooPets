/* Sean Duane
 * ItemManager.cs
 * 8:27:2013   12:04
 * Description:
 * An item pool holds spawned items in storage to be re-used.
 * This cuts down on the runtime of creating/deleting gameobjcts a bit.
 * 
 * TODO:
 * This is a super simple implementation of a pool. There is much that could be improved if needed.
 */

using UnityEngine;
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

using Random = UnityEngine.Random;

public class ItemManager : MonoBehaviour {
    public int ItemPoolMaxSize = 50;

    public List<CoinItem> CoinItems;
    public List<RunnerItem> ItemPrefabs;
    public List<HazardItem> HazardPrefabs;

    //private List<RunnerItem> mItemPool = new List<RunnerItem>();
    private Dictionary<Type, Queue<RunnerItem>> mItemPool = new Dictionary<Type, Queue<RunnerItem>>();

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public RunnerItem GetRandomItemOfType(Type inItemType) {
        if (mItemPool.ContainsKey(inItemType) && mItemPool[inItemType].Count > 0) {
            return mItemPool[inItemType].Dequeue();
        } else {
            // Create a new item
            RunnerItem newItem = ItemFactory(inItemType);
            return newItem;
        }
    }

    public void StoreOrDisposeItem(RunnerItem inItem) {
        Type itemType = inItem.GetType();
        if (!mItemPool.ContainsKey(itemType)) {
            mItemPool.Add(itemType, new Queue<RunnerItem>());
        }

        if (mItemPool[itemType].Count < ItemPoolMaxSize) {
            mItemPool[itemType].Enqueue(inItem);
        } else
            GameObject.Destroy(inItem.gameObject);
    }

    private RunnerItem ItemFactory(Type inItemType) {
        if (inItemType == typeof(HazardItem)) {
            HazardItem randomHazard = HazardPrefabs[Random.Range(0, HazardPrefabs.Count)];
            HazardItem spawnedItem = (HazardItem)Instantiate(randomHazard);
            return spawnedItem;
        } else if (inItemType == typeof(CoinItem)) {
            CoinItem randomCoin = CoinItems[Random.Range(0, CoinItems.Count)];
            CoinItem spawnedItem = (CoinItem)Instantiate(randomCoin);
            return spawnedItem;
        } else if (inItemType == typeof(RunnerItem)) {
            RunnerItem randomItem = ItemPrefabs[Random.Range(0, ItemPrefabs.Count)];
            RunnerItem spawnedItem = (RunnerItem)Instantiate(randomItem);
            return spawnedItem;
        } else {
            Debug.LogError("No spawn logic set for item type " + inItemType);
        }
        return null;
    }
}
