using UnityEngine;

public class TextureRenderer : MonoBehaviour {

	public string sortingLayerName;
	public int sortingLayerOrder;

	void Start() {
		GetComponent<MeshRenderer>().sortingLayerName = sortingLayerName;
		GetComponent<MeshRenderer>().sortingOrder = sortingLayerOrder;
	}
}
