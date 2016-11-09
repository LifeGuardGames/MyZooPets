using System.Collections.Generic;

public class MutableDataMicroMix{
	public List<string> MicrosCompleted { get; set; }
	public bool HasWon { get; set; }
	public bool EntranceHasCrystal { get; set; }

	public MutableDataMicroMix(){
		Init();
	}

	private void Init(){
		MicrosCompleted = new List<string>();
		HasWon = false;
		EntranceHasCrystal = false;
	}
}