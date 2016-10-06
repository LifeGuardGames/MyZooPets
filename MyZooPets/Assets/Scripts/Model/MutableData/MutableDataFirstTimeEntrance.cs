using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class MutableDataFirstTimeEntrance {
	public List<string> EntranceClicked { get; set; }

	public MutableDataFirstTimeEntrance() {
		EntranceClicked = new List<string>();
	}

	public bool IsFirstTimeEntrance(string entranceID) {
		if(EntranceClicked.Contains(entranceID)) {
			return false;
		}
		else {
			return true;
		}
	}

	public void EntranceUsed(string entranceID) {
		if(!EntranceClicked.Contains(entranceID)) {
			EntranceClicked.Add(entranceID);
		}
	}
}
