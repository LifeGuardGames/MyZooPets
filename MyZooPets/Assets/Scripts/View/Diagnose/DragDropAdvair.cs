using UnityEngine;
using System.Collections;
using System;

public class DragDropAdvair : MonoBehaviour {
    public GameObject target;
    public string functionName; //function to be called from target when advair drop on pet

    private Transform mTrans;
    private bool mIsDragging = false;
    private Vector3 originalPos;

    void Awake(){
        mTrans = transform;
    }

    //Start or stop the drag operation
    void OnPress(bool isPressed){
        if(enabled){
            if(isPressed){
                if(!UICamera.current.stickyPress) UICamera.current.stickyPress = true;
            }

            mIsDragging = false;
            Collider col = GetComponent<Collider>();
            
            //collider needs to be disabled while dragging otherwise it will catch the OnDrop event
            if(col != null) col.enabled = !isPressed; 
            if(!isPressed) Drop();
        }
    }	

    void OnDrag(Vector2 delta){
        if(enabled && UICamera.currentTouchID > -2){
            if(!mIsDragging){ //start dragging if not dragging yet
                mIsDragging = true;
                originalPos = mTrans.position;
            }else{ //move position
                mTrans.localPosition += (Vector3)delta;
            }
        }
    }

    private void Drop(){
        Collider col = UICamera.lastHit.collider;
        if(col.gameObject.name == "SpritePet"){ //take action if drop on pet
            this.GetComponent<UISprite>().alpha = 0;
            if(target != null){
                target.SendMessage(functionName, true, SendMessageOptions.DontRequireReceiver);
            }
        }else{ //not on target. back to original position
            mTrans.position = originalPos;
        }
    }


}
