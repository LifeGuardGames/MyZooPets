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
using System.Collections;
using System.Collections.Generic;

public class LevelGroup : MonoBehaviour {
    public eLevelGroupID LevelGroupID;
    public int LevelGroupNumber = 0;
    public ParallaxingBackgroundGroup ParallaxingBackground;
    public LevelComponent StartingLevelComponent;
    public List<GameObject> LevelComponentPrefabs; //list of prefab reference

    public Dictionary<string, GameObject> componentCache = new Dictionary<string, GameObject>();


    public LevelComponent GetRandomComponent(){
        int randomIndex = Random.Range(0, LevelComponentPrefabs.Count);

        //get the prefab
        GameObject lvComponentPrefab = LevelComponentPrefabs[randomIndex];

        //check for it in the cache
        GameObject lvComponentObj = null;
        if(componentCache.ContainsKey(lvComponentPrefab.name)){
            lvComponentObj = componentCache[lvComponentPrefab.name];
            lvComponentObj.SetActive(true);
        }
        else{
            //instantiate if not in cache
            lvComponentObj = (GameObject) GameObject.Instantiate(lvComponentPrefab);
            lvComponentObj.name = lvComponentPrefab.name;
        }

        return lvComponentObj.GetComponent<LevelComponent>();
    }

    //if GO is not in the cache add it else destroy. Component items
    //are destroyed in both cases
    public void DestroyAndCache(GameObject lvComponentObj){
        lvComponentObj.GetComponent<LevelComponent>().DestroyItems();

        if(!componentCache.ContainsKey(lvComponentObj.name)){
            componentCache.Add(lvComponentObj.name, lvComponentObj);
            lvComponentObj.SetActive(false);
        }else{
            GameObject.Destroy(lvComponentObj);
        }
    }

    public enum eLevelGroupID{
        Forest,
        City,
        CityNight,
    }
}
