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
		health += ShooterGameManager.Instance.waveNum / 10;
        damage += ShooterGameManager.Instance.waveNum / 10;
		ShooterGameManager.onRestart += WipeOnRestart;
	}

	public void WipeOnRestart() {
		StartCoroutine("DestroyEnemy");
	}

	// Update is called once per frame
	void Update(){
		// work around for enemies who spawn during a state change they seem to miss the event call when this happens
		if(ShooterGameManager.Instance.isPaused){
			LeanTween.pause(this.gameObject);
		}
		else {
			LeanTween.resume(this.gameObject);
		}
		if(ShooterGameManager.Instance.isGameOver){
			StartCoroutine(DestroyEnemy());
		}
	}


	// handles collision not too much special there
	void OnTriggerEnter2D(Collider2D collider){
		if(collider.gameObject.tag == "Shooterbullet"){
			health -= collider.GetComponent<ShooterGameBulletScript>().health;
			if(!collider.GetComponent<ShooterGameBulletScript>().isPierceing ){
				if(collider.GetComponent<ShooterGameBulletScript>().health > damage){
					collider.GetComponent<ShooterGameBulletScript>().health--;
				}
				else{
					Destroy(collider.gameObject);
				}
			}		
			if(health <= 0){
				ShooterGameManager.Instance.AddScore(scoreVal);
				StartCoroutine(DestroyEnemy());
			}
		}
		else if(collider.gameObject.tag == "Player"){
			PlayerShooterController.Instance.ChangeHealth(-damage);
			StartCoroutine(DestroyEnemy());
		}
		else if(collider.gameObject.tag == "ShooterWall"|| collider.gameObject.tag == "ShooterMiniPetFireball"){
			StartCoroutine(DestroyEnemy());
		}
	}

	// this is a coroutine to make sure enemies are destroyed at the end of frame otherwise an error is thrown by NGUI
	private IEnumerator DestroyEnemy(){
		if(isMarkedForDestroy){     // Make sure this only gets called once
			ShooterGameManager.onRestart -= WipeOnRestart;
			isMarkedForDestroy = false;
			particleDead.gameObject.SetActive(true);
			AudioManager.Instance.PlayClip("shooterEnemyDie", variations:3);
			yield return new WaitForEndOfFrame();

			isDead = true;
			GetComponent<Collider2D>().enabled = false;
			LeanTween.cancel(this.gameObject);
			ShooterGameEnemyController.Instance.enemiesInWave--;
			ShooterGameEnemyController.Instance.CheckEnemiesInWave();
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
