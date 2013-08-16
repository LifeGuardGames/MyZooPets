using UnityEngine;
using System.Collections;

public class CoinItem : RunnerItem {
    public int CoinValue = 1;

	// Use this for initialization
	public override void Start () {
        base.Start();
	}
	
	// Update is called once per frame
    public override void Update() {
        base.Update();
	}

    public override void OnPickup()
    {
        RunnerGameManager.GetInstance().ScoreManager.AddCoins(CoinValue);
        GameObject.Destroy(gameObject);
    }
}
