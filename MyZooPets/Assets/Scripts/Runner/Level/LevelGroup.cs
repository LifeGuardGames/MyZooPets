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
    public List<LevelComponent> LevelComponents;

	// Use this for initialization
	void Start() {
	
	}
	
	// Update is called once per frame
	void Update() {
	
	}

    public enum eLevelGroupID{
        Forest,
        City,
        CityNight,
    }
}
