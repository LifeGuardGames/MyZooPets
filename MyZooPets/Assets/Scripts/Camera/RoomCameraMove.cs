using UnityEngine;
using System.Collections;

public class RoomCameraMove : CameraMove{

    protected GameObject shelf;
    protected Vector3 shelfFinalFaceDirection = new Vector3(0,353.8f, 0);

    protected Vector3 petSideFinalPosition;
    protected Vector3 petSideFinalFaceDirection = new Vector3(15.54f, 0, 0);

    protected GameObject slotMachine;
    protected Vector3 slotMachineFinalDirection = new Vector3(17.8f, 20.47f, 0f);

    protected GameObject realInhaler;
    protected Vector3 realInhalerFinalDirection = new Vector3(36f, 0, 0);

    protected GameObject teddyInhaler;
    protected Vector3 teddyInhalerFinalDirection = new Vector3(15.54f, 0, 0);

    protected Vector3 petCameraOffsetRoom = new Vector3(4.83f, 8.253f, -10.36f); // use this whenever changing petSideFinalPosition
    protected Vector3 realInhalerCameraOffset = new Vector3(0.29f, 3.24f, -6.19f); // use this whenever changing realInhalerFinalPosition
    protected Vector3 teddyInhalerCameraOffset = new Vector3(0.99f, 2.02f, -10.36f); // use this whenever changing teddyInhalerFinalPosition
    protected Vector3 slotMachineCameraOffset = new Vector3(-0.2f, 9.95f, -8.2f); // use this whenever changing slotMachineFinalPosition
    protected Vector3 shelfCameraOffset = new Vector3(-39.4f, -0.29f, 2.08f); // use this whenever changing shelfFinalPosition
    // this way, the camera will always go to the pet

    protected GameObject spritePet;

    public override void Init(){
        spritePet = GameObject.Find("SpritePet");

        slotMachine = GameObject.Find("GO_SlotMachine");
        realInhaler = GameObject.Find("GO_RealInhaler");
        teddyInhaler = GameObject.Find("GO_TeddyInhaler");
        shelf = GameObject.Find("GO_Shelf");
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
                    Vector3 slotMachineFinalPosition = slotMachine.transform.localPosition + slotMachineCameraOffset;
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

                    default:
                    Debug.Log("Invalid zoom item!");
                    return;
                    break;
                }
                zoomed = true;
                LockCameraMove();

            }
        }
    }

}
