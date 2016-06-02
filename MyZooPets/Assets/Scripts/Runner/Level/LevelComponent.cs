/* Sean Duane
 * LevelComponent.cs
 * 8:26:2013   14:25
 * Description:
 * The peices of the levels, these Components define 'sections' of a random level component.
 * They are simply a bunch of colliders and art that are grouped together as one component.
 * As the level progresses, a random component is chosen and spawned. These are generally not very large. 
 * 
 * Components are, as I said, made up of whatever it's children objects are.
 * As well, they contain the Point and Bundle information. This is the data we use for spawning items.
 * The pointgroups are the collection of positions, item spawn types, and the kind of line. (As of this writing its just Lines. But they could be curves, or whatever.)
 * PointGroups are spawned based off a Bundle and that bundles chance. Thus components also keep a list of bundles, their IDS, and the chances for each.
 *
 * When a component is created, and when it's destroyed, it brings the items with it. The items are spawned and store din here. On destroy, we
 * flush those items out.
 * 
 * Besides that... Almost everything here is for the level editor to work more easily with.
 */
using UnityEngine;
using System;
using System.Collections.Generic;

#pragma warning disable 0168

[System.Serializable]
public class LevelComponent : MonoBehaviour{
	[SerializeField]
	private List<PointGroup> mPointGroups = new List<PointGroup>();
	[SerializeField]
	private List<Bundle> mBundles = new List<Bundle>();
	private List<RunnerItem> mSpawnedItems = new List<RunnerItem>(); //all the spawned items in this component
	

	public List<PointGroup> PointGroups { get { return mPointGroups; } }
	public List<Bundle> Bundles { get { return mBundles; } }
	public LevelGroup ParentGroup { get; set; }

	//Destroy all the items for this component
	public void DestroyItems(){
		RunnerItemManager runnerItemManager = RunnerItemManager.Instance; 
		foreach(RunnerItem currentItem in mSpawnedItems){
			if(currentItem != null){
				runnerItemManager.StoreOrDisposeItem(currentItem, ParentGroup.LevelGroupID);
			}
		}
	}
	
	//Destroys this level component and all its items
    public void Destroy(){
    	DestroyItems();
		this.gameObject.SetActive(false);
        
    }

	public PointGroup GetGroup(int index){
		PointGroup pointGroup = null;
		try{
			pointGroup = mPointGroups[index];
		} catch(ArgumentOutOfRangeException e){
			pointGroup = null;
		}
		return pointGroup;
	}

	public void SetPointGroupInfo(PointGroup inGroup){
		mPointGroups.Add(inGroup);
	}

	public PointInfo AddNewPoint(int groupIndex, Vector3 inNewPoint){
		PointGroup currentGroup = GetGroup(groupIndex);
		return AddNewPoint(currentGroup, inNewPoint);
	}

	public PointInfo AddNewPoint(PointGroup currentGroup, Vector3 inNewPoint){
		if(currentGroup != null){
			PointInfo newPoint = new PointInfo(inNewPoint);
			currentGroup.mPoints.Add(newPoint);
			return newPoint;
		}
		return null;
	}

	public int GetNextPointNum(int groupIndex){
		PointGroup currentGroup = GetGroup(groupIndex);
		return GetNextPointNum(currentGroup);
	}

	public int GetNextPointNum(PointGroup currentGroup){
		if(currentGroup != null)
			return currentGroup.mPoints.Count;
		return -1;
	}

	public void AddLevelItem(RunnerItem inItemToAdd){
		mSpawnedItems.Add(inItemToAdd);
	}

	public void DeletePointGroup(PointGroup inGroupToDelete){
		// Remove the group from our list
		if(mPointGroups.Contains(inGroupToDelete)){
			mPointGroups.Remove(inGroupToDelete);
		}
	}

	public void DeletePointInfo(PointGroup inParentPointGroup, PointInfo inInfoToDelete){
		if(mPointGroups.Contains(inParentPointGroup)
			&& inParentPointGroup.mPoints.Contains(inInfoToDelete)){
			inParentPointGroup.mPoints.Remove(inInfoToDelete);
		}
	}

	public Bundle GetBundle(int inBundleID){
		foreach(Bundle currentBundle in mBundles){
			if(currentBundle.mBundleID == inBundleID){
				return currentBundle;
			}
		}
		return null;
	}

	public void RemoveBundle(int inBundleID){
		foreach(Bundle currentBundle in mBundles){
			if(currentBundle.mBundleID == inBundleID){
				mBundles.Remove(currentBundle);
			}
		}
	}

	public void SetBundleChance(int inBundleID, float inChance){
		// @Unity doesnt do dictionaries idk
		if(inBundleID < 0){
			Debug.LogError("You can only use positive numbers as bundle IDs.");
			return;
		}

		Bundle bundleToModify = GetBundle(inBundleID);
		if(bundleToModify != null)
			bundleToModify.mSpawnChance = inChance;
		else{
			bundleToModify = new Bundle(inBundleID, inChance);
			mBundles.Add(bundleToModify);
		}
	}

}

//====================================================================
// Classes use to specify how items within a component should be spawn
//====================================================================

[System.Serializable]
public class PointGroup{
	[SerializeField]
	public int
		mBundleID;
	[SerializeField]
	public int
		mSpawnChance;
	[SerializeField]
	public List<PointInfo>
		mPoints;
	[SerializeField]
	public eSpawnType
		mSpawnType;
	[SerializeField]
	public eCurveType
		mCurveType;

	public PointGroup(){
		// mID = inID;
		mBundleID = 0;
		mSpawnChance = 100;
		mPoints = new List<PointInfo>();
		mSpawnType = eSpawnType.None;
		mCurveType = eCurveType.Linear;
	}
}

[System.Serializable]
public class PointInfo{
	[SerializeField]
	public Vector3
		mPosition;
	[SerializeField]
	public Vector3
		mLocalPosition;

	public PointInfo(){
		mPosition = Vector3.zero;
	}

	public PointInfo(Vector3 inPosition){
		mPosition = inPosition;
	}
}

[System.Serializable]
public class Bundle{
	[SerializeField]
	public int
		mBundleID;
	[SerializeField]
	public float
		mSpawnChance;

	public Bundle(int inID, float inChance){
		mBundleID = inID;
		mSpawnChance = inChance;
	}
}

[System.Serializable]
public enum eCurveType{
	Point,
	Linear,
	Quadratic,
	Cubic,

	Max
}

[System.Serializable]
public enum eSpawnType{
	None = -1,
	Coins = 0,
	Hazards,
	Items,

	Max
}