using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class MutableDataPlayPeriod{
    public DateTime LatestPlayPeriod {get; set;}		// Last play period that use has signed on

	public MutableDataPlayPeriod(){
		Init();
	}

	private void Init(){
		LatestPlayPeriod = DateTime.MinValue;
	}
}
