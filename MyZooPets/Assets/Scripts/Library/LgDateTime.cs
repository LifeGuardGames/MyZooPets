using UnityEngine;
using System;

public class LgDateTime{
	public static DateTime Today{
		get{
			DateTime now = GetTimeNow(); 
			return now.Date;
		}
	}

	public static TimeSpan GetTimeSpanSinceLastPlayed() {
 		DateTime now = GetTimeNow();
		DateTime last = DataManager.Instance.GameData.PlayPeriod.LastTimeUserPlayedGame;
        TimeSpan sinceLastPlayed = now - last;
		return sinceLastPlayed;
	}

    public static DateTime GetTimeNow(){
        DateTime now = DateTime.Now; 
        
        if(DataManager.Instance.isDebug && Constants.GetConstant<bool>("MorphTime")) {
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

	/// <summary>
	/// Gets the UTC now in timestamp format.
	/// </summary>
	/// <returns>The UTC now timestamp.</returns>
	public static string GetUtcNowTimestamp(){
		DateTime utcNow = DateTime.UtcNow;
		return GetUnixTimestampFromDateTime(utcNow);
	}

	/// <summary>
	/// Gets the time from unix timestamp.
	/// </summary>
	/// <returns>The time from unix timestamp.</returns>
	/// <param name="unixTimestamp">Unix timestamp.</param>
	private static DateTime GetTimeFromUnixTimestamp(long unixTimestamp){
		DateTime unixYear0 = new DateTime(1970, 1, 1);
		long unixTimeStampInTicks = unixTimestamp * TimeSpan.TicksPerSecond;
		DateTime dtUnix = new DateTime(unixYear0.Ticks + unixTimeStampInTicks);
		return dtUnix;
	}

	/// <summary>
	/// Gets the unix timestamp from date time.
	/// </summary>
	/// <returns>The unix timestamp from date time.</returns>
	/// <param name="date">Date.</param>
	private static string GetUnixTimestampFromDateTime(DateTime date){
		long unixTimestamp = date.Ticks - new DateTime(1970, 1, 1).Ticks;
		unixTimestamp /= TimeSpan.TicksPerSecond;
		return unixTimestamp.ToString();
	}
    
}