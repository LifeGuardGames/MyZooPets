//source: https://code.google.com/p/pixelplacement/source/browse/trunk/unity/com/pixelplacement/scripts/SwipeDetection.cs?spec=svn158&r=158

// To toggle inverting of directions, modify UserNavigation.cs

using UnityEngine;
using System.Collections;

public enum Swipe{Left,Right,Up,Down};

public class SwipeDetection : MonoBehaviour {
    // Native dimensions
    private const float NATIVE_WIDTH = 1280.0f;
    private const float NATIVE_HEIGHT = 800.0f;

    private int layerNGUI;

    float screenDiagonalSize;
    float minSwipeDistancePixels;
    bool touchStarted;
    Vector2 touchStartPos;
    float flickTimer = 0;
    public float minSwipeDistance = .025f;
    public float flickTime = 0.5f;

    public static event System.Action<Swipe> OnSwipeDetected;
    private static bool swipeCancelled;
    private Camera NGUICamera;

    void Start() {
        screenDiagonalSize = Mathf.Sqrt(NATIVE_WIDTH * NATIVE_WIDTH + NATIVE_HEIGHT * NATIVE_HEIGHT);
        minSwipeDistancePixels = minSwipeDistance * screenDiagonalSize;

        layerNGUI = LayerMask.NameToLayer("NGUI");
        NGUICamera = NGUITools.FindCameraForLayer(layerNGUI);
        if (NGUICamera == null){
            Debug.LogError("NGUI camera not found!");
        }
    }

    void Update() {
        if (Input.touchCount > 0) {

            var touch = Input.touches[0];
            flickTimer += Time.deltaTime;

            switch (touch.phase) {

                case TouchPhase.Began:
                flickTimer = 0;
                touchStarted = true;
                touchStartPos = touch.position;
                if (IsTouchingNGUI(touch.position)){
                    swipeCancelled = true;
                }
                break;

                case TouchPhase.Ended:
                if (!swipeCancelled && touchStarted) {
                    TestForSwipeGesture(touch);
                    touchStarted = false;
                }
                break;

                case TouchPhase.Canceled:
                touchStarted = false;
                break;

                case TouchPhase.Stationary:
                break;

                case TouchPhase.Moved:
                break;
            }
        }
        else {
            swipeCancelled = false;
        }
    }

    bool IsTouchingNGUI(Vector2 screenPos){

        Ray ray = NGUICamera.ScreenPointToRay (screenPos);
        RaycastHit hit;

        // Raycast
        if (Physics.Raycast(ray, out hit, Mathf.Infinity)) {

            if (hit.transform.gameObject.layer == layerNGUI) {
                return true;
            }
        }
        return false;
    }

    // Use this when dragging an item.
    public static void CancelSwipe(){
        swipeCancelled = true;
    }

    void TestForSwipeGesture(Touch touch){
        Vector2 lastPos = touch.position;
        float distance = Vector2.Distance(lastPos, touchStartPos);

        if (distance > minSwipeDistancePixels && flickTimer <= flickTime) {
            float dy = lastPos.y - touchStartPos.y;
            float dx = lastPos.x - touchStartPos.x;

            float angle = Mathf.Rad2Deg * Mathf.Atan2(dx, dy);

            angle = (360 + angle - 45) % 360;

            if (angle < 90) {
                OnSwipeDetected(Swipe.Right);
                } else if (angle < 180) {
                    OnSwipeDetected(Swipe.Down);
                    } else if (angle < 270) {
                        OnSwipeDetected(Swipe.Left);
                        } else {
                            OnSwipeDetected(Swipe.Up);
                        }
                    }
                }
            }
