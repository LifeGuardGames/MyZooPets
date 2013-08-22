using UnityEngine;
using System.Collections;

/*
    Rescue Draggable Body (Rescue Step 4).

    This listens for the user's touch on the body, and the inhaler "becomes" a smaller rescue inhaler
    when dragged to the pet.

    How this happens is, when the user's touch starts on the inhaler, the original body is hidden.
    At the same time, a sprite is drawn under the user's touch position.

    If the inhaler is dragged to and released over the pet, it will disappear, and a smaller
    rescue inhaler will appear next to the pet's head. Also, the step is completed.
    If it is released anywhere else, the original advair inhaler will reappear.
*/
public class RescueDrag : DragInhaler{
    protected override void Awake(){
        base.Awake();
        spriteName = "rescue_small";
        advairStepID = 4;
    }
}