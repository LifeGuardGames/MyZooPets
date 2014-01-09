using UnityEngine;
using System.Collections;

/// <summary>
/// NGUI set scroll panel size.
/// This dynamically sets the panel size of the scroll panel on start!
/// Clipping must be enabled!!
/// </summary>
public class NGUISetScrollPanelSize : MonoBehaviour {
	
	public int width;
	public int height;
	
	public bool useDeviceWidth;
	public bool useDeviceHeight;
	
	private int deviceWidth;
	private int deviceHeight;
	
	private UIPanel scrollPanel;
	
	void Awake(){
		if(gameObject.GetComponent<UIPanel>().clipping != UIDrawCall.Clipping.None){
			deviceWidth = Screen.width;
			deviceHeight = Screen.height;
			
			int nativeWidth = Constants.GetConstant<int>("NativeWidth");
			int nativeHeight = Constants.GetConstant<int>("NativeHeight");
			
			print ("deviceW: " + deviceWidth + " || " + "nativeW: " + nativeWidth);
			
			int newWidth = useDeviceWidth ? (int)(deviceWidth * ((deviceWidth * 1.0f) / (nativeWidth * 1.0f))) : width;
			int newHeight = useDeviceHeight ? deviceHeight : height;
			
			print (newWidth);
			
			scrollPanel = gameObject.GetComponent<UIPanel>();
			Vector4 originalClipRange = scrollPanel.clipRange;
			
			scrollPanel.clipRange = new Vector4(originalClipRange.x, originalClipRange.y, newWidth, newHeight);
		}
		else{
			Debug.LogError("Trying to set clipping range for UIpanel that doesnt clip");
		}
		
		// Remove self
		Destroy(this);
	}
}