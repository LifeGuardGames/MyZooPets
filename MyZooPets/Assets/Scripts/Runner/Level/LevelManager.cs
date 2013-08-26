/* Sean Duane
 * LevelManager.cs
 * 8:26:2013   12:00
 * Description:
 * Handles the pushing, popping, and management of all levelcomponents.
 * This works mainly off 'levelgroups'. These groups contain a level ID, and a list of components.
 * Every x distance, this manager will grab the current levelgroup, pull out a random component, and push that to the queue.
 * The queue get automatically destroyed and added as we run along it.
 * 
 * Level transitions occur every x seconds, and simply switch the current group to a random new group.
 * The new group cannot be the current group, and must be within a level range (mNumLevelSwitches)
 * 
 * Whenever we push a new level component, it will randomly spawn items on that level component.
 * This manager pulls from the new component all the "bundles" of items.
 * Based on each ones spawn chance, it rolls for a random one.
 * Then, we take that bundle, and spawn everything within it on that level component.
 */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LevelManager : MonoBehaviour
{
    public int BottomLayer = 31;
	public float LevelTooLowYValue = -50.0f;
	public float LevelGroupSwitchTime = 40.0f;
    public float CoinSpawnDistance = 1f;
    public CoinItem CoinPrefab;
    public LevelGroup StartingLevelGroup;
    public List<LevelGroup> LevelGroups;
    public List<LevelTransitionComponent> LevelTransitionGroups;
    public List<RunnerItem> ItemPrefabs;
    public List<RunnerItem> HazardPrefabs;

    private int mNumLevelSwitches;
    private float mLevelSwitchPulse;
    private Vector3 mLastCenterPosition;
    private LevelGroup mCurrentLevelGroup;
	private Queue<LevelComponent> mLevelComponentQueue = new Queue<LevelComponent>();

	// Use this for initialization
	void Start() {
		if (LevelGroups.Count <= 0)
			Debug.LogError("No level groups found.");

        Reset();
	}
	
	// Update is called once per frame
	void Update() {
		// Assuming there is a runner and a level.
		PlayerRunner playerRunner = RunnerGameManager.GetInstance().PlayerRunner;
		if (mLevelComponentQueue.Count > 0 && playerRunner != null) {
			Vector3 currentRunnerPosition = playerRunner.transform.position;
			LevelComponent frontLevelComponent = mLevelComponentQueue.Peek();

            Transform minAnchor = frontLevelComponent.transform.FindChild("AnchorMin");
            Vector3 frontLevelPosition = minAnchor.position;

            const int zExtent = 2;
			// Different between the two positions
			float distanceBetween = Mathf.Abs(currentRunnerPosition[zExtent] - frontLevelPosition[zExtent]);
			
			float distanceToUpdateLevel = GetLengthWithChildren(frontLevelComponent.gameObject, zExtent) * 2.0f;
            if (minAnchor.position.z < currentRunnerPosition.z && distanceBetween >= distanceToUpdateLevel) {
				// Dequeue the first
				LevelComponent removedLevelComponent = mLevelComponentQueue.Dequeue();
                // Find the new first
                LevelComponent newFront = mLevelComponentQueue.Peek();

                if (removedLevelComponent.ParentGroup.LevelID != newFront.ParentGroup.LevelID) {
                    ParallaxingBackgroundManager parralaxManager = RunnerGameManager.GetInstance().ParallaxingBackgroundManager;
                    parralaxManager.TransitionToGroup(newFront.ParentGroup.ParallaxingBackground.GroupID);
                }

				// Destroy it
				GameObject.Destroy(removedLevelComponent.gameObject);
				// Push a new one
				LevelComponent nextLevel = PushAndInstantiateRandomComponent();
				PopulateLevelComponent(nextLevel);
			}
		}

    	mLevelSwitchPulse -= Time.deltaTime / Time.timeScale;
		if (mLevelSwitchPulse <= 0f) {
            mLevelSwitchPulse = LevelGroupSwitchTime;

            TransitionToRandomNewLevelGroup();
		}
    }

    public void Reset() {
        // Reset to default values
        mLevelSwitchPulse = LevelGroupSwitchTime;
        mCurrentLevelGroup = StartingLevelGroup;
        mNumLevelSwitches = 0;

        // Clean out some other values
        mLastCenterPosition = Vector3.zero;

        // Clear out all components
        LevelComponent currentComponent;
        while (mLevelComponentQueue.Count > 0) { 
            currentComponent = mLevelComponentQueue.Dequeue();
            GameObject.Destroy(currentComponent.gameObject);
        }

        // @HACK shove some default components in there. @TODO Better way to do it w/ screen size or something..?
        PushAndInstantiateRandomComponent(StartingLevelGroup.StartingLevelComponent);
        PushAndInstantiateRandomComponent();
        LevelComponent nextLevel;
        nextLevel = PushAndInstantiateRandomComponent();
        PopulateLevelComponent(nextLevel);
        nextLevel = PushAndInstantiateRandomComponent();
        PopulateLevelComponent(nextLevel);
        nextLevel = PushAndInstantiateRandomComponent();
        PopulateLevelComponent(nextLevel);
    }

    private void TransitionToRandomNewLevelGroup() {
        // Cut to a new, different level that has the proper level ID.
        // Update our level switches
        mNumLevelSwitches++;

        // Pull out all levels that are 'legal' to switch to
        List<LevelGroup> potentialLevels = new List<LevelGroup>();
        foreach (LevelGroup levelGroup in LevelGroups) {
            if (levelGroup != mCurrentLevelGroup  // Don't switch to the current group
                && levelGroup.LevelGroupNumber <= mNumLevelSwitches) // Dont switch to a group that is a higher 'level' then us
            {
                potentialLevels.Add(levelGroup);
            }
        }

        if (potentialLevels.Count > 0) {
            // Choose a random level
            int randomIndex = Random.Range(0, potentialLevels.Count);
            LevelGroup newLevelGroup = potentialLevels[randomIndex];
            string currentGroupID = mCurrentLevelGroup.LevelID;
            string newGroupID = newLevelGroup.LevelID;

            // Transition!
            mCurrentLevelGroup = newLevelGroup;
            Debug.Log("Transitioning to level " + newLevelGroup.LevelID);

            // Now that we succesfully transitioned, determine if there is a level transition component.
            foreach (LevelTransitionComponent currentTransition in LevelTransitionGroups) {
                if (currentTransition.FromGroupID == currentGroupID
                    && currentTransition.ToGroupID == newGroupID) 
                {
                    // Push this component nowww
                    PushAndInstantiateRandomComponent(currentTransition);
                }
            }

        } else {
            Debug.LogError("No other levels to switch to. Perhaps there are no other level groups, or we havent reached a new "
                + "level number yet? Current number transitions: " + mNumLevelSwitches);
        }
    }

    public float GetTooLowYValue(Vector3 inPosition) {

        // Get the lowest component
        GameObject lowestComponent = GetLowestGameObjectOnField();
        float yTooLowValue = 0f;
        if (lowestComponent != null) {
            yTooLowValue = lowestComponent.collider.bounds.max.y + LevelTooLowYValue;
        }

        return yTooLowValue;
    }

	private LevelComponent PushAndInstantiateRandomComponent(LevelComponent inForceUseThisComponent = null) {
		if (mCurrentLevelGroup.LevelComponents.Count > 0) {
			LevelComponent nextlevelComponent = null;

			if (inForceUseThisComponent == null) {
				nextlevelComponent = mCurrentLevelGroup.LevelComponents[Random.Range(0, mCurrentLevelGroup.LevelComponents.Count)];
				Debug.Log("Pushing Next Level Component " + nextlevelComponent.name + " from group " + mCurrentLevelGroup.LevelID);
			} else {
				Debug.Log("Pushing default");
				nextlevelComponent = inForceUseThisComponent;
			}

			LevelComponent newComponent = (LevelComponent)GameObject.Instantiate(nextlevelComponent);
            newComponent.ParentGroup = mCurrentLevelGroup;
			
			// Set its position to the last max point (I hope).
			newComponent.transform.position = mLastCenterPosition;

			// Get the min of the new component
			Transform minAnchor = newComponent.transform.FindChild("AnchorMin");
			// Determine the vector that we need to push the center by
			Vector3 pushVector = newComponent.transform.position - minAnchor.position;
			// And push it
			newComponent.transform.position += pushVector;
			
			// Update the next position as this ones max anchor
			Transform maxAnchor = newComponent.transform.FindChild("AnchorMax");
			mLastCenterPosition = maxAnchor.position;
			
			mLevelComponentQueue.Enqueue(newComponent);

			return newComponent;
		}
		return null;
	}

	private void PopulateLevelComponent(LevelComponent inLevelComponent) {
        // See what groups we could spawn.
        List<Bundle> possibleSpawns = new List<Bundle>();
        foreach (Bundle currentBundle in inLevelComponent.Bundles) {
            float spawnChance = currentBundle.mSpawnChance;
            // Roll the dice
            bool bSpawnBundle = (Random.value < (spawnChance / 100f));
            if (bSpawnBundle)
                possibleSpawns.Add(currentBundle);
        }

        if (possibleSpawns.Count > 0) {
            // Choose a random bundle
            Bundle chosenBundle = possibleSpawns[Random.Range(0, possibleSpawns.Count)];

            //@OPTIMIZE cache this out in a dictionary when we generate the group. prefab. somehow. Then just pull from the dictionary.
            List<PointGroup> bundlePointGroups = new List<PointGroup>();
		    foreach (PointGroup currentGroup in inLevelComponent.PointGroups) {
                if (currentGroup.mBundleID == chosenBundle.mBundleID)
                    bundlePointGroups.Add(currentGroup);
            }

            // Phew. Now spawn all those itams! 
            foreach (PointGroup spawningGroup in bundlePointGroups) {
                SpawnItemsInLevel(inLevelComponent, spawningGroup);
            }
        }

	}

    private void SpawnItemsInLevel(LevelComponent inLevelComponent, PointGroup inGroup) {
        switch (inGroup.mSpawnType) {
            case eSpawnType.Coins: {
                CoinItem newCoin = CoinPrefab;
                SpawnCoinStrip(inLevelComponent, inGroup, newCoin);
                break;
            }

            case eSpawnType.Hazards: {
                HazardItem newHazard = (HazardItem)HazardPrefabs[Random.Range(0, HazardPrefabs.Count)];
                SpawnitemtAtRandomPointInGroup(inLevelComponent, inGroup, newHazard);
                break;
            }

            case eSpawnType.Items: {
                RunnerItem newItem = HazardPrefabs[Random.Range(0, HazardPrefabs.Count)];
                SpawnitemtAtRandomPointInGroup(inLevelComponent, inGroup, newItem);
                break;
            }
        }
    }

    private void SpawnCoinStrip(LevelComponent inLevelComponent, PointGroup inSpawnGroup, CoinItem inCoinPrefab) {
        float interpolationLeftovers = 0;
	    for (int pointIndex = 0; pointIndex < inSpawnGroup.mPoints.Count - 1; pointIndex++) {
		    Vector3 currentLineBegin = inSpawnGroup.mPoints[pointIndex].mPosition;
		    Vector3 currentLineEnd = inSpawnGroup.mPoints[pointIndex + 1].mPosition;
		    float currentLineDistance = Vector3.Distance(currentLineBegin, currentLineEnd);

		    // Interpolate along our current line
		    for (float currentInterpolation = interpolationLeftovers; currentInterpolation < currentLineDistance; currentInterpolation += CoinSpawnDistance) {
			    // Find our new spawn point
			    Vector3 newCoinPosition = Vector3.Lerp(currentLineBegin, currentLineEnd, (currentInterpolation / currentLineDistance));
			    // But wait, that's on the prefab. Add in our real world clones position.
			    newCoinPosition += (inLevelComponent.transform.position);

                GameObject newCoin = (GameObject)GameObject.Instantiate(inCoinPrefab.gameObject);
			    newCoin.transform.position = newCoinPosition;

                inLevelComponent.AddLevelItem(newCoin.GetComponent<RunnerItem>());

			    interpolationLeftovers = currentInterpolation - currentLineDistance;
		    }
	    }
    }

	private void SpawnitemtAtRandomPointInGroup(LevelComponent inLevelComponent, PointGroup inSpawnGroup, RunnerItem inItemToSpawn) {
		// Determine a random location of all given points
        if (inSpawnGroup.mPoints.Count > 0) {
            Vector3 randomPoint = inSpawnGroup.mPoints[Random.Range(0, inSpawnGroup.mPoints.Count)].mPosition;
			randomPoint += inLevelComponent.transform.position;
            GameObject spawnedItem = (GameObject)GameObject.Instantiate(inItemToSpawn);
			spawnedItem.transform.position = randomPoint;

            // Spawn it at that point
			inLevelComponent.AddLevelItem(spawnedItem.GetComponent<RunnerItem>());
		}
	}
	
	private float GetLengthWithChildren(GameObject inObjectToSearch, int inExtent) {
		Vector3 max = inObjectToSearch.transform.position;
		Vector3 min = inObjectToSearch.transform.position;
		GetMinMaxExtentsIncludingChildren(inObjectToSearch, inExtent, ref min, ref max);
		return (max[inExtent] - min[inExtent]);
	}
	
	private void GetMinMaxExtentsIncludingChildren(GameObject inObjectToSearch, int inExtent, ref Vector3 ioMinExtent, ref Vector3 ioMaxExtent) {
		if (inObjectToSearch.collider != null) {
			if (ioMinExtent == null
				|| inObjectToSearch.collider.bounds.min[inExtent] < ioMinExtent[inExtent])
				ioMinExtent = inObjectToSearch.collider.bounds.min;
			if (ioMaxExtent == null 
				|| inObjectToSearch.collider.bounds.max[inExtent] > ioMaxExtent[inExtent])
				ioMaxExtent = inObjectToSearch.collider.bounds.max;
		}
		
		foreach (Transform currentChild in inObjectToSearch.transform) {
			GetMinMaxExtentsIncludingChildren(currentChild.gameObject, inExtent, ref ioMinExtent, ref ioMaxExtent);
		}
	}

    private GameObject GetLowestGameObjectOnField() {
        GameObject lowestComponent = null;
        foreach (LevelComponent currentComponent in mLevelComponentQueue) {
            foreach (Transform currentLevelPiece in currentComponent.transform) {
                if (lowestComponent == null
                    || currentLevelPiece.position.y < lowestComponent.transform.position.y)
                {
                    lowestComponent = currentLevelPiece.gameObject;
                }
            }
        }
        return lowestComponent;
    }
}
