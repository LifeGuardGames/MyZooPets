using System;

public class MutableDataDegradation{
    public DateTime LastPlayPeriodTriggerSpawned {get; set;}
    public bool IsTriggerSpawned {get; set;} 	//True: triggers for this play period has been spawned
	public int UncleanedTriggers {get; set;}

    public MutableDataDegradation(){
        Init();
    }

    private void Init(){
        LastPlayPeriodTriggerSpawned = PlayPeriodLogic.GetCurrentPlayPeriod();
        IsTriggerSpawned = false;
		UncleanedTriggers = 0;
    }
}
