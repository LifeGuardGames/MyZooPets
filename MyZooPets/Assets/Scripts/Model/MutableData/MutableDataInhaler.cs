using System;

// InhalerData
// Save data for Inhaler
// Mutable data 
public class MutableDataInhaler{
    public bool IsFirstTimeRescue {get; set;} 			// First time the player has seen the rescue inhaler
                                        				// (this tells us whether to show tutorial arrows in the Inhaler Game)
	public DateTime LastPlayPeriodUsed {get; set;}		// Last pp the user played the regular inhaler
	public DateTime LastInhalerPlayTime {get; set;}		// Saved time value for inhaler countdown
	public int timesUsedInARow {get; set;}

    public MutableDataInhaler(){
        Init();
    }

    public void Init(){
        IsFirstTimeRescue = true;
		LastPlayPeriodUsed = DateTime.MinValue;
        LastInhalerPlayTime = LgDateTime.GetTimeNow();
		timesUsedInARow = 0;
    }
}
