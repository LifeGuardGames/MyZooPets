using UnityEngine;
using System.Collections;

public class RescueShaker : MonoBehaviour {

    public Texture2D statBarTexture;
    public Texture2D statBarVerFrame;
    public Texture2D statBarVerFillGreen;

    // native dimensions
    private const float NATIVE_WIDTH = 1280.0f;
    private const float NATIVE_HEIGHT = 800.0f;

    private Vector2 shakeBarOffset = new Vector2(60, 15);
    private Vector2 shakeBarloc = new Vector2(NATIVE_WIDTH / 2, NATIVE_HEIGHT / 2);

    float shakeValue = 0f;
    float shakeTarget = 1f;
    public RescueBody rescueBody;

    void Start(){
        renderer.enabled = false;
        // set in InhalerGameManager.DestroyAndRecreatePrefabs()
        // rescueBody = GameObject.Find("RescueBody").GetComponent<RescueBody>();
    }

    void Update(){
        if (InhalerLogic.CurrentStep != 2){
            return;
        }
        renderer.enabled = true;

        if(Input.touchCount > 0){
            Touch touch = Input.GetTouch(0);
            if (isTouchingObject(touch)){
                if (touch.phase == TouchPhase.Moved){
                    shakeValue += Time.deltaTime;
                    rescueBody.Shake();
                }
            }
        }
        if (shakeValue >= shakeTarget){
            // todo: play sound
            Debug.Log("shake target reached");

            if (InhalerLogic.IsCurrentStepCorrect(2)){
                if (!InhalerLogic.IsDoneWithGame()){
                    InhalerLogic.NextStep();
                }
                renderer.enabled = false;
            }
        }
    }

    void OnGUI(){
        if (InhalerLogic.CurrentStep != 2){
            return;
        }
        if (NATIVE_WIDTH != Screen.width || NATIVE_HEIGHT != Screen.height){
            float horizRatio = Screen.width/NATIVE_WIDTH;
            float vertRatio = Screen.height/NATIVE_HEIGHT;
            GUI.matrix = Matrix4x4.TRS(new Vector3(0, 0, 0), Quaternion.identity, new Vector3(horizRatio, vertRatio, 1));
        }

        GUI.DrawTexture(new Rect(shakeBarloc.x,shakeBarloc.y,100,100), statBarTexture);
        GUI.DrawTexture(new Rect(shakeBarloc.x + shakeBarOffset.x,shakeBarloc.y + shakeBarOffset.y,25,70),statBarVerFrame);
        GUI.DrawTexture(new Rect(shakeBarloc.x + shakeBarOffset.x,shakeBarloc.y + shakeBarOffset.y+(70-70*shakeValue/shakeTarget),25, 70 * Mathf.Clamp01(shakeValue/shakeTarget)),statBarVerFillGreen, ScaleMode.StretchToFill, true, 1f);
        // GUI.DrawTexture(new Rect(shakeBarloc.x + healthIconOffset.x,shakeBarloc.y + healthIconOffset.y,60,60),healthIcon, ScaleMode.ScaleToFit, true, 0f);

    }

    bool isTouchingObject(Touch touch){
        Ray ray = Camera.main.ScreenPointToRay(touch.position);
        RaycastHit hit ;

        //Check if there is a collider attached already, otherwise add one on the fly
        if(collider == null){
            gameObject.AddComponent(typeof(BoxCollider));
        }

        if (Physics.Raycast (ray, out hit)) {
            if(hit.collider.gameObject == this.gameObject){
                return true;
            }
        }
        return false;
    }
}