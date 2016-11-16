using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MutableDataAdData {

	public Dictionary<string, int> AdViews;
	public int SeanAdViews;

	public MutableDataAdData() {
		Init();
	}
	
	public void Init() {
		AdViews = new Dictionary<string, int>();
		SeanAdViews = 0;
	}
}
