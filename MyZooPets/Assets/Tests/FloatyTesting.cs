using UnityEngine;
using System.Collections;

public class FloatyTesting : MonoBehaviour {
    public GameObject anchor;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnGUI(){
        if(GUI.Button(new Rect(0, 0, 100, 100), "FloatyText")){
            Hashtable option = new Hashtable();
            option.Add("parent", anchor);
            option.Add("text", "testing123");
            option.Add("textSize", 128);
            FloatyUtil.SpawnFloatyText(option);
        }
    }
}
