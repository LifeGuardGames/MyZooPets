using UnityEngine;
using System.Collections;

public class YardCameraMove : CameraMove{

    public GameObject runnerEntrance;
    public Vector3 runnerFinalPosition;
    public Vector3 runnerFinalFaceDirection;

    public GameObject spritePet;

    public override void ZoomToggle(ZoomItem item){
        if(!isCameraMoving){
            if(!zoomed){
                switch (item){
                    case ZoomItem.RunnerGame:
                    CameraTransformLoadLevel(runnerFinalPosition, runnerFinalFaceDirection, 2f, runnerEntrance);
                    break;
                    default:
                    Debug.Log("Invalid zoom item!");
                    break;
                }
                zoomed = true;
                LockCameraMove();

            }
        }
    }
}
