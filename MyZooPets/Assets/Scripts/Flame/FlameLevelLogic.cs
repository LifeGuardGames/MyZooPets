using System;
using System.Linq;
using System.Collections.Generic;

/// <summary>
/// Controller that checks for flame level up whenever the pet levels up 
/// </summary>
public class FlameLevelLogic : Singleton<FlameLevelLogic> {
	public static EventHandler<FlameLevelEventArgs> OnFlameLevelUp;
	public class FlameLevelEventArgs : EventArgs {
		private Skill unlockedSkill;

		public Skill UnlockedSkill {
			get { return unlockedSkill; }
		}

		public FlameLevelEventArgs(Skill skill) {
			unlockedSkill = skill;
		}
	}

	private List<Skill> allSkills;

	void Awake() {
		allSkills = SortList(DataLoaderSkills.GetDataList());
	}

	void Start() {
		HUDAnimator.OnLevelUp += CheckFlameLevelUp;
	}

	void OnDestroy() {
		HUDAnimator.OnLevelUp -= CheckFlameLevelUp;
	}

	public Skill GetSkillData(string skillID) {
		Skill data = DataLoaderSkills.GetData(skillID);
		return data;
	}

	public Skill GetCurrentSkill() {
		Skill currentSkill = GetSkillData(DataManager.Instance.GameData.Flame.CurrentSkillID);
		return currentSkill;
	}

	public Skill GetSkillUnlockAtNextLevel() {
		int nextLevel = LevelLogic.Instance.NextLevel;
		Skill selectedSkill = null;

		foreach(Skill skill in allSkills) {
			if(skill.UnlockLevel == nextLevel) {
				selectedSkill = skill;
			}
		}
		return selectedSkill;
	}

	/// <summary>
	/// Listen to pet level up and Checks the flame level up.
	/// </summary>
	private void CheckFlameLevelUp(object sender, EventArgs args) {
		if(allSkills == null) {
			allSkills = SortList(DataLoaderSkills.GetDataList());
		}

		int currentLevel = (int)LevelLogic.Instance.CurrentLevel;
		foreach(Skill skill in allSkills) {
			if(skill.UnlockLevel <= currentLevel) {
				if(!skill.IsUnlocked) {
					DataManager.Instance.GameData.Flame.UpdateSkillStatus(skill.ID, true);
					DataManager.Instance.GameData.Flame.CurrentSkillID = skill.ID;

					if(OnFlameLevelUp != null) {
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
	private List<Skill> SortList(List<Skill> skills) {
		List<Skill> skillList = (from skill in skills
								 orderby skill.UnlockLevel ascending
								 select skill).ToList();
		return skillList;
	}
}
