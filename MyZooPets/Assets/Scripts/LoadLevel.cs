using UnityEngine;
using System.Collections;

//Needs to store the last level that was loaded
//this class can maybe handle any loading screen in between scenes as well
public static class LoadLevel{
    public static string lastLevelLoaded = "BedRoom";

    public static void Load(string levelName){
        lastLevelLoaded = Application.loadedLevelName;
        Application.LoadLevel(levelName);
    }
}
