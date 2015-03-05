using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class MutableDataPlayPeriod{
    public DateTime LastPlayPeriod {get; set;}		// Last play period that use has signed on

	public MutableDataPlayPeriod(){
		Init();
	}

	private void Init(){
		LastPlayPeriod = DateTime.MinValue;
	}
}
