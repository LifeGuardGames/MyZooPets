using UnityEngine;
using System.Collections;

public class PetSelectionAnimationController : LWFAnimator {
    // // key that tells where the animation is used -- i.e runner, bedroom, trigger ninja
    public string animType;

    protected override void Start(){
        folderPath = "LWF/" + animType + "/" + animName + "/";

        base.Start();
    }
}
