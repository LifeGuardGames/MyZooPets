using UnityEngine;
using System;
using UnityEngine.UI;

/// <summary>
/// UI that appears over anything that is locked by level.
/// </summary>
public class LevelLockObject : MonoBehaviour {
	// elements of this UI
	public Image spriteIcon;
	public Text labelLevel;
	
	// level of this lock
	private int level;

	/// <summary>
	/// Creates the lock. and put it in the parent object
	/// </summary>
	/// <param name="parentObject">Parent object.</param>
	/// <param name="level">Level.</param>
	/// <param name="prefabName">Prefab name.</param>
	public static GameObject CreateLock(GameObject parentObject, int level, string prefabName = "LevelLockUI"){
		GameObject goPrefab = Resources.Load(prefabName) as GameObject;
		GameObject lockObject = GameObjectUtils.AddChild(parentObject, goPrefab);

		lockObject.GetComponent<LevelLockObject>().Init(level);
		return lockObject;
	}

	/// <summary>
	/// This function does the work and actually sets the UI lables, sprites, etc
	/// for this UI based on the incoming data.
	/// </summary>
	/// <param name="level">Level.</param>
	public void Init(int level){
		// if this lock breaks, it needs to listen for level up messages
		labelLevel.text = "" + level;		

		// set the proper values on the entry
		this.level = level;
		HUDAnimator.OnLevelUp += LevelUp;		
	}
	
	void OnDestroy(){
		// stop listening for callbacks
		HUDAnimator.OnLevelUp -= LevelUp;	
	}

	/// <summary>
	/// If this lock breaks, it listens to level up messages and destroys itself
	/// if appropriate.
	/// </summary>
	/// <param name="senders">Senders.</param>
	/// <param name="args">Arguments.</param>
	private void LevelUp(object senders, EventArgs args){
        int newLevel = (int) LevelLogic.Instance.CurrentLevel; 
		if (newLevel >= this.level){
			Destroy(gameObject);
		}
	}
}
