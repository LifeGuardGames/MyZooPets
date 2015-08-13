using UnityEngine;
using System.Collections;

[RequireComponent (typeof(Collider2D))]
public class ShooterEnemy : MonoBehaviour{
	public float moveDuration = 2f;
	public int scoreVal;
	public int damage;
	public int health;
	public Animator animator;
	public ParticleSystem particle;
	public ParticleSystem particleDead;
	protected GameObject player;
	public bool isDead = false;
	private bool isMarkedForDestroy = true;

	// Use this for initialization
	void Awake(){
		player = GameObject.FindWithTag("Player");
		ShooterGameManager.OnStateChanged += OnGameStateChanged;
	}

	void OnDestroy(){
		ShooterGameManager.OnStateChanged -= OnGameStateChanged;
	}

	// Update is called once per frame
	void Update(){
		// work around for enemies who spawn during a state change they seem to miss the event call when this happens
		if(ShooterGameManager.Instance.GetGameState() == MinigameStates.Paused){
			LeanTween.pause(this.gameObject);
		}
		if(ShooterGameManager.Instance.GetGameState() == MinigameStates.GameOver){
			StartCoroutine(DestroyEnemy());
		}
	}

	void OnGameStateChanged(object sender, GameStateArgs args){
		MinigameStates eState = args.GetGameState();
		switch(eState){
		case MinigameStates.GameOver:
			StartCoroutine(DestroyEnemy());
			break;
		case MinigameStates.Paused:
			LeanTween.pause(this.gameObject);
			break;
		case MinigameStates.Playing:
			LeanTween.resume(this.gameObject);
			break;
		case MinigameStates.Restarting:
			StartCoroutine(DestroyEnemy());
			break;
		}
	}

	// handles collision not too much special there
	void OnTriggerEnter2D(Collider2D collider){
		if(collider.gameObject.tag == "bullet"){
			if(!collider.GetComponent<ShooterGameBulletScript>().isPierceing){
				Destroy(collider.gameObject);
			}
			health--;
			if(health <= 0){
				ShooterGameManager.Instance.AddScore(scoreVal);
				StartCoroutine(DestroyEnemy());
			}
		}
		else if(collider.gameObject.tag == "Player"){
			PlayerShooterController.Instance.ChangeHealth(-damage);
			StartCoroutine(DestroyEnemy());
		}
	}

	// this is a coroutine to make sure enemies are destroyed at the end of frame otherwise an error is thrown by NGUI
	private IEnumerator DestroyEnemy(){
		if(isMarkedForDestroy){		// Make sure this only gets called once
			isMarkedForDestroy = false;
			particleDead.gameObject.SetActive(true);
			AudioManager.Instance.PlayClip("shooterEnemyDie", variations:3);
			yield return new WaitForEndOfFrame();

			isDead = true;
			collider2D.enabled = false;
			LeanTween.cancel(this.gameObject);
			ShooterGameEnemyController.Instance.enemiesInWave--;
			ShooterGameEnemyController.Instance.CheckEnemiesInWave();
			ShooterGameManager.OnStateChanged -= OnGameStateChanged;
			// Visual changes
			animator.gameObject.SetActive(false);
			if(particle != null){
				particle.Stop();
			}

			// Wait until the particles has finished clearing before you destroy
			Destroy(this.gameObject, 2f);
		}
	}
}
