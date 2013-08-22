using UnityEngine;
using System.Collections;

/*
    Advair Draggable Body (Advair Step 4).

    This listens for the user's touch on the body, and the inhaler "becomes" a smaller advair inhaler
    when dragged to the pet.

    How this happens is, when the user's touch starts on the inhaler, the original body is hidden.
    At the same time, a sprite is drawn under the user's touch position.

    If the inhaler is dragged to and released over the pet, it will disappear, and the
    step is completed.
    If it is released anywhere else, the original advair inhaler will reappear.
*/
public class AdvairDrag : DragInhaler{
    protected override void Awake(){
        base.Awake();
        spriteName = "advair_small";
        advairStepID = 4;
    }
}