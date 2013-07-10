using UnityEngine;
using System.Collections;

public class RoomCameraMove : CameraMove{

    protected GameObject shelf;
    // protected Vector3 shelfFinalPosition = new Vector3 (10.7f,1.6f,6.6f);
    protected Vector3 shelfFinalPosition;
    // protected Vector3 shelfFinalFaceDirection = new Vector3(7.34f,90.11f,359.62f);
    protected Vector3 shelfFinalFaceDirection = new Vector3(0,353.8f, 0);

    // protected Vector3 petSideFinalPosition = new Vector3(3f, 1.3f, -15f);
    // protected Vector3 petSideFinalPosition = new Vector3(4.83f, 8.6f, 12.64f);
    protected Vector3 petSideFinalPosition;
    protected Vector3 petSideFinalFaceDirection = new Vector3(15.54f, 0, 0);

    protected Vector3 gameboyFinalPosition = new Vector3(-11.9f, -1.6f, -1.4f);
    protected Vector3 gameboyFinalDirection = new Vector3(27f, 0, 1.35f);

    // protected Vector3 slotMachineFinalPosition = new Vector3(-9.66f, 9.95f, 32.58f);
    protected GameObject slotMachine;
    protected Vector3 slotMachineFinalPosition;
    protected Vector3 slotMachineFinalDirection = new Vector3(17.8f, 20.47f, 0f);

    protected GameObject realInhaler;
    protected Vector3 realInhalerFinalPosition;
    protected Vector3 realInhalerFinalDirection = new Vector3(36f, 0, 0);

    protected GameObject teddyInhaler;
    protected Vector3 teddyInhalerFinalPosition;
    protected Vector3 teddyInhalerFinalDirection = new Vector3(15.54f, 0, 0);

    protected Vector3 petCameraOffsetRoom = new Vector3(4.83f, 8.253f, -10.36f); // use this whenever changing petSideFinalPosition
    protected Vector3 petCameraOffsetYard = new Vector3(1.8f, 2.87f, -3.1f); // use this whenever changing petSideFinalPosition
    protected Vector3 realInhalerCameraOffset = new Vector3(0.29f, 3.24f, -6.19f); // use this whenever changing realInhalerFinalPosition
    protected Vector3 teddyInhalerCameraOffset = new Vector3(0.99f, 2.02f, -10.36f); // use this whenever changing teddyInhalerFinalPosition
    protected Vector3 slotMachineCameraOffset = new Vector3(-0.2f, 9.95f, -8.2f); // use this whenever changing slotMachineFinalPosition
    protected Vector3 shelfCameraOffset = new Vector3(-39.4f, -0.29f, 2.08f); // use this whenever changing shelfFinalPosition
    // this way, the camera will always go to the pet

    protected GameObject spritePet;

    public override void Init(){
        //Camera move is used in multiple scenes so it needs to know what specific
        //gameobjects to load at diff scenes
        spritePet = GameObject.Find("SpritePet");

        slotMachine = GameObject.Find("SlotMachine");
        realInhaler = GameObject.Find("RealInhaler");
        teddyInhaler = GameObject.Find("TeddyInhaler");
        shelf = GameObject.Find("Shelf");

        initPosition = gameObject.transform.position;
        // initFaceDirection = new Vector3(15.54f, 0, 0);
        initFaceDirection = gameObject.transform.eulerAngles;
    }

    public override void ZoomToggle(ZoomItem item){
        if(!isCameraMoving){
            if(!zoomed){
                switch (item){
                    case ZoomItem.Pet:
                    petSideFinalPosition = spritePet.transform.localPosition + petCameraOffsetRoom;
                    CameraTransformEnterMode(petSideFinalPosition,petSideFinalFaceDirection, 0.5f);
                    break;

                    case ZoomItem.TrophyShelf:
                    shelfFinalPosition = shelf.transform.position + shelfCameraOffset;
                    CameraTransformEnterMode(shelfFinalPosition,shelfFinalFaceDirection, 1.0f);
                    break;

                    case ZoomItem.SlotMachine:
                    slotMachineFinalPosition = slotMachine.transform.localPosition + slotMachineCameraOffset;
                    CameraTransformLoadLevel(slotMachineFinalPosition, slotMachineFinalDirection, 2f, "SlotMachineGame");
                    break;

                    case ZoomItem.RealInhaler:
                    realInhalerFinalPosition = realInhaler.transform.localPosition + realInhalerCameraOffset;
                    CameraTransformLoadLevel(realInhalerFinalPosition, realInhalerFinalDirection, 2f, "InhalerGamePet");
                    break;

                    case ZoomItem.PracticeInhaler:
                    teddyInhalerFinalPosition = teddyInhaler.transform.localPosition + teddyInhalerCameraOffset;
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
