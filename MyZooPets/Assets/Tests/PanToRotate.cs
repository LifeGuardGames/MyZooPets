using UnityEngine;
using System.Collections;

public class PanToRotate : MonoBehaviour {
    public Camera mainCamera;
    public float speed;
    private Vector2 startTouchPos;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.touchCount > 0) {

            Touch touch = Input.touches[0];

            switch (touch.phase) {
                case TouchPhase.Began:
                    startTouchPos = touch.position;                 
                break;
                case TouchPhase.Ended:
                break;

                case TouchPhase.Canceled:
                break;

                case TouchPhase.Stationary:
                break;

                case TouchPhase.Moved:
                    Vector2 touchDeltaPosition = touch.deltaPosition;
                   
                    float rotate = touchDeltaPosition.x * speed;

                    print(rotate);
                    mainCamera.transform.Rotate(0, -rotate, 0);
                break;
            }
        }
        else {

        }	
	}
}
