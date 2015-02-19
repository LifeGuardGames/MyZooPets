using UnityEngine;
using System.Collections;

public static class SceneUtils{
	public static string MENU = "MenuScene";
	public static string BEDROOM = "NewBedRoom";
	public static string YARD = "Yard";
	public static string LOADING = "LoadingScene";
	public static string INHALERGAME = "InhalerGamePet";

	public static ZoneTypes GetZoneTypeFromSceneName(string levelName){
		if(string.Equals(levelName, BEDROOM)){
			return ZoneTypes.Bedroom;
		}
		else if(string.Equals(levelName, YARD)){
			return ZoneTypes.Yard;
		}
		else{
			return ZoneTypes.None;
		}
	}
}
