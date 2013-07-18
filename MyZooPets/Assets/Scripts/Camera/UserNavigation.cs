using UnityEngine;
using System.Collections;

public abstract class UserNavigation : MonoBehaviour {

    public abstract void ToTheRight();
    public abstract void ToTheLeft();
    protected bool inverted = true;

    public bool CanShowLeftArrow {
        get {return IsLeftArrowEnabled();}
    }

    public bool CanShowRightArrow {
        get {return IsRightArrowEnabled();}
    }

    protected virtual void Start(){
        SwipeDetection.OnSwipeDetected += OnSwipeDetected;
    }

    protected virtual void OnDestroy(){
        SwipeDetection.OnSwipeDetected -= OnSwipeDetected;
    }

    protected abstract bool IsLeftArrowEnabled();
    protected abstract bool IsRightArrowEnabled();

    // function to pass to Swipe Listener
    protected void OnSwipeDetected(Swipe s){
        if (inverted){
            switch (s){
                // case Swipe.Up:
                // print("Swipe.Up");
                // break;

                // case Swipe.Down:
                // print("Swipe.Down");
                // break;

                case Swipe.Left:
                print("ToTheRight");
                ToTheRight();
                break;

                case Swipe.Right:
                print("ToTheLeft");
                ToTheLeft();
                break;
            }
        }
        else{
            switch (s){
                // case Swipe.Up:
                // print("Swipe.Up");
                // break;

                // case Swipe.Down:
                // print("Swipe.Down");
                // break;

                case Swipe.Left:
                print("ToTheLeft");
                ToTheLeft();
                break;

                case Swipe.Right:
                print("ToTheRight");
                ToTheRight();
                break;
            }
        }
    }
}
