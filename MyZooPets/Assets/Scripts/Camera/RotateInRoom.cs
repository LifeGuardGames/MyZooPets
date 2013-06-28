using UnityEngine;
using System.Collections;

public class RotateInRoom : MonoBehaviour {

    float currentYRotation;
    public float rotationIncrement = 72;
    bool inverse = true;
    Hashtable optional = new Hashtable();
    bool lockRotation;
	// Use this for initialization
	void Start () {
        currentYRotation = transform.eulerAngles.y;
        optional.Add("onComplete", "FinishedRotation");
        if (inverse){
            rotationIncrement = - rotationIncrement;
        }

        // todo: remove after testing
        // Invoke("RotateRight", 5);
        // Invoke("RotateLeft", 10);
	}

    public void RotateRight(){
        if (!lockRotation){
            if (ClickManager.CanRespondToTap()){
                lockRotation = true;
                currentYRotation += rotationIncrement;
                LeanTween.rotateY(gameObject, currentYRotation, 1.0f, optional);
            }
        }
    }

    public void RotateLeft(){
        if (!lockRotation){
            if (ClickManager.CanRespondToTap()){
                lockRotation = true;
                currentYRotation -= rotationIncrement;
                LeanTween.rotateY(gameObject, currentYRotation, 1.0f, optional);
            }
        }
    }

    void FinishedRotation(){
        lockRotation = false;
        currentYRotation = transform.eulerAngles.y; // normalize angle
    }
}
