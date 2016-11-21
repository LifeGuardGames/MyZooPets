using System.Collections.Generic;

public class MutableDataAdData {

	public Dictionary<string, int> AdViews { get; set; }
	public int SeanAdViews { get; set; }	// AB Test for sequential adViews - consecutive for 3 times minus first time
	public bool SeanAdViewsIosAlternateToggle { get; set; } // iOS restarts the app, so dont show on every other return

	public MutableDataAdData() {
		Init();
	}
	
	public void Init() {
		AdViews = new Dictionary<string, int>();
		SeanAdViews = 0;
		SeanAdViewsIosAlternateToggle = true;
    }
}
