using UnityEngine;
using System.Collections;

public class Speech : MonoBehaviour {
    public GameObject textGameObject;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void Talk(string words){
        textGameObject.GetComponent<TextMesh>().text = words;
    }


}
