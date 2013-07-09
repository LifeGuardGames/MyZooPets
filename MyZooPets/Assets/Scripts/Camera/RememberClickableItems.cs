using UnityEngine;
using System.Collections;

/*
    This class is to control what items in the room can be clicked, and what cannot be
    at any specific time.

    Since we have multiple partitions in the room, and are intentionally placing items
    so that they can be visible from other partitions (to hint to the user that there
    are other rooms), we will need to prevent the user from clicking on these items.

    Usage:
        Call the corresponding method when switching to a different room partition
        from RotateInRoom.cs
*/

public class RememberClickableItems : MonoBehaviour {

    // partition 1
    public GameObject notebook;
    public GameObject calendar;
    public GameObject laptop;
    public GameObject realInhaler;
    public GameObject practiceInhaler;
    public GameObject slotMachine;

    // partition 2
    public GameObject trophyShelf;
    public GameObject firstTrophy;

    public void ActivatePartition1(){
        notebook.collider.enabled = true;
        calendar.collider.enabled = true;
        laptop.collider.enabled = true;
        realInhaler.collider.enabled = true;
        practiceInhaler.collider.enabled = true;
        slotMachine.collider.enabled = true;

        trophyShelf.collider.enabled = false;
        firstTrophy.collider.enabled = false;
    }

    public void ActivatePartition2(){
        notebook.collider.enabled = false;
        calendar.collider.enabled = false;
        laptop.collider.enabled = false;
        realInhaler.collider.enabled = false;
        practiceInhaler.collider.enabled = false;
        slotMachine.collider.enabled = false;

        trophyShelf.collider.enabled = true;
        firstTrophy.collider.enabled = true;
    }

    public void ActivatePartition3(){
        notebook.collider.enabled = false;
        calendar.collider.enabled = false;
        laptop.collider.enabled = false;
        realInhaler.collider.enabled = false;
        practiceInhaler.collider.enabled = false;
        slotMachine.collider.enabled = false;

        trophyShelf.collider.enabled = false;
        firstTrophy.collider.enabled = false;
    }
}
