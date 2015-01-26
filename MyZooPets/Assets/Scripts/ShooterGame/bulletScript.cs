using UnityEngine;
using System.Collections;

public class bulletScript : MonoBehaviour {

	public float speed=5f;
	public Vector3 target;

	public void FindTarget(){
		target = target - transform.position;
		float angle = Mathf.Atan2(target.y, target.x) * Mathf.Rad2Deg;
		transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
	}

	// Update is called once per frame
	void Update () {
		transform.Translate (speed*Time.deltaTime,0,0);
	}

	void OnBecameInvisible (){
		Destroy(this.gameObject);
	}

	void OnTriggerEnter2D(Collider2D collider){
		if(collider.gameObject.tag == "EnemyBullet"){
			Destroy(collider.gameObject);
			Destroy(this.gameObject);
		}
	}
}
