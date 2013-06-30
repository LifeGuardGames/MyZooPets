using UnityEngine;
using System.Collections;

public class LevelUpGUI : MonoBehaviour {
    public NotificationUIManager notificationUIManager;
    private LevelUpLogic levelUpLogic; //reference 

	// Use this for initialization
	void Start () {
	   levelUpLogic = GameObject.Find("GameManager").GetComponent<LevelUpLogic>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

}
