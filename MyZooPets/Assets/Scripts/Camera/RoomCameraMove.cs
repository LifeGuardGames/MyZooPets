using UnityEngine;
using System.Collections;

public class RoomCameraMove : CameraMove{

    protected GameObject shelf;
    protected Vector3 shelfFinalFaceDirection = new Vector3(0,353.8f, 0);

    protected GameObject dojo;
    protected Vector3 dojoFinalFaceDirection = new Vector3(0, 0, 0);

    protected GameObject badgeBoard;
    protected Vector3 badgeBoardFinalFaceDirection = new Vector3(0, 0, 0);

    protected Vector3 petSideFinalPosition;
    protected Vector3 petSideFinalFaceDirection = new Vector3(15.54f, 0, 0);

    protected GameObject slotMachine;
	protected Vector3 slotMachineFinalPosition = new Vector3(13.82f, 9.41f, 30.16f);
    protected Vector3 slotMachineFinalDirection = new Vector3(8.79f, 45.47f, 0.3f);

    protected GameObject realInhaler;
    protected Vector3 realInhalerFinalDirection = new Vector3(36f, 0, 0);

    protected GameObject teddyInhaler;
    protected Vector3 teddyInhalerFinalDirection = new Vector3(15.54f, 0, 0);

    protected Vector3 petCameraOffsetRoom = new Vector3(4.83f, 8.253f, -10.36f); // use this whenever changing petSideFinalPosition
    protected Vector3 realInhalerCameraOffset = new Vector3(0.29f, 3.24f, -6.19f); // use this whenever changing realInhalerFinalPosition
    protected Vector3 teddyInhalerCameraOffset = new Vector3(0.99f, 2.02f, -10.36f); // use this whenever changing teddyInhalerFinalPosition
    protected Vector3 slotMachineCameraOffset = new Vector3(-0.2f, 9.95f, -8.2f); // use this whenever changing slotMachineFinalPosition
    protected Vector3 shelfCameraOffset = new Vector3(-39.4f, -0.29f, 2.08f); // use this whenever changing shelfFinalPosition
//    protected Vector3 badgeBoardCameraOffset = new Vector3(-24.53f, 0.8f, 54.89f); // use this whenever changing badgeBoardFinalPosition
	protected Vector3 badgeBoardCameraOffset = new Vector3(0f, 0f, 0f); // use this whenever changing badgeBoardFinalPosition
	protected Vector3 dojoCameraOffset = new Vector3(40.5f,8f,20f);
    // this way, the camera will always go to the pet

    protected GameObject spritePet;

    protected override void Start(){
        base.Start();
        spritePet = GameObject.Find("SpritePet");

        slotMachine = GameObject.Find("GO_SlotMachine");
        realInhaler = GameObject.Find("GO_RealInhaler");
        teddyInhaler = GameObject.Find("GO_TeddyInhaler");
        shelf = GameObject.Find("GO_Shelf");
        badgeBoard = GameObject.Find("GO_HousePlaque");
        dojo = GameObject.Find("GO_Dojo");
    }

    public override void ZoomToggle(ZoomItem item){
        if(!isCameraMoving){
            if(!zoomed){
                switch (item){
                    case ZoomItem.Pet:
                    petSideFinalPosition = spritePet.transform.position + petCameraOffsetRoom;
                    CameraWorldTransformEnterMode(petSideFinalPosition, petSideFinalFaceDirection, 0.5f);
                    break;

                    case ZoomItem.TrophyShelf:
                    Vector3 shelfFinalPosition = shelf.transform.position + shelfCameraOffset;
                    CameraTransformEnterMode(shelfFinalPosition,shelfFinalFaceDirection, 1.0f);
                    break;

                    case ZoomItem.SlotMachine:
                    //Vector3 slotMachineFinalPosition = slotMachine.transform.localPosition + slotMachineCameraOffset;
                    CameraTransformLoadLevel(slotMachineFinalPosition, slotMachineFinalDirection, 2f, "SlotMachineGame");
                    break;

                    case ZoomItem.RealInhaler:
                    Vector3 realInhalerFinalPosition = realInhaler.transform.localPosition + realInhalerCameraOffset;
                    CameraTransformLoadLevel(realInhalerFinalPosition, realInhalerFinalDirection, 2f, "InhalerGamePet");
                    break;

                    case ZoomItem.PracticeInhaler:
                    Vector3 teddyInhalerFinalPosition = teddyInhaler.transform.localPosition + teddyInhalerCameraOffset;
                    CameraTransformLoadLevel(teddyInhalerFinalPosition, teddyInhalerFinalDirection, 2f, "InhalerGameTeddy");
                    break;

                    case ZoomItem.BadgeBoard:
                    Vector3 badgeBoardFinalPosition = badgeBoard.transform.localPosition + badgeBoardCameraOffset;
					// TODO-s i dont understand this code ^, hardcoding. needs fixing!
                    CameraTransformEnterMode(new Vector3(.35f, 9f, 24.8f),badgeBoardFinalFaceDirection, 1.0f);
                    break;

                    case ZoomItem.Dojo:
                    Vector3 dojoFinalPosition = dojo.transform.position + dojoCameraOffset;
					// TODO-s i dont understand this code ^, hardcoding. needs fixing!
                    CameraTransformEnterMode(new Vector3(.19f, 6.82f, 31.34f), dojoFinalFaceDirection, 1.0f);
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
