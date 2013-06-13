using UnityEngine;
using System.Collections;

public class SpinningWheel : MonoBehaviour {

	private float scrollSpeed = 8.0f;
    private bool spin; //True: spin the wheel, False: don't spin
    private float chosenSlot;

    void Start(){
    }

    //start the slot machine
    public void StartSpin(float chosenSlot){
        this.chosenSlot = chosenSlot;
        spin = true;
        Invoke("StopSpin", 2);
    }

    //stop the slot machine at a random slot
    private void StopSpin(){
        spin = false;
        renderer.material.mainTextureOffset = new Vector2(this.chosenSlot, 0);
    }

    //spinning the slot
    void Update() {
        if(spin){
            float offset = Time.time * scrollSpeed;
            renderer.material.mainTextureOffset = new Vector2(offset, 0);
        }
    }
}
