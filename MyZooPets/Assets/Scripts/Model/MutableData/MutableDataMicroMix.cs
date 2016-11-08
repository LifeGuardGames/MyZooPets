using System.Collections.Generic;

public class MutableDataMicroMix{
	public List<string> MicrosCompleted { get; set; }
	public bool hasWon;

	public MutableDataMicroMix(){
		Init();
	}

	private void Init(){
		MicrosCompleted = new List<string>();
		hasWon = false;
	}
}