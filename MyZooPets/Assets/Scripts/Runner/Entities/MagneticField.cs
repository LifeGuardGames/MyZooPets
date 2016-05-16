using UnityEngine;
using System.Collections;

public class MagneticField : Singleton<MagneticField> {
	private float defaultHeight = 15f;
	private float xOffset;
	// Use this for initialization
	void Start () {
		xOffset = transform.localScale.x;
		GetComponent<MeshRenderer>().enabled=false;
	}
	
	// Update is called once per frame
	void Update () {
		transform.position=Camera.main.ScreenToWorldPoint(new Vector3(Screen.width/2,Screen.height/4,-Camera.main.transform.position.z));//new Vector3(PlayerController.Instance.transform.position.x+xOffset/4,defaultHeight);
	}
	void OnTriggerEnter(Collider other) {
		if (other.CompareTag("Coin")){
			other.GetComponent<CoinItem>().Magnetize=true;
		}
	}
}
