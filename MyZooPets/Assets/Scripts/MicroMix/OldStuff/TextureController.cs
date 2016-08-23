using UnityEngine;
using System.Collections;

/* We must make our own mesh for now if we want to use this because it sems like Unity's plane mesh is not exactly just a tesselation of triangles
 * So we must make our own and then use the shaded out below to destroy them for now
 */
public class TextureController : MonoBehaviour{

	// Use this for initialization
	void Start(){
		Mesh mesh = GetComponent<MeshFilter>().mesh;
		Vector3[] vertices = mesh.vertices;
		for(int i = 0; i < vertices.Length; i++){
			vertices[i] = Normal(vertices[i]);
		}
		mesh.vertices = vertices;
	}

	void Update(){
		Mesh mesh = GetComponent<MeshFilter>().mesh;
		Vector3[] vertices = mesh.vertices;
		Vector3[] normals = mesh.normals;
		Vector3 mousePosition = Input.mousePosition;
		Vector3 realPos = CameraUtils.ScreenToWorldPointZ(Camera.main, mousePosition, 50f);
		float distance = Mathf.Infinity;
		Vector3 closest = Vector3.zero;
		int vertIndex = -1; //We have found the index of the vertex we want to remove
		if(true){//Input.GetKeyDown(KeyCode.Space)){
			for(int i = 0; i < mesh.vertices.Length; i++){
				if(Vector3.SqrMagnitude(realPos - mesh.vertices[i]) < distance){
					distance = Vector3.SqrMagnitude(realPos - mesh.vertices[i]);
					closest = mesh.vertices[i];
					vertIndex = i;
				}
			}
			/*int triIndex = -1; //Now we must find the index of that index (as a value) in triangles
			for(int i = 0; i < mesh.triangles.Length; i++){
				if(mesh.triangles[i] == vertIndex){
					triIndex = i;
				}
			}
			string s = triIndex + ": ";
			for(int i = 0; i < 3; i++){
				s += GetIndeces(triIndex)[i] + ",";
			}
			Debug.Log(s);
			//Vector3[] newVerts = new Vector3[mesh.vertices.Length-3]; //So go through and see if their value is one of the values at triangles[GetIndeces]
			//Vector3[] newNorms = new Vector3[newVerts.Length];
			int[] newTris = new int[mesh.triangles.Length - 3];
			int index = 0;

			for(int i = 0; i < mesh.triangles.Length; i++){
				if(!Contains(newTris, i)){
					newTris[index] = mesh.triangles[i];
					Debug.Log(i + ":" + index);
					index++;
				}
			}

			mesh.triangles = newTris;*/
			vertices[vertIndex] = new Vector3(0, 0, 0);
			mesh.vertices = vertices;
			Debug.Log(vertIndex);
		}
	}

	Vector3 Normal(Vector3 input){
		return new Vector3(input.x, input.z);
	}

	int[] GetIndeces(int triIndex){
		switch(triIndex % 3){
		case 0: 
			return new int[3]{ 0 + triIndex, 1 + triIndex, 2 + triIndex };
		case 1:
			return new int[3]{ -1 + triIndex, 0 + triIndex, 1 + triIndex };
		case 2:
			return new int[3]{ -1 + triIndex, -2 + triIndex, 0 + triIndex };
		default:
			Debug.LogWarning("Error");
			return new int[0];
		}
	}

	bool Contains(int[] array, int value){
		for(int i = 0; i < array.Length; i++){
			if(array[i] == value){
				return true;
			}
		}
		return false;
	}
}
