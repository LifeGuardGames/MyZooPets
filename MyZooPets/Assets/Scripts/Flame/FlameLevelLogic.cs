using System;
using System.Linq;
using System.Collections.Generic;

/// <summary>
/// Controller that checks for flame level up whenever the pet levels up 
/// </summary>
public class FlameLevelLogic : Singleton<FlameLevelLogic> {
	public static EventHandler<FlameLevelEventArgs> OnFlameLevelUp;
	public class FlameLevelEventArgs : EventArgs {
		private ImmutableDataSkill unlockedSkill;

		public ImmutableDataSkill UnlockedSkill {
			get { return unlockedSkill; }
		}

		public FlameLevelEventArgs(ImmutableDataSkill skill) {
			unlockedSkill = skill;
		}
	}

	private List<ImmutableDataSkill> allSkills;

	void Awake() {
		allSkills = SortList(DataLoaderSkills.GetDataList());
	}

	void Start() {
		HUDAnimator.OnLevelUp += CheckFlameLevelUp;
	}

	void OnDestroy() {
		HUDAnimator.OnLevelUp -= CheckFlameLevelUp;
	}

	public ImmutableDataSkill GetSkillUnlockAtNextLevel() {
		int nextLevel = LevelLogic.Instance.NextLevel;
		ImmutableDataSkill selectedSkill = null;

		foreach(ImmutableDataSkill skill in allSkills) {
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

		ImmutableDataSkill skillData = DataLoaderSkills.NewFlameOnLevelUp(currentLevel);
        if(skillData != null) {
			if(OnFlameLevelUp != null) {
				FlameLevelEventArgs flameArgs = new FlameLevelEventArgs(skillData);
				OnFlameLevelUp(this, flameArgs);
			}
		}
	}

	/// <summary>
	/// Selects the list from dictionary and sort.
	/// </summary>
	private List<ImmutableDataSkill> SortList(List<ImmutableDataSkill> skills) {
		List<ImmutableDataSkill> skillList = (from skill in skills
								 orderby skill.UnlockLevel ascending
								 select skill).ToList();
		return skillList;
	}
}
