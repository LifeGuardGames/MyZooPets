using UnityEngine;
using System.Collections;


public class ShooterGameManager : MinigameManager<ShooterGameManager>
{

		//used to calculate fire rate
		private float lastFire = 0.0f;
		// used to calculate spawn rate
		private float lastSpawn = 0.0f;
		//Rate of Fire
		public float timeBetweenFire = 1.0f;
		//Fireball scale
		public float fBallScale = 1.0f;
		//Rate of enemy spawn at a spawner
		public float spawnTimer = 6.0f; 
		//The Spawn Manager 
		public GameObject Spawner;
		public GameObject bullet;
		public GameObject bulletSpawn;
		public GameObject Player;
		public Camera nguiCamera;
		//player health
		public float playerHealth;

		void Awake (){
				Application.targetFrameRate = 60;
				quitGameScene = SceneUtils.BEDROOM;
		}

		protected override void _Start (){
		}

		protected override void _OnDestroy (){
				Application.targetFrameRate = 30;
		}

		protected override string GetMinigameKey (){
				return "ShooterGame";
		}

		protected override bool IsTutorialOn (){
				return false;	//TODO Change
		}
	
		protected override bool HasCutscene (){
				return false;	//TODO Change
		}

		protected override void _NewGame (){
				//Debug.Log ("hi");
				lastFire = Time.time;
				lastSpawn = Time.time;
		}
		
		public override int GetReward (MinigameRewardTypes eType){
				return GetStandardReward (eType);
		}

		void OnTap(TapGesture e){
		if(!IsTouchingNGUI(e.Position)){
#if !UNITY_EDITOR
		
		Vector3 touchPos = new Vector3(Input.GetTouch(0).position.x, Input.GetTouch(0).position.y, 1);
		Vector3 lookPos = Camera.main.ScreenToWorldPoint(touchPos);
		shoot(lookPos);
#endif
#if UNITY_EDITOR
		Vector3 mousePos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 1);
		Vector3 lookPos = Camera.main.ScreenToWorldPoint(mousePos);
		shoot(lookPos);
#endif
			}
		}

		public void shoot (Vector3 dir){
				fBallScale= Player.GetComponent<Player>().FBallScale;
				GameObject instance = Instantiate (bullet,bulletSpawn.transform.position, bullet.transform.rotation)as GameObject;
				instance.gameObject.transform.localScale/=fBallScale;
				instance.GetComponent<bulletScript>().target= dir;
		}
	
		// Update is called once per frame
		protected override void _Update (){
		if (Time.time - lastSpawn >= spawnTimer) {
			int randomeSpawner = Random.Range (0, 3);
			Spawner.GetComponent<SpawnManager> ().spawnTrigger (randomeSpawner);
			lastSpawn= Time.time;
		}
	}
	//True: if finger touches NGUI 
	/// <summary>
	/// Determines whether if the touch is touching NGUI element
	/// </summary>
	/// <returns><c>true</c> if this instance is touching NGUI; otherwise, <c>false</c>.</returns>
	/// <param name="screenPos">Screen position.</param>
	private bool IsTouchingNGUI(Vector2 screenPos){
		Ray ray = nguiCamera.ScreenPointToRay(screenPos);
		RaycastHit hit;
		int layerMask = 1 << 10; 
		bool isOnNGUILayer = false;
		
		// Raycast
		if(Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask)){
			isOnNGUILayer = true;
		}
		return isOnNGUILayer;
	}
}
