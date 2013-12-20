using UnityEngine;
using System;

public class LgDateTime{
	
	public static TimeSpan GetTimeSinceLastPlayed() {
 		DateTime now = GetTimeNow();
        TimeSpan sinceLastPlayed = now.Date - DataManager.Instance.GameData.Degradation.LastTimeUserPlayedGame.Date;
		
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

    
}