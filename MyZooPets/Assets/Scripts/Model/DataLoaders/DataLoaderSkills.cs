using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// Get data for flames
public class DataLoaderSkills : XMLLoaderGeneric<DataLoaderSkills>{
	public static ImmutableDataSkill GetData(string skillID){
		instance.InitXMLLoader();
		return instance.GetData<ImmutableDataSkill>(skillID);
	}
	
	public static List<ImmutableDataSkill> GetDataList(){
		instance.InitXMLLoader();
		return instance.GetDataList<ImmutableDataSkill>();
	}

	// Null if nothing
	public static ImmutableDataSkill NewFlameOnLevelUp(int level) {
		foreach(ImmutableDataSkill skillData in GetDataList()) {
			if(skillData.UnlockLevel == level) {
				return skillData;
			}
		}
		return null;
	}

	public static ImmutableDataSkill GetFlameAtLevel(int level) {
		ImmutableDataSkill highestSkillUnlockedSoFar = null;
		foreach(ImmutableDataSkill skillData in GetDataList()) {
			if(skillData.UnlockLevel <= level) {
				if(highestSkillUnlockedSoFar != null) {
					if(highestSkillUnlockedSoFar.UnlockLevel <= skillData.UnlockLevel) {
						highestSkillUnlockedSoFar = skillData;	// Overwrite highest so far
					}
				}
				else {
					highestSkillUnlockedSoFar = skillData;	// Assign initial case
				}
			}
		}
		return highestSkillUnlockedSoFar;
	}

	protected override void XMLNodeHandler(string id, IXMLNode xmlNode, Hashtable hashData, string errorMessage){
		ImmutableDataSkill data = new ImmutableDataSkill(id, xmlNode, errorMessage);

		// store the data
		if(hashData.ContainsKey(id)) {
			Debug.LogError(errorMessage + "Duplicate keys!");
		}
		else {
			hashData.Add(id, data);
		}
	}

	protected override void InitXMLLoader(){
		xmlFileFolderPath = "Skills";
	}
}