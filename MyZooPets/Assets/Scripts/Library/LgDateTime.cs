using UnityEngine;
using System;

public class LgDateTime{
	
	public static TimeSpan GetTimeSinceLastPlayed() {
 		DateTime now = GetTimeNow();
		DateTime last = DataManager.Instance.GameData.Degradation.LastTimeUserPlayedGame;
        TimeSpan sinceLastPlayed = now - last;
	    // Debug.Log("sinceLastPlayed: " + sinceLastPlayed);
		return sinceLastPlayed;
	}

    public static DateTime GetTimeNow(){
        DateTime now = DateTime.Now; 
        bool morphTime = Constants.GetConstant<bool>("MorphTime");
        
        if(morphTime){
            try{
                string testTime = Constants.GetConstant<string>("TestTime");
                now = DateTime.Parse(testTime);
            }
            catch(FormatException e){
                Debug.LogError(e.Message);
            }
        }

        return now;
    }

    public static DateTime Today{
        get{
            DateTime now = GetTimeNow(); 
            return now.Date;
        }
    }

    
}