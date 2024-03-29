﻿using UnityEngine;
using System.Collections;

public class ParticleColorChange : MonoBehaviour{

	public ParticleSystem particle;
	public string color1Name;
	public Color color1;

	public string color2Name;
	public Color color2;

	public string color3Name;
	public Color color3;

	public string color4Name;
	public Color color4;

	public string color5Name;
	public Color color5;

	public string color6Name;
	public Color color6;


	public void ChangeColor(string colorName){
		if(string.Equals(colorName, color1Name)){
			particle.startColor = color1;
		}
		else if(string.Equals(colorName, color2Name)){
			particle.startColor = color2;
		}
		else if(string.Equals(colorName, color3Name)){
			particle.startColor = color3;
		}
		else if(string.Equals(colorName, color4Name)){
			particle.startColor = color4;
		}
		else if(string.Equals(colorName, color5Name)){
			particle.startColor = color5;
		}
		else if(string.Equals(colorName, color6Name)){
			particle.startColor = color6;
		}
		else{
			Debug.LogWarning("No valid colors detected!");
		}
		particle.Stop();
	}
}
