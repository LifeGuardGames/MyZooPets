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
using System.Collections.Generic;

[System.Serializable]
public class LevelComponent : MonoBehaviour {
	[SerializeField]
    private List<PointGroup> mPointGroups = new List<PointGroup>();
    [SerializeField]
    private List<Bundle> mBundles = new List<Bundle>();

    private int mNextID = 0;
	private List<RunnerItem> mSpawnedItems = new List<RunnerItem>();

	public List<PointGroup> PointGroups { get { return mPointGroups; } }
    public List<Bundle> Bundles { get { return mBundles; } }
	public int NextID { get { return mNextID++; } }
	public LevelGroup ParentGroup {
		get;
		set;
	}

	// Use this for initialization.
	void Start() {
	}

	// Update is called once per frame.
	void Update() {
	}

	void OnDestroy() {
		foreach (RunnerItem currentItem in mSpawnedItems) {
			if (currentItem != null)
				GameObject.Destroy(currentItem.gameObject);
		}
	}

	public PointGroup GetGroup(string inID) {
		foreach (PointGroup currentGroup in mPointGroups) {
			if (currentGroup.mID == inID)
				return currentGroup;
		}
		return null;
	}

	public int GetGroupIndex(string inID) {
		for (int pointIndex = 0; pointIndex < mPointGroups.Count; pointIndex++) {
			if (mPointGroups[pointIndex].mID == inID)
				return pointIndex;
		}
		return -1;
	}

	public void SetPointGroupInfo(string inID, PointGroup inGroup) {
		int existingGroupIndex = GetGroupIndex(inID);
		if (existingGroupIndex != -1)
			mPointGroups[existingGroupIndex] = inGroup;
		else
			mPointGroups.Add(inGroup);
	}

	public PointInfo AddNewPoint(string inID, Vector3 inNewPoint) {
		PointGroup currentGroup = GetGroup(inID);
		if (currentGroup != null) {
			PointInfo newPoint = new PointInfo(inNewPoint, eLineType.Straight);
			currentGroup.mPoints.Add(newPoint);
			return newPoint;
		}

		return null;
	}

	public void UpdatePointInfo(string inGroupID, PointInfo inPointInfo, int inPointIndex) {
		PointGroup currentGroup = GetGroup(inGroupID);
		if (currentGroup != null
			&& inPointIndex >= 0 && inPointIndex < currentGroup.mPoints.Count)
		{
			currentGroup.mPoints[inPointIndex] = inPointInfo;
		}
		else if (inPointIndex != -1)
			Debug.LogError("Point for group ID " + inGroupID + " and point index " + inPointIndex + " does not exist!");
	}

	public int GetNextPointNum(string inID) {
		PointGroup currentGroup = GetGroup(inID);
		if (currentGroup != null)
			return currentGroup.mPoints.Count;
		return -1;
	}

	public void AddLevelItem(RunnerItem inItemToAdd) {
		mSpawnedItems.Add(inItemToAdd);
	}

	public void DeletePointGroup(PointGroup inGroupToDelete) {
		// Remove the group from our list
		if (mPointGroups.Contains(inGroupToDelete)) {
			mPointGroups.Remove(inGroupToDelete);
		}
	}

	public void DeletePointInfo(PointGroup inParentPointGroup, PointInfo inInfoToDelete) {
		if (mPointGroups.Contains(inParentPointGroup)
			&& inParentPointGroup.mPoints.Contains(inInfoToDelete))
		{
			inParentPointGroup.mPoints.Remove(inInfoToDelete);
		}
	}

    public Bundle GetBundle(int inBundleID) {
        foreach (Bundle currentBundle in mBundles) {
            if (currentBundle.mBundleID == inBundleID) {
                return currentBundle;
            }
        }
        return null;
    }

    public void RemoveBundle(int inBundleID) {
        foreach (Bundle currentBundle in mBundles) {
            if (currentBundle.mBundleID == inBundleID) {
                mBundles.Remove(currentBundle);
            }
        }
    }

    public void SetBundleChance(int inBundleID, float inChance) {
        // @Unity doesnt do dictionaries idk
        if (inBundleID < 0) {
            Debug.LogError("You can only use positive numbers as bundle IDs.");
            return;
        }

        Bundle bundleToModify = GetBundle(inBundleID);
        if (bundleToModify != null)
            bundleToModify.mSpawnChance = inChance;
        else {
            bundleToModify = new Bundle(inBundleID, inChance);
            mBundles.Add(bundleToModify);
        }
    }
}

[System.Serializable]
public class PointGroup {
	[SerializeField]
	public string mID;
	[SerializeField]
	public int mBundleID;
	[SerializeField]
	public List<PointInfo> mPoints;
	[SerializeField]
	public eSpawnType mSpawnType;

	public PointGroup(string inID) {
		mID = inID;
        mBundleID = 0;
		mPoints = new List<PointInfo>();
		mSpawnType = eSpawnType.None;
	}
}

[System.Serializable]
public class PointInfo {
	[SerializeField]
	public Vector3 mPosition;
	[SerializeField]
	public eLineType mLineType;
	[SerializeField]
	public Vector3 mLocalPosition;

	public PointInfo() {
		mPosition = Vector3.zero;
		mLineType = eLineType.Straight;
	}

	public PointInfo(Vector3 inPosition, eLineType inLineType) {
		mPosition = inPosition;
		mLineType = inLineType;
	}
}

[System.Serializable]
public class Bundle {
    [SerializeField]
    public int mBundleID;
    [SerializeField]
    public float mSpawnChance;

    public Bundle(int inID, float inChance) {
        mBundleID = inID;
        mSpawnChance = inChance;
    }
}

[System.Serializable]
public enum eLineType {
	Straight,
	Curve,
	Bezier
}

[System.Serializable]
public enum eSpawnType {
	None = -1,
	Coins = 0,
	Hazards,
	Items,
	Max
}