using UnityEngine;
using System.Collections;

public class ShooterGameBulletScript : MonoBehaviour{
	
	// speed of the bullet
	public float speed = 5f;
	// the position the bullet will move toward
	public Vector3 target;
	public bool isPierceing;
	public int health = 1;
	public bool canHitPlayer = false;
	
	//find target aids the bullet in moving toward the fire position
	public void FindTarget(){
		target = target - transform.position;
		float angle = Mathf.Atan2(target.y, target.x) * Mathf.Rad2Deg;
		transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
	}
	
	// Update is called once per frame
	void Update(){
		if(!ShooterGameManager.Instance.isPaused) {
			transform.Translate(speed * Time.deltaTime, 0, 0);
		}
	}
	
	// when offscreen destroy
	// !!!!! note onbecame invisible only works if the object has a renderer!!!!!!
	void OnBecameInvisible(){
		Destroy(this.gameObject);
	}
	
	// collision handling
	void OnTriggerEnter2D(Collider2D collider){
		if(collider.gameObject.tag == "ShooterEnemyBullet"){
			Destroy(collider.gameObject);
			if(!isPierceing || health < 2){
				Destroy(this.gameObject);
			}
		}
		else if(collider.gameObject.tag == "ShooterEnemyWall"){
			speed = -speed;
			canHitPlayer = true;
		}
		else if (collider.gameObject.tag == "ShooterBouncyWallTop") {
			target = new Vector3(target.x, -target.y, target.z);
			FindTarget();
		}
		else if(collider.gameObject.tag == "ShooterBouncyWallBottom") {
			target = new Vector3(target.x, target.y+ 500, target.z);
			FindTarget();
			target = new Vector3(target.x, target.y - 500, target.z);
		}
		else if(collider.gameObject.tag == "Player"){
			if(canHitPlayer){
				PlayerShooterController.Instance.ChangeHealth(-1);
				Destroy(this.gameObject);
			}
		}
	}
}