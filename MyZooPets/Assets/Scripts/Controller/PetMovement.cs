using UnityEngine;
using System.Collections;

public class PetMovement : MonoBehaviour {

    public GameObject petSprite;
    private Vector3 destinationPoint;
    private TapItem tapItem;

	public void Init () {
        petSprite = GameObject.Find("SpritePet");
        destinationPoint = petSprite.transform.position;

        tapItem = GetComponent<TapItem>();
        tapItem.OnTap += MovePet;
	}

    void MovePet(){
        if (!ClickManager.CanRespondToTap()) return;

        Ray myRay = Camera.main.ScreenPointToRay(tapItem.lastTapPosition);
        RaycastHit hit;
        if(Physics.Raycast(myRay,out hit)){
            if (hit.collider == collider){
                destinationPoint = hit.point;
            }
        }
    }

	// Update is called once per frame
	void Update () {
        if (petSprite != null){
            petSprite.transform.position = Vector3.MoveTowards(petSprite.transform.position,destinationPoint,5f * Time.deltaTime);
        }
	}
}
