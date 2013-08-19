using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LevelGroup : MonoBehaviour {
    public string LevelID = "";
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
}
