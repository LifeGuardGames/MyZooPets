//source: https://code.google.com/p/pixelplacement/source/browse/trunk/unity/com/pixelplacement/scripts/SwipeDetection.cs?spec=svn158&r=158
using UnityEngine;
using System.Collections;

public enum Swipe{Left,Right,Up,Down};

public class SwipeDetection : MonoBehaviour {
    // Native dimensions
    private const float NATIVE_WIDTH = 1280.0f;
    private const float NATIVE_HEIGHT = 800.0f;

    float screenDiagonalSize;
    float minSwipeDistancePixels;
    bool touchStarted;
    Vector2 touchStartPos;
    float flickTimer = 0;
    public float minSwipeDistance = .025f;
    public float flickTime = 0.5f;
    public static event System.Action<Swipe> OnSwipeDetected;

    void Start() {
        screenDiagonalSize = Mathf.Sqrt(NATIVE_WIDTH * NATIVE_WIDTH + NATIVE_HEIGHT * NATIVE_HEIGHT);
        minSwipeDistancePixels = minSwipeDistance * screenDiagonalSize;
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
                break;

                case TouchPhase.Ended:
                if (touchStarted) {
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
