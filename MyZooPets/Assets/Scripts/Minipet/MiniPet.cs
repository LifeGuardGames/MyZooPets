using UnityEngine;
using System.Collections;

/// <summary>
/// Script to control Minipet and contains the basic properties of a minipet.
/// Should attach this script to the highest parent in the minipet prefab
/// </summary>
public class MiniPet : MonoBehaviour {

	/*
	 * Attributes:
	 * Immutable
	 * ID
	 * Type
	 * Dict of amount of food required to lv up
	 * Dict of lv up reward (gems, coins, special decos)
	 * 
	 * prefab name
	 * startiing position
	 * walking pattern?
	 * 
	 * ------------------
	 * Mutable
	 * current lv
	 * current food xp
	 * isUnlocked
	 * 
	 * cannot use DM directly from here so connect to MinipetManager for any mutable data
	 */

	//On item drop handler. if the correct food modify current food xp

	//On tap handler. do a funny dance or sth

	//
	public void Init(string id, ImmutableDataMiniPet miniPet){

	}

}
