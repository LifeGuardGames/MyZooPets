using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LevelManager : MonoBehaviour
{
	public GameObject CoinPrefab;
    public List<RunnerItem> ItemPrefabs;
    public List<RunnerItem> HazardPrefabs;

	public float CoinSpawnChance = 60.0f;
	public float CoinSpawnDistance = 1.0f;
	public float ItemSpawnChance = 10.0f;
	public float HazardSpawnChance = 10.0f;

	public float LevelTooLowYValue = -50.0f;

	public LevelComponent StartingLevelComponent;
	public List<LevelComponent> LevelComponents;

	private Queue<LevelComponent> mLevelComponentQueue = new Queue<LevelComponent>();
	private Vector3 lastCenterPosition = Vector3.zero;

	// Use this for initialization
	void Start ()
	{
		if (LevelComponents.Count <= 0)
			Debug.LogError("No level components found.");
		
		// @HACK shove 3 in there. @TODO Better way to do it w/ screen size or something..?
		PushAndInstantiateRandomComponent(StartingLevelComponent);
		PushAndInstantiateRandomComponent();
		PushAndInstantiateRandomComponent();
		LevelComponent nextLevel = PushAndInstantiateRandomComponent();
		PopulateLevelComponent(nextLevel);
		nextLevel = PushAndInstantiateRandomComponent();
		PopulateLevelComponent(nextLevel);
	}
	
	// Update is called once per frame
	void Update ()
	{
		// When the runner has run enough distance from the last position, it's time to kick it for a new one.
		GameObject runner = GameObject.FindGameObjectWithTag("Player");

		// Yknow, assuming there is a runner and  alevel.
		if (mLevelComponentQueue.Count > 0 && runner != null)
		{
			Vector3 currentRunnerPosition = runner.transform.position;
			LevelComponent frontLevelComponent = mLevelComponentQueue.Peek();

			//@TODO test if this actually is a good number.
			int zExtent = 2;

			Vector3 frontLevelPosition = frontLevelComponent.transform.position;
			// Different betwee the two positions
			float distanceBetween = currentRunnerPosition[zExtent] - frontLevelPosition[zExtent];
			
			float distanceToUpdateLevel = GetLengthWithChildren(frontLevelComponent.gameObject, zExtent) * 2.0f;
			if (distanceBetween >= distanceToUpdateLevel)
			{
				// Dequeue the first
				LevelComponent removedLevelComponent = mLevelComponentQueue.Dequeue();
				// Destroy it
				GameObject.Destroy(removedLevelComponent);
				// Push a new one
				LevelComponent nextLevel = PushAndInstantiateRandomComponent();
				PopulateLevelComponent(nextLevel);
			}
		}
	}

	private LevelComponent PushAndInstantiateRandomComponent(LevelComponent inForceUseThisComponent = null)
	{
		if (LevelComponents.Count > 0)
		{
			LevelComponent nextlevelComponent = null;
			if (inForceUseThisComponent == null)
				nextlevelComponent = LevelComponents[Random.Range(0, LevelComponents.Count)];
			else 
			{
				Debug.Log("Pushing default");
				nextlevelComponent = inForceUseThisComponent;
			}

			LevelComponent newComponent = (LevelComponent)GameObject.Instantiate(nextlevelComponent);
			
			// Set its position to the last max point (I hope).
			newComponent.transform.position = lastCenterPosition;
			
			// Get the min of the new component
			Transform minAnchor = newComponent.transform.FindChild("AnchorMin");
			// Determine the vector that we need to push the center by
			Vector3 pushVector = newComponent.transform.position - minAnchor.position;
			// And push it
			newComponent.transform.position += pushVector;
			
			// Update the next position as this ones max anchor
			Transform maxAnchor = newComponent.transform.FindChild("AnchorMax");
			lastCenterPosition = maxAnchor.position;
			
			mLevelComponentQueue.Enqueue(newComponent);

			return newComponent;
		}
		return null;
	}

	private void PopulateLevelComponent(LevelComponent inLevelComponent)
	{
		// Roll for some values
		bool bSpawnCoins = Random.Range(0.0f, 100.0f) <= CoinSpawnChance;
        bool bSpawnItems = Random.Range(0.0f, 100.0f) <= ItemSpawnChance;
        bool bSpawnHazards = Random.Range(0.0f, 100.0f) <= HazardSpawnChance;

		// Go through every group, and if we are set to spawn stuff, spawn stuff!
		if (bSpawnCoins)
		{
			foreach (PointGroup currentGroup in inLevelComponent.PointGroups)
			{
				if (currentGroup.mPurposes[(int)eSelectionTypes.Coins])
				{
					float interpolationLeftovers = 0;
					for (int pointIndex = 0; pointIndex < currentGroup.mPoints.Count - 1; pointIndex++)
					{
						Vector3 currentLineBegin = currentGroup.mPoints[pointIndex].mPosition;
						Vector3 currentLineEnd = currentGroup.mPoints[pointIndex + 1].mPosition;
						float currentLineDistance = Vector3.Distance(currentLineBegin, currentLineEnd);

						// Interpolate along our current line
						for (float currentInterpolation = interpolationLeftovers; currentInterpolation < currentLineDistance; currentInterpolation += CoinSpawnDistance)
						{
							// Find our new spawn point
							Vector3 newCoinPosition = Vector3.Lerp(currentLineBegin, currentLineEnd, (currentInterpolation / currentLineDistance));
							// But wait, that's on the prefab. Add in our real world clones position.
							newCoinPosition += (inLevelComponent.transform.position);

							GameObject newCoin = (GameObject)GameObject.Instantiate(CoinPrefab);
							newCoin.transform.position = newCoinPosition;

							interpolationLeftovers = currentInterpolation - currentLineDistance;
						}
					}
				}
			}
		}
		else if (bSpawnItems && ItemPrefabs.Count > 0)
		{
			// Determine which item to spawn
			GameObject chosenPrefab = ItemPrefabs[Random.Range(0, ItemPrefabs.Count)].gameObject;
            SpawnObjectAtRandomStartPointInLevel(inLevelComponent, chosenPrefab, eSelectionTypes.Items);
        }
        else if (bSpawnHazards && HazardPrefabs.Count > 0)
        {
            // Determine which hazard to spawn
            GameObject chosenPrefab = HazardPrefabs[Random.Range(0, HazardPrefabs.Count)].gameObject;
            SpawnObjectAtRandomStartPointInLevel(inLevelComponent, chosenPrefab, eSelectionTypes.Hazards);
        }
	}

    private void SpawnObjectAtRandomStartPointInLevel(LevelComponent inLevelComponent, GameObject inObjectToSpawn, eSelectionTypes inPurpose)
    {
        // Determine a random location of all given points
        List<Vector3> pointChoices = new List<Vector3>();
        foreach (PointGroup currentGroup in inLevelComponent.PointGroups)
        {
            if (currentGroup.mPoints.Count > 0
                && currentGroup.mPurposes[(int)inPurpose])
            {
                pointChoices.Add(currentGroup.mPoints[0].mPosition);
            }
        }

        if (pointChoices.Count > 0)
        {
            Vector3 randomPoint = pointChoices[Random.Range(0, pointChoices.Count)];
            randomPoint += inLevelComponent.transform.position;
            GameObject spawnedItem = (GameObject)GameObject.Instantiate(inObjectToSpawn);
            spawnedItem.transform.position = randomPoint;
        }
    }
	
	private float GetLengthWithChildren(GameObject inObjectToSearch, int inExtent)
	{
		Vector3 max = inObjectToSearch.transform.position;
		Vector3 min = inObjectToSearch.transform.position;
		GetMinMaxExtentsIncludingChildren(inObjectToSearch, inExtent, ref min, ref max);
		return (max[inExtent] - min[inExtent]);
	}
	
	private void GetMinMaxExtentsIncludingChildren(GameObject inObjectToSearch, int inExtent, ref Vector3 ioMinExtent, ref Vector3 ioMaxExtent)
	{
		if (inObjectToSearch.collider != null)
		{
			if (ioMinExtent == null
				|| inObjectToSearch.collider.bounds.min[inExtent] < ioMinExtent[inExtent])
				//|| Vector3.Min(inObjectToSearch.collider.bounds.min, ioMinExtent) == ioMinExtent)
				ioMinExtent = inObjectToSearch.collider.bounds.min;
			if (ioMaxExtent == null 
				|| inObjectToSearch.collider.bounds.max[inExtent] < ioMaxExtent[inExtent])
				//|| Vector3.Min(inObjectToSearch.collider.bounds.max, ioMaxExtent) == ioMaxExtent)
				ioMaxExtent = inObjectToSearch.collider.bounds.max;
		}
		
		foreach (Transform currentChild in inObjectToSearch.transform)
		{
			GetMinMaxExtentsIncludingChildren(currentChild.gameObject, inExtent, ref ioMinExtent, ref ioMaxExtent);
		}
	}
}
