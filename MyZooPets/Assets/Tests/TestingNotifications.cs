using UnityEngine;
using System.Collections;

public class TestingNotifications : MonoBehaviour {

    public GameObject anchor;
    public GameObject message;
	// Use this for initialization
	void Start () {
        // Instantiate(message, Vector3 (0, 0, 0), Quaternion.identity);
        GameObject newMessage = Instantiate(message, Vector3.zero, Quaternion.identity) as GameObject;
        newMessage.transform.parent = anchor.transform;
        newMessage.transform.localScale = Vector3.one;
	}

	// Update is called once per frame
	void Update () {

	}
}
