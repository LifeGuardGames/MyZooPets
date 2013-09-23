using UnityEngine;
using System.Collections;

/*
    TO DO: Get rid of camera offset. Use absolute position instead
*/
public class RoomCameraMove : CameraMove{

    public GameObject dojo;
    public Vector3 dojoFinalPosition;
    public Vector3 dojoFinalFaceDirection; 

    public GameObject badgeBoard;
    public Vector3 badgeBoardFinalPosition;
    public Vector3 badgeBoardFinalFaceDirection;
	
	public GameObject yardSign;
	public Vector3 yardFinalPosition;
    public Vector3 yardFinalDirection;

    public GameObject realInhaler;
	public Vector3 realInhalerFinalPosition; 
    public Vector3 realInhalerFinalDirection;

    public GameObject spritePet;
    private  Vector3 petCameraOffsetRoom = new Vector3(4.83f, 8.253f, -10.36f); // use this whenever changing petSideFinalPosition
    private Vector3 petSideFinalPosition;
    private Vector3 petSideFinalFaceDirection = new Vector3(15.54f, 0, 0);

    public override void ZoomToggle(ZoomItem item){
        if(!isCameraMoving){
            if(!zoomed){
                switch (item){
                    case ZoomItem.Pet:
                    petSideFinalPosition = spritePet.transform.position + petCameraOffsetRoom;
                    CameraWorldTransformEnterMode(petSideFinalPosition, petSideFinalFaceDirection, 0.5f);
                    break;
					
					case ZoomItem.YardSign:
					CameraTransformLoadLevel(yardFinalPosition, yardFinalDirection, 2f, yardSign);	// Pass in gameobject to load level callback
					break;

                    case ZoomItem.RealInhaler:
                    CameraTransformLoadLevel(realInhalerFinalPosition, realInhalerFinalDirection, 2f, realInhaler); // Pass in gameobject to load level callback
                    break;

                    case ZoomItem.BadgeBoard:
                    CameraTransformEnterMode(badgeBoardFinalPosition,badgeBoardFinalFaceDirection, 1.0f);
                    break;

                    case ZoomItem.Dojo:
					CameraTransformLoadLevel(dojoFinalPosition, dojoFinalFaceDirection, 2f, dojo);	// Pass in gameobject to load level callback
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
