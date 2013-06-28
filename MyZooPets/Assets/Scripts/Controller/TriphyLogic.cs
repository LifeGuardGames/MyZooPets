using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//=================================
//Trophy class
//Trophy display by tier( null.bronze,silver,gold)

public class TriphyLogic : MonoBehaviour {

	public List<string> name = new List<string>();
	public List<TrophyTier> tier = new List<TrophyTier>();
	public List<string> info = new List<string>();
	
	private Trophies[] trophies;
	private int MAX_TROPHY_COUNT = 10;
		
	
	// Use this for initialization
	void Start () {
	
		trophies = new Trophies[MAX_TROPHY_COUNT];
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
