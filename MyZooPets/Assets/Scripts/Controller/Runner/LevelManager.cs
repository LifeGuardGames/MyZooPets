using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LevelManager : MonoBehaviour
{
	public GameObject StartingLevelComponent;
	public List<GameObject> LevelComponents;
	
	private Queue<GameObject> mLevelComponentQueue = new Queue<GameObject>();
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
		PushAndInstantiateRandomComponent();
		PushAndInstantiateRandomComponent();
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (mLevelComponentQueue.Count > 0)
		{
			// When the runner has run enough distance from the last position, it's time to kick it for a new one.
			GameObject runner = GameObject.FindGameObjectWithTag("Player");
			Vector3 currentRunnerPosition = runner.transform.position;
			GameObject frontLevelComponent = mLevelComponentQueue.Peek();
			
			Vector3 frontLevelPosition = frontLevelComponent.transform.position;
			// Different betwee the two positions
			float distanceBetween = Vector3.Distance(currentRunnerPosition, frontLevelPosition);
			
			//@TODO test if this actually is a good number.
			int zExtent = 2;
			float distanceToUpdateLevel = GetLengthWithChildren(frontLevelComponent, zExtent) * 2.0f;
			if (distanceBetween >= distanceToUpdateLevel)
			{
				// Dequeue the first
				GameObject removedLevelComponent = mLevelComponentQueue.Dequeue();
				// Destroy it
				GameObject.Destroy(removedLevelComponent);
				// Push a new one
				PushAndInstantiateRandomComponent();
			}
		}
	}
	
	private void PushAndInstantiateRandomComponent(GameObject inForceUseThisComponent = null)
	{
		if (LevelComponents.Count > 0)
		{
			GameObject nextlevelComponent = null;
			if (inForceUseThisComponent == null)
				nextlevelComponent = LevelComponents[Random.Range(0, LevelComponents.Count)];
			else 
			{
				Debug.Log("Pushing default");
				nextlevelComponent = inForceUseThisComponent;
			}
			
			GameObject newComponent = (GameObject)GameObject.Instantiate(nextlevelComponent);
			
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
