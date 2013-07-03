using UnityEngine;
using System.Collections;

public abstract class UserNavigation : MonoBehaviour {

    public abstract void ToTheRight();
    public abstract void ToTheLeft();

    void Start(){
        SwipeDetection.OnSwipeDetected += OnSwipeDetected;
    }

    // function to pass to Swipe Listener
    protected void OnSwipeDetected(Swipe s){
        // disabled swipe

        // switch (s){
        //     // case Swipe.Up:
        //     // print("Swipe.Up");
        //     // break;

        //     // case Swipe.Down:
        //     // print("Swipe.Down");
        //     // break;

        //     case Swipe.Left:
        //     print("Swipe.Left");
        //     ToTheLeft();
        //     break;

        //     case Swipe.Right:
        //     print("Swipe.Right");
        //     ToTheRight();
        //     break;
        // }
    }
}
