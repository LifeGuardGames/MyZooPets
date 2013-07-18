using UnityEngine;
using System.Collections;

public class YardCameraMove : CameraMove{

    protected Vector3 petSideFinalPosition;
    protected Vector3 petSideFinalFaceDirection = new Vector3(15.54f, 0, 0);

    protected Vector3 petCameraOffsetYard = new Vector3(1.8f, 2.87f, -3.1f); // use this whenever changing petSideFinalPosition
    // this way, the camera will always go to the pet

    protected GameObject spritePet;

    public override void Init(){
        spritePet = GameObject.Find("SpritePet");
    }

    public override void ZoomToggle(ZoomItem item){
        if(!isCameraMoving){
            if(!zoomed){
                switch (item){
                    case ZoomItem.Pet:
                    // todo: change or review this after pet moves along with camera

                    // petSideFinalPosition = spritePet.transform.localPosition + petCameraOffsetYard;
                    // while pet doesn't move along with slide:
                    petSideFinalPosition = spritePet.transform.position + petCameraOffsetYard;
                    CameraWorldTransformEnterMode(petSideFinalPosition,petSideFinalFaceDirection, 0.5f);
                    break;

                    default:
                    Debug.Log("Invalid zoom item!");
                    return;
                }
                zoomed = true;
                LockCameraMove();

            }
        }
    }
}
