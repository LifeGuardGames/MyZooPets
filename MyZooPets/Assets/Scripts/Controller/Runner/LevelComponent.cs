using UnityEngine;
using System.Collections.Generic;

public class LevelComponent : MonoBehaviour
{
	private int mNextID = 0;
	[SerializeField]
    private List<PointGroup> mPointGroups = new List<PointGroup>();
    private List<GameObject> mSpawnedItems = new List<GameObject>();

	public List<PointGroup> PointGroups
	{
		get { return mPointGroups; }
	}
	public int NextID
	{
		get
        {
            return mNextID++;
        }
	}

	public PointGroup GetGroup(string inID)
	{
		foreach (PointGroup currentGroup in mPointGroups)
		{
			if (currentGroup.mID == inID)
				return currentGroup;
		}
		return null;
	}

	public int GetGroupIndex(string inID)
	{
		for (int pointIndex = 0; pointIndex < mPointGroups.Count; pointIndex++)
		{
			if (mPointGroups[pointIndex].mID == inID)
				return pointIndex;
		}
		return -1;
	}
	public void SetPointGroupInfo(string inID, PointGroup inGroup)
	{
		int existingGroupIndex = GetGroupIndex(inID);
		if (existingGroupIndex != -1)
			mPointGroups[existingGroupIndex] = inGroup;
		else
			mPointGroups.Add(inGroup);
	}
	public PointInfo AddNewPoint(string inID, Vector3 inNewPoint)
	{
		PointGroup currentGroup = GetGroup(inID);
		if (currentGroup != null)
		{
			PointInfo newPoint = new PointInfo(inNewPoint, eLineType.Straight);
			currentGroup.mPoints.Add(newPoint);
			return newPoint;
		}

		return null;
	}
	public void UpdatePointInfo(string inID, PointInfo inPointInfo, int inPointIndex)
	{
		PointGroup currentGroup = GetGroup(inID);
		if (currentGroup != null
			&& inPointIndex >= 0 && inPointIndex < currentGroup.mPoints.Count)
		{
			currentGroup.mPoints[inPointIndex] = inPointInfo;
		}
	}
	public int GetNextPointNum(string inID)
	{
		PointGroup currentGroup = GetGroup(inID);
		if (currentGroup != null)
			return currentGroup.mPoints.Count;
		return -1;
	}

	// Use this for initialization.
	void Start ()
	{
	
	}
	
	// Update is called once per frame.
	void Update ()
	{
	
	}
}

[System.Serializable]
public class PointGroup
{
	public string mID;
	public List<PointInfo> mPoints;
	public List<string> mGroups;
    public bool[] mPurposes;

	public PointGroup(string inID)
	{
		mID = inID;
		mPoints = new List<PointInfo>();
		mGroups = new List<string>();
        mPurposes = new bool[(int)eSelectionTypes.Max];
        for (int purposeIndex = 0; purposeIndex < (int)eSelectionTypes.Max; purposeIndex++)
        {
            mPurposes[purposeIndex] = false;
        }
	}
}

[System.Serializable]
public class PointInfo
{
	public Vector3 mPosition;
    public eLineType mLineType;
    public Vector3 mLocalPosition;

	public PointInfo()
	{
		mPosition = Vector3.zero;
		mLineType = eLineType.Straight;
	}

	public PointInfo(Vector3 inPosition, eLineType inLineType)
	{
		mPosition = inPosition;
		mLineType = inLineType;
	}
}

[System.Serializable]
public enum eLineType
{
	Straight,
	Curve,
	Bezier
}

[System.Serializable]
public enum eSelectionTypes
{
    None = -1,
    Coins = 0,
    Hazards,
    Items,
    Max
}