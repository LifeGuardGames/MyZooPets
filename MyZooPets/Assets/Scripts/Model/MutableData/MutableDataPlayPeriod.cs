using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class MutableDataPlayPeriod{
    public DateTime LatestPlayPeriod {get; set;}		// Last play period that use has signed on
	public DateTime InhalerInitialTime {get; set;}		// Saved time value for inhaler countdown

	public MutableDataPlayPeriod(){
		Init();
	}

	private void Init(){
		LatestPlayPeriod = DateTime.MinValue;
		InhalerInitialTime = DateTime.MinValue;
	}
}
