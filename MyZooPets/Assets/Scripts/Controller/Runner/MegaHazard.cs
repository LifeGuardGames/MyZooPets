using UnityEngine;
using System.Collections;

public class MegaHazard : MonoBehaviour {

    public float Speed = 3.0f;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	    Vector3 pos = transform.position;
        pos.z += Speed * Time.deltaTime;
        transform.position = pos;
	}

    void OnTriggerEnter(Collider inOther)
    {
        if (inOther.gameObject.tag == "Player")
        {
            Debug.Log("Smoke monster ahhh");
    
            RunnerGameManager gameManager = ((GameObject)GameObject.FindGameObjectWithTag("GameManager")).GetComponent<RunnerGameManager>();
            gameManager.ActivateGameOver();
        }
    }
}
