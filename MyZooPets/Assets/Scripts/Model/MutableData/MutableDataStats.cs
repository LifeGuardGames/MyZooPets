public class MutableDataStats {
	public int Points { get; set; }     // Renamed to Xp
	public int Stars { get; set; }      // Renamed to Coin
	public int TotalStars { get; set; }
	public int Health { get; set; }		// Pet's Health
	public int Mood { get; set; }		// Renamed to Hunger
	public int Shards { get; set; }     // Shard count, get out of 100 for now

	private static int SAD_THRESH = 50;         // if the pet's mood <= this number, it will be sad
	private static int SICK_THRESH = 60;        // if the pet's Health <= this number, it will be sick
	private static int VERY_SICK_THRESH = 30;   // if the pet's Health <= this number, it will be very sick

	//=======================Initialization==================
	public MutableDataStats() {
		Init();
	}

	private void Init() {
		Health = 80;
		Mood = 80;
		Points = 0;
		Stars = 100;
		Shards = 0;
		TotalStars = 100000;
	}

	#region Stat manipulation calls
	// NOTE: Stats are tracked by LOCAL xp, each level will start at zero again once leveled up
	public void AddCurrentLevelXp(int deltaXp) {
		Points += deltaXp;
	}

	public void ResetCurrentLevelXp() {
		Points = 0;
	}

	public void UpdateCoins(int deltaCoins) {
		Stars += deltaCoins;
		if(Stars < 0) {
			Stars = 0;
		}
		if(deltaCoins > 0) {
			TotalStars += deltaCoins;
		}
	}

	public void UpdateHealth(int deltaHealth) {
		Health += deltaHealth;
		if(Health > 100) {
			Health = 100;
		}
		if(Health < 0) {
			Health = 0;
		}
	}

	public void UpdateHunger(int deltaMood) {
		Mood += deltaMood;
		if(Mood > 100) {
			Mood = 100;
		}
		if(Mood < 0) {
			Mood = 0;
		}
	}

	// The animation takes care of rewarding the crystal after 100 shard... take care of it here?
	public void AddShard(int val) {
		Shards += val;
		if(Shards >= 100) {
			ResetShard();
		}
	}

	public void ResetShard() {
		Shards = 0;
	}
	#endregion

	/// <summary>
	/// Gets the state of the mood. Based on the numerical value of the mood stat,
	/// returns an enum of the pet's mood
	/// </summary>
	/// <returns>The mood state.</returns>
	public PetMoods GetMoodState() {
		PetMoods eMood = PetMoods.Happy;
		if(Mood <= SAD_THRESH) {
			eMood = PetMoods.Sad;
		}
		return eMood;
	}

	/// <summary>
	/// Gets the state of the health. Based on the numerical value of the Health stat,
	/// returns an enum of the pet's health.
	/// </summary>
	/// <returns>The health state.</returns>
	public PetHealthStates GetHealthState() {
		PetHealthStates eHealth = PetHealthStates.Healthy;
		if(Health <= VERY_SICK_THRESH) {
			eHealth = PetHealthStates.VerySick;
		}
		else if(Health <= SICK_THRESH) {
			eHealth = PetHealthStates.Sick;
		}
		return eHealth;
	}
}
