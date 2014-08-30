using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

//---------------------------------------------------
// FlameLevelUpLogic 
// Controller that checks for flame level up whenever
// the pet levels up 
//---------------------------------------------------
public class FlameLevelLogic : Singleton<FlameLevelLogic>{
	public static EventHandler<FlameLevelEventArgs> OnFlameLevelUp;
	public class FlameLevelEventArgs : EventArgs{
		private Skill unlockedSkill;

		public Skill UnlockedSkill{
			get{ return unlockedSkill;}
		}

		public FlameLevelEventArgs(Skill skill){
			unlockedSkill = skill;
		}
	}

	private List<Skill> allSkills;

	void Awake(){
//        DataLoaderSkills.SetupData();
		allSkills = SortList(DataLoaderSkills.GetDataList());
	}

	void Start(){
			HUDAnimator.OnLevelUp += CheckFlameLevelUp;
	}

	void OnDestroy(){
			HUDAnimator.OnLevelUp -= CheckFlameLevelUp;
	}

	/// <summary>
	/// Gets the skill data.
	/// </summary>
	/// <returns>The skill data.</returns>
	/// <param name="skillID">Skill ID.</param>
	public Skill GetSkillData(string skillID){
		Skill data = DataLoaderSkills.GetData(skillID);
		return data;
	}

	/// <summary>
	/// Gets the current skill.
	/// </summary>
	/// <returns>The current skill.</returns>
	public Skill GetCurrentSkill(){
		Skill currentSkill = GetSkillData(DataManager.Instance.GameData.Flame.CurrentSkillID);
		return currentSkill;
	}

	/// <summary>
	/// Gets the skill unlock at next level.
	/// </summary>
	/// <returns>The skill unlock at next level.</returns>
	public Skill GetSkillUnlockAtNextLevel(){
		int nextLevel = LevelLogic.Instance.NextLevel;
		Skill selectedSkill = null;

		foreach(Skill skill in allSkills){
			if(skill.UnlockLevel == nextLevel)
				selectedSkill = skill;
		}

		return selectedSkill;
	}
	
	/// <summary>
	/// Listen to pet level up and Checks the flame level up.
	/// </summary>
	/// <param name="sender">Sender.</param>
	/// <param name="args">Arguments.</param>
	private void CheckFlameLevelUp(object sender, EventArgs args){
		if(allSkills == null){
			allSkills = SortList(DataLoaderSkills.GetDataList());
		}

		int currentLevel = (int)LevelLogic.Instance.CurrentLevel;
		foreach(Skill skill in allSkills){
			if(skill.UnlockLevel == currentLevel){
				if(!skill.IsUnlocked){
					DataManager.Instance.GameData.Flame.UpdateSkillStatus(skill.ID, true);
					DataManager.Instance.GameData.Flame.CurrentSkillID = skill.ID;

					if(OnFlameLevelUp != null){
						FlameLevelEventArgs flameArgs = new FlameLevelEventArgs(skill);
						OnFlameLevelUp(this, flameArgs);
					}
				}
			}
		}
	}

	/// <summary>
	/// Selects the list from dictionary and sort.
	/// </summary>
	/// <returns>The list from dictionary and sorted.</returns>
	/// <param name="skillDict">Skill dict.</param>
	private List<Skill> SortList(List<Skill> skills){
//        var skills = from keyValuePair in skillDict
//                        select keyValuePair.Value;
		List<Skill> skillList = (from skill in skills
                                orderby skill.UnlockLevel ascending
                                select skill).ToList();
		return skillList;
	}
}
