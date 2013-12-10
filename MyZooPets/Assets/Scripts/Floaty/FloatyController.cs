using UnityEngine;
using System.Collections;

//-----------------------------------------
// Starts the floating tween and alpha tween
// self destructs when it's completed
//-----------------------------------------
public class FloatyController : MonoBehaviour {
    public Vector3 floatingUpPos;
    public float floatingTime;

    private NGUIAlphaTween alphaTween; 

    void Awake(){
        alphaTween = GetComponent<NGUIAlphaTween>();
    }

	void Start () {
        FloatUp();
        alphaTween.StartAlphaTween();
	}

    private void FloatUp(){
        Hashtable optional = new Hashtable();
        optional.Add("onCompleteTarget", gameObject);
        optional.Add("onComplete", "SelfDestruct");
        LeanTween.moveLocal(gameObject, floatingUpPos, floatingTime, optional);
    }

    private void SelfDestruct(){
        Destroy(gameObject);
    }
}
