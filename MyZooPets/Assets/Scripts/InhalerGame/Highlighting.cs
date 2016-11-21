using UnityEngine;
using System.Collections;

public class Highlighting : MonoBehaviour {

	public bool expand = false;
	public bool forArrow = false;
	// Use this for initialization
	void Start () {

	}

	// Update is called once per frame
	void Update () {
		if(Input.touchCount > 0){
			Ray myRay = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;
			if(Physics.Raycast(myRay,out hit)){
				if(hit.collider.name == this.name)
				if(!forArrow){
					if (expand){
						transform.localScale = new Vector3 (1.1f,1.1f,1.1f);
					}
					else {
						Glow();
					}
				}
				else{
					if (expand){
						GameObject.Find("PetSprite").transform.localScale = new Vector3(1.1f,1.1f,1.1f);
					}
					else {
						Glow(GameObject.Find("PetSprite"));
					}
				}
			}
		}
		else{
			if(!forArrow){
				if (expand){
					transform.localScale = new Vector3(1,1,1);
				}
				else {
					UnGlow();
				}
			}
			else {
				if (expand){
					GameObject.Find("PetSprite").transform.localScale = new Vector3(1,1,1);
				}
				else {
					UnGlow(GameObject.Find("PetSprite"));
				}
			}

		}
	}

	void Glow(){
		// bright glow
		// renderer.material.shader = Shader.Find("Mobile/Particles/Additive");
	}

	void Glow(GameObject g){
		// bright glow
		// g.renderer.material.shader = Shader.Find("Mobile/Particles/Additive");
	}

	void UnGlow(){
		// normal
		renderer.material.shader = Shader.Find("Unlit/Transparent Cutout");
	}

	void UnGlow(GameObject g){
		// normal
		g.renderer.material.shader = Shader.Find("Unlit/Transparent Cutout");
	}

	void OnDestroy(){
		DestroyImmediate(renderer.material);
	}
}