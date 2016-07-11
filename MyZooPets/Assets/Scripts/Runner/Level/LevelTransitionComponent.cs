/* Sean Duane
 * LevelTransitionComponent.cs
 * 8:26:2013   11:59
 * Description:
 * Hold a component, which is linked by a "From" and "To" group.
 * These groups are checked upon a new group transition. The first one that has:
 * CurrentGroup == FromGroupID
 * NextGroup == ToGroupID
 * is then pushed as the next level component and instantiated.
 * This will, hopefully, provide smoother level transitions.
 */

using UnityEngine;
using System.Collections;

public class LevelTransitionComponent : LevelComponent{
	public LevelGroup.eLevelGroupID FromGroupID;
	public LevelGroup.eLevelGroupID ToGroupID;
}
