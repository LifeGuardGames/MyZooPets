using UnityEngine;
using System.Collections;

public abstract class UserNavigation : MonoBehaviour {

    public abstract void ToTheRight();
    public abstract void ToTheLeft();

    public bool CanShowLeftArrow {
        get {return IsLeftArrowEnabled();}
    }

    public bool CanShowRightArrow {
        get {return IsRightArrowEnabled();}
    }

    void Start(){
        SwipeDetection.OnSwipeDetected += OnSwipeDetected;
    }

    protected abstract bool IsLeftArrowEnabled();
    protected abstract bool IsRightArrowEnabled();

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
