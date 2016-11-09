using UnityEngine;
using System.Collections.Generic;

public class MutableDataMicroMix{
	public List<string> MicrosCompleted { get; set; }
	private bool HasWon;
	public bool hasWon {
		get{ return HasWon; }
		set { HasWon = value; }
	}

	public MutableDataMicroMix(){
		Init();
	}

	private void Init(){
		MicrosCompleted = new List<string>();
		HasWon = true;
	}
}