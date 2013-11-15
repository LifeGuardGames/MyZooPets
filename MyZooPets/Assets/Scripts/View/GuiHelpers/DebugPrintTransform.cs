using UnityEngine;
using System.Collections;

public class DebugPrintTransform : MonoBehaviour {

	public GameObject go;
	
	void Start(){
		if(go == null){
			go = gameObject;
		}
		PrintInfo();
	}

	void PrintInfo(){
		print("==============================");
		print(go.name);
		print("World Position: " + go.transform.position);
		print("Local Position: " + go.transform.localPosition);
		print("World Rotation: " + go.transform.rotation);
		print("Local Rotation: " + go.transform.localRotation);
		print("Local Scale: " + go.transform.localScale);
		print("==============================");
	}
	
	void OnGUI(){
		if(GUI.Button(new Rect(100, 100, 100, 100), "Print info")){
			PrintInfo();
		}
	}
}
