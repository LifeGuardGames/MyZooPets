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
        notebook.GetComponent<Collider>().enabled = true;
        calendar.GetComponent<Collider>().enabled = true;
        laptop.GetComponent<Collider>().enabled = true;
        realInhaler.GetComponent<Collider>().enabled = true;
        practiceInhaler.GetComponent<Collider>().enabled = true;
        slotMachine.GetComponent<Collider>().enabled = true;

        trophyShelf.GetComponent<Collider>().enabled = false;
        firstTrophy.GetComponent<Collider>().enabled = false;
    }

    public void ActivatePartition2(){
        notebook.GetComponent<Collider>().enabled = false;
        calendar.GetComponent<Collider>().enabled = false;
        laptop.GetComponent<Collider>().enabled = false;
        realInhaler.GetComponent<Collider>().enabled = false;
        practiceInhaler.GetComponent<Collider>().enabled = false;
        slotMachine.GetComponent<Collider>().enabled = false;

        trophyShelf.GetComponent<Collider>().enabled = true;
        firstTrophy.GetComponent<Collider>().enabled = true;
    }

    public void ActivatePartition3(){
        notebook.GetComponent<Collider>().enabled = false;
        calendar.GetComponent<Collider>().enabled = false;
        laptop.GetComponent<Collider>().enabled = false;
        realInhaler.GetComponent<Collider>().enabled = false;
        practiceInhaler.GetComponent<Collider>().enabled = false;
        slotMachine.GetComponent<Collider>().enabled = false;

        trophyShelf.GetComponent<Collider>().enabled = false;
        firstTrophy.GetComponent<Collider>().enabled = false;
    }
}
