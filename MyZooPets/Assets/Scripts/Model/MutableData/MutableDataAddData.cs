using System.Collections.Generic;

public class MutableDataAdData {

	public Dictionary<string, int> AdViews;
	public int SeanAdViews;	// AB Test for sequential adViews - consecutive for 3 times minus first time

	public MutableDataAdData() {
		Init();
	}
	
	public void Init() {
		AdViews = new Dictionary<string, int>();
		SeanAdViews = 0;
	}
}
