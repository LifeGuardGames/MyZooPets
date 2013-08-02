using UnityEngine;
using System.Collections;

public abstract class RunnerItem : MonoBehaviour
{
    public string Name = "";
    public float PickupChance = 1.0f;

	// Use this for initialization
	void Start ()
    {
	    
	}
	
	// Update is called once per frame
	void Update ()
    {
	    
	}

    void OnTriggerEnter(Collider inOther)
    {
        Debug.Log("Coh-Liding");
        if (inOther.gameObject.tag == "Player")
        {
            Debug.Log("Picking up " + Name);
            OnPickup();
        }
    }

    // Define what you want the item to do on pickup here
    public abstract void OnPickup();
}
