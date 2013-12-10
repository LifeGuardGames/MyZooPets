using UnityEngine;
using System.Collections;

//---------------------------------------------------
// FlameChartLogic
// Manager layer for the fire chart.
//---------------------------------------------------

public class FlameChartLogic : Singleton<FlameChartLogic> {
	
	//---------------------------------------------------
	// GetSkillData()
	// Returns the skill data for the incoming skill id.
	//---------------------------------------------------
	public Skill GetSkillData( string strID ) {
		Skill data = DataSkills.GetSkill( strID );
		return data;
	}
}
