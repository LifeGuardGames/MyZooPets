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
	public float LevelTooLowYValueGameOver = -80.0f;
	public float LevelGroupSwitchTime = 40.0f;
    public float CoinSpawnDistance = 1f;
    public LevelGroup StartingLevelGroup;
    public List<LevelGroup> LevelGroups;
    public List<LevelTransitionComponent> LevelTransitionGroups;

    private int mNumLevelSwitches;
    private float mLevelSwitchPulse;
    private Vector3 mLastCenterPosition;
    private LevelGroup mCurrentLevelGroup;
	private Queue<LevelComponent> mLevelComponentQueue = new Queue<LevelComponent>();
	private Dictionary<Vector3, GameObject> mBetweenInvinciblePlatforms = new Dictionary<Vector3, GameObject>();

    private LevelComponent lastQueuedComponent;

	// Use this for initialization
	void Start() {
		if (LevelGroups.Count <= 0)
			Debug.LogError("No level groups found.");

        Reset();
	}
	
	// Update is called once per frame
	void Update() {
		if ( !RunnerGameManager.GetInstance().GameRunning )
			return;
		
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

                if (removedLevelComponent.ParentGroup.LevelGroupID != newFront.ParentGroup.LevelGroupID) {
                    ParallaxingBackgroundManager parralaxManager = RunnerGameManager.GetInstance().ParallaxingBackgroundManager;
                    parralaxManager.TransitionToGroup(newFront.ParentGroup.ParallaxingBackground.GroupID);
                }

				// Destroy it
                removedLevelComponent.DestroyAndCache();

				// GameObject.Destroy(removedLevelComponent.gameObject);
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
		
		UpdateInvincibility();
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
            currentComponent.DestroyAndCache();
        }

        LevelComponent nextLevel;
        // @HACK shove some default components in there. @TODO Better way to do it w/ screen size or something..?
        nextLevel = PushAndInstantiateRandomComponent(StartingLevelGroup.StartingLevelComponent);
        PopulateLevelComponent(nextLevel);
        //PopulateLevelComponent(PushAndInstantiateRandomComponent(StartingLevelGroup.StartingLevelComponent));
        // PushAndInstantiateRandomComponent(StartingLevelGroup.StartingLevelComponent);
        nextLevel = PushAndInstantiateRandomComponent();
        PopulateLevelComponent(nextLevel);
        nextLevel = PushAndInstantiateRandomComponent();
        PopulateLevelComponent(nextLevel);
        nextLevel = PushAndInstantiateRandomComponent();
        PopulateLevelComponent(nextLevel);
    }
	
	//@HACK there is a lot of room for optimization here. IF it needs to be.
	// The bottom layers can be cached out
	// The order of them can be cached out and manag
	// The function can be handled through messages, not glued to the player. That removes the need for constant updates, only update when needed!
	private void UpdateInvincibility() {
		PlayerRunner player = RunnerGameManager.GetInstance().PlayerRunner;
		if (player.Invincible) {
			Debug.Log("asfafas");
			// Get all lowest components
			List<GameObject> allLowestObjects = new List<GameObject>();
			foreach (LevelComponent currentComponent in mLevelComponentQueue) {
				List<GameObject> currentLowestObjects = currentComponent.BottomLayers;
				allLowestObjects.AddRange(currentLowestObjects);
			}
			
			if (allLowestObjects.Count > 1) {
				// Sort
				allLowestObjects.Sort(delegate(GameObject g1, GameObject g2) {
					if (g1.transform.position.z < g2.transform.position.z)
						return -1;
					else if (g1.transform.position.z > g2.transform.position.z)
						return 1;
					else
						return 0;
				});
				
				// Attempt to spawn items between it all
				for (int lowestIndex = 1; lowestIndex < allLowestObjects.Count; lowestIndex++) {
					GameObject leftObject = allLowestObjects[lowestIndex - 1];
					GameObject rightObject = allLowestObjects[lowestIndex];
					Vector3 leftObjectRightPoint = leftObject.collider.ClosestPointOnBounds(rightObject.collider.bounds.max);
					Vector3 rightObjectRightPoint = rightObject.collider.ClosestPointOnBounds(leftObject.collider.bounds.max);
					
					// Vector that goes between the points
					Vector3 betweenVector = rightObjectRightPoint - leftObjectRightPoint;
					Vector3 midPoint = leftObjectRightPoint + (betweenVector * 0.5f);
					
					if (!mBetweenInvinciblePlatforms.ContainsKey(midPoint)) {
						// No object exists, create one
						GameObject betweenPlatform = GameObject.CreatePrimitive(PrimitiveType.Cube);
						// Position
						betweenPlatform.transform.position = midPoint;
						
						// Scale
						//BoxCollider collider = (BoxCollider)betweenPlatform.collider;
						//Vector3 colliderSize = collider.size;
						//colliderSize.z = betweenVector.magnitude;
						//collider.size = colliderSize;
						Vector3 scale = betweenPlatform.transform.localScale;
						scale.z = betweenVector.magnitude;
						betweenPlatform.transform.localScale = scale;
						
						// Angle
						float angleBetween = Vector3.Angle(betweenVector, Vector3.forward);
						Quaternion newAngle = new Quaternion();
						newAngle.eulerAngles = new Vector3(angleBetween, 0f, 0f);
						betweenPlatform.transform.rotation = newAngle;
						
						// Add to the list
						mBetweenInvinciblePlatforms[midPoint] = betweenPlatform;
					}
				}
			}
		} else {
			foreach (GameObject currentObject in mBetweenInvinciblePlatforms.Values) {
				GameObject.Destroy(currentObject);
			}
			mBetweenInvinciblePlatforms.Clear();
		}
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
            LevelGroup.eLevelGroupID currentGroupID = mCurrentLevelGroup.LevelGroupID;
            LevelGroup.eLevelGroupID newGroupID = newLevelGroup.LevelGroupID;

            // Transition!
            mCurrentLevelGroup = newLevelGroup;
            Debug.Log("Transitioning to level " + newLevelGroup.LevelGroupID);

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
                //Only queue new component that is different from the previous ona
                while(true){
                    int randomIndex = Random.Range(0, mCurrentLevelGroup.LevelComponents.Count);

                    nextlevelComponent = mCurrentLevelGroup.LevelComponents[randomIndex];
                    if(lastQueuedComponent.name != nextlevelComponent.name) break;
                }
				Debug.Log("Pushing Next Level Component " + nextlevelComponent.name + " from group " + mCurrentLevelGroup.LevelGroupID);
			} else {
				Debug.Log("Pushing default");
				nextlevelComponent = inForceUseThisComponent;
			}


			LevelComponent newComponent = (LevelComponent)GameObject.Instantiate(nextlevelComponent);
            newComponent.ParentGroup = mCurrentLevelGroup;
            newComponent.name = nextlevelComponent.name;
			
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
            lastQueuedComponent = newComponent;

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
        ItemManager itemManager = RunnerGameManager.GetInstance().ItemManager;
        switch (inGroup.mSpawnType) {
            case eSpawnType.Coins: {
                SpawnCoinStrip(inLevelComponent, inGroup);
                break;
            }

            case eSpawnType.Hazards: {
                HazardItem newHazard = (HazardItem)itemManager.GetRandomItemOfType(typeof(HazardItem), mCurrentLevelGroup.LevelGroupID);
                SpawnitemtAtRandomPointInGroup(inLevelComponent, inGroup, newHazard);
                break;
            }

            case eSpawnType.Items: {
                RunnerItem newItem = (RunnerItem)itemManager.GetRandomItemOfType(typeof(RunnerItem), mCurrentLevelGroup.LevelGroupID);
                SpawnitemtAtRandomPointInGroup(inLevelComponent, inGroup, newItem);
                break;
            }
        }
    }

    private void SpawnCoinStrip(LevelComponent inLevelComponent, PointGroup inSpawnGroup) {
        ItemManager itemManager = RunnerGameManager.GetInstance().ItemManager;
        switch (inSpawnGroup.mCurveType) {
            case eCurveType.Point:
            case eCurveType.Linear: {
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
                        newCoinPosition += inLevelComponent.transform.position;

                        CoinItem newCoin = (CoinItem)itemManager.GetRandomItemOfType(typeof(CoinItem), mCurrentLevelGroup.LevelGroupID);
                        newCoin.transform.position = newCoinPosition;

                        inLevelComponent.AddLevelItem(newCoin);

                        interpolationLeftovers = currentInterpolation - currentLineDistance;
                    }
                }
            }
            break;

            case eCurveType.Quadratic:
            case eCurveType.Cubic: {
                float lineLength = CalculateCurveLength(inSpawnGroup);
                if (lineLength > 0f) {
                    // Set up some variables.
                    Vector3 ptA = inSpawnGroup.mPoints[0].mPosition;
                    Vector3 ptB = inSpawnGroup.mPoints[1].mPosition;
                    Vector3 ptC = inSpawnGroup.mPoints[2].mPosition;
                    Vector3 ptD = Vector3.zero;
                    if (inSpawnGroup.mPoints.Count > 3)
                        ptD = inSpawnGroup.mPoints[3].mPosition;

                    // Iterate the line length.
                    for (float currentPosition = 0f; currentPosition < lineLength; currentPosition += CoinSpawnDistance) {
                        // Determine current t
                        float currentT = currentPosition / lineLength;
                        // Get the new position
                        Vector3 coinSpawnLocation;
                        if (inSpawnGroup.mCurveType == eCurveType.Quadratic)
                            coinSpawnLocation = CalculateQuadtraticPoint(currentT, ptA, ptB, ptC);
                        else
                            coinSpawnLocation = CalculateBezierPoint(currentT, ptA, ptB, ptC, ptD);

                        coinSpawnLocation += inLevelComponent.transform.position;
                        // And spawn
                        CoinItem newCoin = (CoinItem)itemManager.GetRandomItemOfType(typeof(CoinItem), mCurrentLevelGroup.LevelGroupID);
                        newCoin.transform.position = coinSpawnLocation;
                        inLevelComponent.AddLevelItem(newCoin);
                    }
                }
            }
            break;
        }
    }

	private void SpawnitemtAtRandomPointInGroup(LevelComponent inLevelComponent, PointGroup inSpawnGroup, RunnerItem inItemToSpawn) {
        RunnerItem spawnedItem = (RunnerItem)GameObject.Instantiate(inItemToSpawn);
        Vector3 newPosition = inLevelComponent.transform.position;
        switch (inSpawnGroup.mCurveType) {
            case eCurveType.Point: {
                if (inSpawnGroup.mPoints.Count > 0)
                    newPosition += inSpawnGroup.mPoints[0].mPosition;
                else
                    Debug.LogError("No point for the line type point?");
            }
            break;

            case eCurveType.Linear: {
                if (inSpawnGroup.mPoints.Count > 0) {
                    //Vector3 randomPoint = inSpawnGroup.mPoints[Random.Range(0, inSpawnGroup.mPoints.Count)].mPosition;
                    newPosition += inSpawnGroup.mPoints[0].mPosition;
                } else
                    Debug.LogError("No points for the line type linear");
            }
            break;

            case eCurveType.Quadratic: {
                if (inSpawnGroup.mPoints.Count > 3) {
                    float randomT = Random.value;
                    Vector3 retrievedCurvePoint = CalculateQuadtraticPoint(randomT,
                        inSpawnGroup.mPoints[0].mPosition,
                        inSpawnGroup.mPoints[1].mPosition,
                        inSpawnGroup.mPoints[2].mPosition);
                    newPosition += retrievedCurvePoint;
                } else
                    Debug.LogError("only " + inSpawnGroup.mPoints.Count + " points for quad curve when I need 3");
            }
            break;

            case eCurveType.Cubic: {
                if (inSpawnGroup.mPoints.Count > 4) {
                    float randomT = Random.value;
                    Vector3 retrievedCurvePoint = CalculateBezierPoint(randomT,
                        inSpawnGroup.mPoints[0].mPosition,
                        inSpawnGroup.mPoints[1].mPosition,
                        inSpawnGroup.mPoints[2].mPosition,
                        inSpawnGroup.mPoints[3].mPosition);
                    newPosition += retrievedCurvePoint;
                } else
                    Debug.LogError("only " + inSpawnGroup.mPoints.Count + " points for cubic curve when I need 4");
            }
            break;
        }
        spawnedItem.transform.position = newPosition;

        // Spawn it at that point
        inLevelComponent.AddLevelItem(spawnedItem);
	}
	
	private float GetLengthWithChildren(GameObject inObjectToSearch, int inExtent) {
		Vector3 max = inObjectToSearch.transform.position;
		Vector3 min = inObjectToSearch.transform.position;
		GetMinMaxExtentsIncludingChildren(inObjectToSearch, inExtent, ref min, ref max);
		return (max[inExtent] - min[inExtent]);
	}
	
	private void GetMinMaxExtentsIncludingChildren(GameObject inObjectToSearch, int inExtent, ref Vector3 ioMinExtent, ref Vector3 ioMaxExtent) {		
		if (inObjectToSearch.collider != null) {
			// there used to be a check in these two ifs for ioMinExtend != null -- but this check is pointless.  What was it really
			// supposed to be checking?			
			if (inObjectToSearch.collider.bounds.min[inExtent] < ioMinExtent[inExtent])
				ioMinExtent = inObjectToSearch.collider.bounds.min;
			if (inObjectToSearch.collider.bounds.max[inExtent] > ioMaxExtent[inExtent])
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
                //TO DO: this is a temporary fix to ignore tile map that doesn't have
                //a collider
                if(currentLevelPiece.name != "TileMap" && 
                    currentLevelPiece.name != "TileMap Render Data"){

                    if (lowestComponent == null
                        || currentLevelPiece.position.y < lowestComponent.transform.position.y){

                        lowestComponent = currentLevelPiece.gameObject;
                    }
                }
            }
        }
        return lowestComponent;
    }
    
    // Quadtratic(t) = (s^2)A + 2(st)B + (t^2)C where s=1-t
    static public Vector3 CalculateQuadtraticPoint(float inTime, Vector3 inA, Vector3 inB, Vector3 inC) {
        float inverseTime = 1f - inTime;
        float cubeInverse = inverseTime * inverseTime;
        float timeAndInverse = inTime * inverseTime;
        float cubeTime = inTime * inTime;

        Vector3 termA = cubeInverse * inA;
        Vector3 termB = 2f * timeAndInverse * inB;
        Vector3 termC = cubeTime * inC;

        return termA + termB + termC;
    }


    // Function taken from:
    // http://devmag.org.za/2011/04/05/bzier-curves-a-tutorial/
    // It's blending in the 4 points, and spitting out the one point of all the blends.
    static public Vector3 CalculateBezierPoint(float inTime, Vector3 inA, Vector3 inB, Vector3 inC, Vector3 inD) {
        float u = 1f - inTime;
        float tt = inTime * inTime;
        float uu = u*u;
        float uuu = uu * u;
        float ttt = tt * inTime;
 
        Vector3 p = uuu * inA; //first term
        p += 3 * uu * inTime * inB; //second term
        p += 3 * u * tt * inC; //third term
        p += ttt * inD; //fourth term
 
        return p;
    }

    static public float CalculateCurveLength(PointGroup inSpawnGroup) {
        if (inSpawnGroup.mCurveType == eCurveType.Quadratic && inSpawnGroup.mPoints.Count < 3) {
            Debug.LogError("Not enough points, fix it!");
            return -1f;
        }
        if (inSpawnGroup.mCurveType == eCurveType.Cubic && inSpawnGroup.mPoints.Count < 4) {
            Debug.LogError("Not enough points, fix it!");
            return -1f;
        }
        // Uhh.. Determine the lenght?
        Vector3 ptA = inSpawnGroup.mPoints[0].mPosition;
        Vector3 ptB = inSpawnGroup.mPoints[1].mPosition;
        Vector3 ptC = inSpawnGroup.mPoints[2].mPosition;
        Vector3 ptD = Vector3.zero;
        if (inSpawnGroup.mPoints.Count > 3)
            ptD = inSpawnGroup.mPoints[3].mPosition;

        float notExactLengthOfCurve = 0.0f;

        bool bPastFirst = false;
        Vector3 lastDTVector = Vector3.zero;
        for (int dt = 0; dt < 100; dt++) {
            float currentDT = (float)dt / 100f;
            Vector3 currentDTVector;

            if (inSpawnGroup.mCurveType == eCurveType.Quadratic)
                currentDTVector = CalculateQuadtraticPoint(currentDT, ptA, ptB, ptC);
            else
                currentDTVector = CalculateBezierPoint(currentDT, ptA, ptB, ptC, ptD);

            if (bPastFirst) {
                notExactLengthOfCurve += Vector3.Distance(currentDTVector, lastDTVector);
            } else
                bPastFirst = true;
            lastDTVector = currentDTVector;
        }

        return notExactLengthOfCurve;
    }
}
