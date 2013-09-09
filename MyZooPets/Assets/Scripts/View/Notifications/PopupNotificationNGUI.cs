using UnityEngine;
using System.Collections;

public class PopupNotificationNGUI : MonoBehaviour {

    public UILabel textArea;
    public UILabel button1;
    public UILabel button2;

    // set the following
    public int numOfButtons = 1;
    public bool HideImmediately = false;

	public delegate void HashEntry(); // Used for notification entry
    public HashEntry Button1Callback;
    public HashEntry Button2Callback;
	public HashEntry OnHideFinished;

    public string Message{
        get{return textArea.text;}
        set{textArea.text = value;}
    }
    public string Button1Text{
        get{return button1.text;}
        set{button1.text = value;}
    }
    public string Button2Text{
        get {
            if (numOfButtons < 2){
                Debug.LogError("Number of buttons is set to less than 2.");
                return null;
            }
            return button2.text;
        }
        set{
            if (numOfButtons < 2){
                Debug.LogError("Number of buttons is set to less than 2.");
            }
            button2.text = value;
        }
    }

    // These two functions are called when the buttons are clicked.
    protected void Button1Action(){
		Debug.Log("Button1");
        Hide();
        if (Button1Callback != null) Button1Callback();
    }
    protected void Button2Action(){
		Debug.Log("Button2");
        Hide();
        if (Button2Callback != null) Button2Callback();
    }

    // Display the popup panel
    public void Display(){
        Display(true);
    }

    public void Display(bool pauseGame){
        LoadLevelManager.IsPaused = pauseGame;
        ClickManager.Instance.ClickLock();
		TryShowDemuxThenToggle(-1);
    }

	// IMPORTANT: All notifications should call this when finished tween hide callback
	public void BroadcastHideFinished(){
//		Debug.Log("SENDING UNLOCK");
		if(OnHideFinished != null){
			OnHideFinished();
		}
	}

    // Hide the popup panel
    protected void Hide(){
//		Debug.Log("Moved out! gonna destroy");
        if (HideImmediately){
			TryHideDemuxThenToggle(0f);
            Destroy(gameObject, 1.0f);
        }
        else {
            TryHideDemuxThenToggle(0.5f);
            Destroy(gameObject, 3.0f);
        }
        ClickManager.Instance.ReleaseClickLock();
        LoadLevelManager.IsPaused = false;
    }

	/// <summary>
	/// Helper class to show this object. Will try to see if there is MoveTweenToggleDemultiplexer, else just MoveTweenToggle
	/// </summary>
	/// <param name='toggleDuration'>
	/// -1 to use default
	/// </param>
	private void TryShowDemuxThenToggle(float toggleDuration){
		TweenToggleDemux mtDemux = GetComponent<TweenToggleDemux>();
		TweenToggle mt = GetComponent<TweenToggle>();
		if(mtDemux != null){
			mtDemux.Show();
		}
		else if(mt != null){
			if(toggleDuration >= 0){
				mt.Show(toggleDuration);
			}
			else{
				mt.Show();
			}
		}
		else{
			Debug.LogError("No move tween attached to object");
		}
	}

	/// <summary>
	/// Helper Hide to show this object. Will try to see if there is MoveTweenToggleDemultiplexer, else just MoveTweenToggle
	/// </summary>
	/// <param name='toggleDuration'>
	/// -1 to use default
	/// </param>
	private void TryHideDemuxThenToggle(float toggleDuration){
		TweenToggleDemux mtDemux = GetComponent<TweenToggleDemux>();
		TweenToggle mt = GetComponent<TweenToggle>();
		if(mtDemux != null){
			mtDemux.Hide();
		}
		else if(mt != null){
			if(toggleDuration >= 0){
				mt.Hide(toggleDuration);
			}
			else{
				mt.Hide();
			}
		}
		else{
			Debug.LogError("No move tween attached to object");
		}
	}
	
	void OnDestroy(){
		// Unassign all listeners
		OnHideFinished = null;
		Button1Callback = null;
		Button2Callback = null;
	}
	
	protected virtual void Testing(){

    }
}
