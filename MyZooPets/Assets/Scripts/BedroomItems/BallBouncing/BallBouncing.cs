using UnityEngine;
using System.Collections;

public class BallBouncing : MonoBehaviour {
	
	public GameObject backButton;
	public GameObject comboLabel;
	public bool isActive = false;
	public Vector2 force;
	public int combo;
	public GameObject floor;
	
	void openBallGame(){
			// Hide other UI objects
			NavigationUIManager.Instance.HidePanel();
			InventoryUIManager.Instance.HidePanel();
			RoomArrowsUIManager.Instance.HidePanel();
			
			//need to disable more things here
			PetAnimationManager.Instance.DisableIdleAnimation();
			backButton.SetActive(true);
			PetMovement.Instance.canMove = false;
	}

	/// <summary>
	/// Move pet into ball view after camera is done zooming in
	/// </summary>
	private void MovePet(){
		GameObject pet = GameObject.Find("Pet");
		//teleport first then walk into view
		if(!pet.renderer.isVisible)
			PetMovement.Instance.petSprite.transform.position = new Vector3(transform.position.x-4f, 0, 26.65529f);
		PetMovement.Instance.MovePet(this.gameObject.transform.position);
	}

	void OnTap (TapGesture e){
		Debug.Log("working");
		combo++;
		Debug.Log(new Vector2 ((transform.position.x - e.Position.x),force.y));
		transform.rigidbody.AddForceAtPosition(new Vector2 (((transform.position.x - e.Position.x)+300),force.y), e.Position);
	}

	void OnCollisionEnter (Collision col){
		if(col.gameObject ==  floor){
			combo = 0;
		}
	}

}
