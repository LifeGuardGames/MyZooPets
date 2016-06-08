using UnityEngine;
using System.Collections;
/* Follows camera and basically just helps a collider hit coins and magnetize them
 * disabled and enabled by PlayerController
 */
public class MagneticField : Singleton<MagneticField> {
	// Update is called once per frame
	void Update () {
		transform.position=Camera.main.ScreenToWorldPoint(new Vector3(Screen.width/2,Screen.height/4,-Camera.main.transform.position.z));//new Vector3(PlayerController.Instance.transform.position.x+xOffset/4,defaultHeight);
	}
	void OnTriggerEnter(Collider other) {
		if (other.CompareTag("RunnerCoin")){
			other.GetComponent<CoinItem>().Magnetize();
		}
	}
	public void EnableMagnet(bool enabled) {
		GetComponent<Collider>().enabled=enabled;
	}
}
