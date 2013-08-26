using UnityEngine;
using System.Collections;

public class InhalerUtility{
    //If no layer mask available ray is casted to default layer
    public static bool IsTouchingObject(Touch touch, GameObject target){
        return IsTouchingObject(touch, target, 1 << 0);
    }

    public static bool IsTouchingObject(Touch touch, GameObject target, int maskLayer){
        Ray ray = Camera.main.ScreenPointToRay(touch.position);
        RaycastHit hit ;
        bool retVal = false;
        if (Physics.Raycast (ray, out hit, 100, maskLayer)) {
            if(hit.collider.gameObject == target){
				retVal = true;

            }
        }
        return retVal;
    }
}