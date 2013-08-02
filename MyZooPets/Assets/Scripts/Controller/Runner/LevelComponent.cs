using UnityEngine;
using System.Collections.Generic;

public class LevelComponent : MonoBehaviour
{
    private int mNextID = 0;
    private Dictionary<string, PointGroup> mPointGroups = new Dictionary<string, PointGroup>();

    public Dictionary<string, PointGroup> PointGroups
    {
        get { return mPointGroups; }
    }
    public int NextID
    {
        get;
        set;
    }

    public PointGroup GetGroup(string inID)
    {
        if (mPointGroups.ContainsKey(inID))
            return mPointGroups[inID];
        return null;
    }
    public void SetPointGroupInfo(string inID, PointGroup inGroup)
    {
        if (mPointGroups.ContainsKey(inID))
            mPointGroups[inID] = inGroup;
        else
            mPointGroups.Add(inID, inGroup);
    }

	// Use this for initialization
	void Start ()
    {
	
	}
	
	// Update is called once per frame
	void Update ()
    {
	
	}
}

public class PointGroup
{
    public string mID;
    public List<PointInfo> mPoints;
    public List<string> mGroups;
    public List<string> mPurposes;

    public PointGroup(string inID)
    {
        mID = inID;
        mPoints = new List<PointInfo>();
        mGroups = new List<string>();
        mPurposes = new List<string>();
    }
}

public class PointInfo
{
    public Vector3 mPosition;
    public eLineType mLineType;

    public PointInfo()
    {
        mPosition = new Vector3();
        mLineType = eLineType.Straight;
    }
}

public enum eLineType
{
    Straight,
    Curve,
    Bezier
}