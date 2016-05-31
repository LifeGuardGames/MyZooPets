using UnityEngine;
using System.Collections;

public class InhalerPopup : MonoBehaviour {
	public float bottomY;
	public float leftX;
	public float RightX;
	private Vector3 velocity;
	private float rotationalVelocity = 0;
	private float minRotational = 40;
	private float maxRotational = 70;
	private Vector3 gravity = new Vector3(0, -5);
	private float intensity = 1;
	private int taps = 0;
	private bool canCall=true;
	// Use this for initialization
	public Vector3 Velocity {
		get {
			return velocity;
		}
		set {
			velocity = value;
			ChangeRotational();
		}
	}

	void Update() {
		Vector3 lastPos = transform.position;
		transform.position += velocity * Time.deltaTime;
		velocity += gravity;
		if (Input.GetKeyDown(KeyCode.Space)) {
			OnTap();
		}
		transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles + new Vector3(0, 0, rotationalVelocity) * Time.deltaTime);
		if (transform.position.y<bottomY&&canCall){
			StartCoroutine(DoctorMatchManager.Instance.FinishInhalerPopup(false));
			canCall=false;
		}
		if (transform.position.x<leftX||transform.position.x>RightX) { //If we are outside, flip
			velocity = new Vector3(-1*velocity.x,velocity.y);
			transform.position=lastPos;
		}
	}

	void OnMouseDown() {
		OnTap();
	}

	private void OnTap() {
		DoctorMatchManager.Instance.LockZones();

		Color c = Color.red;
		intensity -= .35f;
		taps++;
		GetComponent<SpriteRenderer>().color = new Color(intensity, intensity, intensity);

		float xVelocity = velocity.x;
		xVelocity *= Random.Range(.8f, 1.4f);
		float rangeResult = Random.Range(-1f,1f);
		xVelocity *= (rangeResult>=0) ? 1 : -1;

		Velocity = new Vector3(xVelocity, Random.Range(120,170));
		if (taps >= 3&&canCall) {
			GetComponent<SpriteRenderer>().color = new Color(0,0,0,0);
			StartCoroutine(DoctorMatchManager.Instance.FinishInhalerPopup(true,transform.position, gameObject));
			canCall=false;
		}
	}

	void ChangeRotational() {
		if (rotationalVelocity == 0) {
			rotationalVelocity = Random.Range(minRotational, maxRotational);
			float rangeResult = Random.Range(-1f,1f);
			rotationalVelocity *= (rangeResult>=0) ? 1 : -1;
		} else {
			rotationalVelocity *= Random.Range(.8f, 1.4f);
			float rangeResult = Random.Range(-1f,1f);
			rotationalVelocity *= (rangeResult>=0) ? 1 : -1;
		}
	}
}
