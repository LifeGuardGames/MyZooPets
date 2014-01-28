using UnityEngine;
using System.Collections;

public class Parallax : MonoBehaviour {
  public bool horizontalMovement;           //TODO-s vertical movement untested
    public float moveTime;
    
    public Vector3 startPosition;
    public Vector3 spawnNewObjectPosition;  // Also the initial case startPosition
    public Vector3 endPosition;

    private bool canSpawnNew;
    
    void Start(){
        canSpawnNew = true;
        if(horizontalMovement){
            LeanTween.moveLocal(gameObject, spawnNewObjectPosition, moveTime, new Hashtable());  // Move to spawn Position
        }
        else{   // Vertical movement
            // LeanTween.moveY(gameObject, spawnNewObjectPosition.y, moveTime, null);  // Move to spawn Position
        }
    }
    
    void Update(){
        if(canSpawnNew && gameObject.transform.position == spawnNewObjectPosition){
            //Debug.Log("spawning " + gameObject + " - ");
            GameObject clone = Instantiate(gameObject, startPosition, transform.rotation) as GameObject;
            clone.name = gameObject.name;
            if(horizontalMovement){
                

                
            }
            else{   // Vertical movement
                // LeanTween.moveY(gameObject, endPosition.y, moveTime, null);
            }
            LeanTween.moveLocal(this.gameObject, endPosition, moveTime, new Hashtable());
            Debug.Log("tweening " + endPosition.x + " " + moveTime);
            canSpawnNew = false;
        }
        
        if(gameObject.transform.position == endPosition){
            Debug.Log("destroying");
            Destroy(gameObject);
        }
    } 
}
