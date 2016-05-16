/* Sean Duane
 * LevelGroup.cs
 * 8:26:2013   14:33
 * Description:
 * LevelGroups are the 'zones' that a player goes between e.g. Forest group, City group, Volcano group, etc.
 * They hold a (hopefully) unique ID like 'Forest' and a list of all components making up this group.
 * It also holds a parralaxing background to display while the player runs behind it.
 * Finally, we hold a LevelGroupNumber to signify it's spawn timing. Think of it as a difficulty. Whenever we transition a group,
 * we only transition one that is the current group number. Anything above that is ignored until later. Thus giving the player more
 * time until the transition can occur.
 */

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class LevelGroup : MonoBehaviour {
    public eLevelGroupID LevelGroupID;
    public int levelGroupDifficulty = 0; //spawning timing or difficulty. Higher number = more difficult
    // public LevelComponent startingLevelComponent; //the component to start the level group
    // public int startLevelComponentIndex;
    public ParallaxingBackgroundGroup parallaxBackgroundPrefab; 

    public List<GameObject> levelComponentsGO; //list of ready to use components
  
    //components currently in the scene. should not be spawned again
    //because we don't want duplicates in the cache
    private HashSet<string> componentsInScene = new HashSet<string>(); 

	/// <summary>
	/// Reset the cached components
	/// </summary>
	public void Reset(){
		componentsInScene = new HashSet<string>();
	}

	public string ReportDeath(){
		if(LevelGroupID == LevelGroup.eLevelGroupID.City){
			return "City";
		}
		else if (LevelGroupID == LevelGroup.eLevelGroupID.CityNight){
			return "CityNight";
		}
		else { 
			return "Forest";
		}	
	}

	/// <summary>
	/// Gets the tutorial level component. Usually the first component of the list
	/// </summary>
	/// <returns>The tutorial level component.</returns>
	public LevelComponent GetTutorialLevelComponent(){
		LevelComponent retVal = null;

		try{
			GameObject tutComponent = levelComponentsGO[0];
			GameObject newTutComponent = (GameObject) Instantiate(tutComponent);
			retVal = newTutComponent.GetComponent<LevelComponent>();
		}
		catch(ArgumentOutOfRangeException e){
			Debug.Log("Error message: " + e.Message);
		}
		catch(ArgumentNullException e){
			Debug.Log("Error message: " + e.Message);
		}


		return retVal;
	}

	/// <summary>
	/// Gets the start level component.
	/// </summary>
	/// <returns>The start level component.</returns>
    public LevelComponent GetStartLevelComponent(){
		LevelComponent retVal = null;

		retVal = GetComponent(0);

		return retVal;
    }

	/// <summary>
	/// Gets the component.
	/// </summary>
	/// <returns>level component</returns>
	/// <param name="index">arrya index</param>
	private LevelComponent GetComponent(int index){
		GameObject lvComponentObj = null;
		LevelComponent retVal = null;

		try{
			lvComponentObj = levelComponentsGO[index];
			
			if(componentsInScene.Contains(lvComponentObj.name)){
				lvComponentObj = levelComponentsGO.Find(component => componentsInScene.Contains(component.name) == false);
			}
			
			lvComponentObj.SetActive(true);
			
			if(lvComponentObj != null){
				componentsInScene.Add(lvComponentObj.name);
				retVal = lvComponentObj.GetComponent<LevelComponent>();
			}
		}
		catch(ArgumentOutOfRangeException e){
			Debug.Log("Error message: " + e.Message);
		}

		return retVal;
	}

	/// <summary>
	/// Gets the random component.
	/// </summary>
	/// <returns>The random component.</returns>
    public LevelComponent GetRandomComponent(){
		LevelComponent retVal = null;
		int randomIndex = UnityEngine.Random.Range(0, levelComponentsGO.Count); //get a random index

		retVal = GetComponent(randomIndex);

        return retVal; 
    }
	
    /// <summary>
	/// If GameObject is not in cache add it; otherwise, remove
    /// </summary>
    /// <param name="lvComponentObj">Lv component object.</param>
    public void DestroyAndCache(GameObject lvComponentObj){
        lvComponentObj.GetComponent<LevelComponent>().DestroyItems();
        componentsInScene.Remove(lvComponentObj.name);

		lvComponentObj.SetActive(false);

    }

    public enum eLevelGroupID{
        Forest,
        City,
        CityNight,
    }
}
