using UnityEngine;
using System.Collections;

/// <summary>
/// Tween ambient color.
/// NOTE: GRAYSCALE ONLY for now
/// </summary>
public class TweenAmbientColor : MonoBehaviour {
	
	private Color initialColor;
	public float time;
	public float lightDimPercentage;
	
	void Start(){
		// Save the current color
		initialColor = RenderSettings.ambientLight;
	}
	
	public void StartTween(){
		LeanTween.value(gameObject, DarkenLight, 0, 1, time);
	}
	
	private void DarkenLight(float factor){
		RenderSettings.ambientLight = new Color(initialColor.r - (lightDimPercentage/100f * factor),
												initialColor.g - (lightDimPercentage/100f * factor),
												initialColor.b - (lightDimPercentage/100f * factor), 1.0f);
	}
}
