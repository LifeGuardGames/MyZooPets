using UnityEngine;
using System.Collections;

public class RotateInRoom : MonoBehaviour {

    float currentYRotation;
    public float rotationIncrement = 72;
    Hashtable optional = new Hashtable();
    bool lockRotation;
	// Use this for initialization
	void Start () {
        currentYRotation = transform.rotation.y;
        optional.Add("onComplete", "FinishedRotation");

        // todo: remove after testing
        // Invoke("RotateRight", 5);
        // Invoke("RotateLeft", 10);
	}

    public void RotateRight(){
        // todo: call lock on ClickManager?
        if (!lockRotation){
            lockRotation = true;
            currentYRotation += rotationIncrement;
            LeanTween.rotateY(gameObject, currentYRotation, 1.0f, optional);
        }
    }

    public void RotateLeft(){
        // todo: call lock on ClickManager?
        if (!lockRotation){
            lockRotation = true;
            currentYRotation -= rotationIncrement;
            LeanTween.rotateY(gameObject, currentYRotation, 1.0f, optional);
        }
    }

    void FinishedRotation(){
        lockRotation = false;
    }
}
