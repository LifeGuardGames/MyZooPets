using UnityEngine;
using System.Collections;

public class MenuScenePetManager : MonoBehaviour{
	public Animator animator;
	private float timer = 0;
	private float timeBeforeNextRandomAnimation = 5f;
	private string[] animTriggers = new string[]{"jump", "wave"}; //trigger param from AnimController

	// Update is called once per frame
	void Update(){
		timer += Time.deltaTime;
		if(timer > timeBeforeNextRandomAnimation){
			timer = 0;
			int randomIndex = Random.Range(0, animTriggers.Length);
			animator.SetTrigger(animTriggers[randomIndex]);
		}
	}
}
	