using UnityEngine;
using System.Collections;

public class ShaderTest : MonoBehaviour {
	
	public GameObject test1;
	public GameObject test2;
	public GameObject test3;
	public GameObject test4;
	public GameObject test5;
	public GameObject test6;
	public GameObject test7;
	public GameObject test8;
	public GameObject test9;
	public GameObject test10;
	
	void Start(){
		resetAll();
	}
	
	void Update(){
		
	}
	
	void resetAll(){
		test1.SetActive(false);
		test2.SetActive(false);
		test3.SetActive(false);
		test4.SetActive(false);
		test5.SetActive(false);
		test6.SetActive(false);
		test7.SetActive(false);
		test8.SetActive(false);
		test9.SetActive(false);
		test10.SetActive(false);
	}
	
	void OnGUI(){
		if(GUI.Button(new Rect(10, 10, 150, 20), "original LG")){
			resetAll();
			test1.SetActive(true);
		}
		if(GUI.Button(new Rect(10, 30, 150, 20), "w/o Shadows")){
			resetAll();
			test2.SetActive(true);
		}
		if(GUI.Button(new Rect(10, 50, 150, 20), "w/o shadows static")){
			resetAll();
			test3.SetActive(true);
		}
		if(GUI.Button(new Rect(10, 70, 150, 20), "unlit transparent")){
			resetAll();
			test4.SetActive(true);
		}
		if(GUI.Button(new Rect(10, 90, 150, 20), "unlit transparent")){
			resetAll();
			test5.SetActive(true);
		}
		if(GUI.Button(new Rect(10, 110, 150, 20), "transparent vertexlit")){
			resetAll();
			test6.SetActive(true);
		}
		if(GUI.Button(new Rect(10, 130, 150, 20), "tk2d blend2tex")){
			resetAll();
			test7.SetActive(true);
		}
		if(GUI.Button(new Rect(10, 150, 150, 20), "tk2d blendVertex")){
			resetAll();
			test8.SetActive(true);
		}
		if(GUI.Button(new Rect(10, 170, 150, 20), "transparent vertexlit")){
			resetAll();
			test9.SetActive(true);
		}
		if(GUI.Button(new Rect(10, 190, 150, 20), "unlit trans colored overlay")){
			resetAll();
			test10.SetActive(true);
		}
	}
}
