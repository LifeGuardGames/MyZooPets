using UnityEngine;
using System.Collections;

public class PopupNotificationNGUI : MonoBehaviour{

	public UILabel textArea;
	public bool HideImmediately = false;
	public delegate void Callback(); // Used for notification entry
	public Callback Button1Callback;
	public Callback Button2Callback;
	public Callback OnHideFinished;
	
	private string soundOpen; // sound this notification should play when it opens

	public string Message{
		get{ return textArea.text;}
		set{ textArea.text = value;}
	}

	public void SetSound(string sound){
		soundOpen = sound;	
	}

	public string GetSound(){
		return soundOpen;
	}

	// These two functions are called when the buttons are clicked.
	protected void Button1Action(){
		Hide();
		if(Button1Callback != null)
			Button1Callback();
	}

	protected void Button2Action(){
		Hide();
		if(Button2Callback != null)
			Button2Callback();
	}

	// Display the popup panel
	public void Display(){
		TryShowDemuxThenToggle(-1);
		
		// play sound if there is one
		string sound = GetSound();
		if(!string.IsNullOrEmpty(sound)) 
			AudioManager.Instance.PlayClip(sound);
	}

	// IMPORTANT: All notifications should call this when finished tween hide callback
	public void BroadcastHideFinished(){
		if(OnHideFinished != null){
			OnHideFinished();
		}
	}

	// Hide the popup panel
	public void Hide(){
		try{
			if(HideImmediately){
				TryHideDemuxThenToggle(0f);
				Destroy(gameObject, 1.0f);
			}
			else{
				TryHideDemuxThenToggle(0.5f);
				Destroy(gameObject, 3.0f);
			}
		} catch(MissingReferenceException e){
			Debug.LogError("Message: " + e.Message + " for this object " + this.gameObject.name);
		}
        
	}

	/// <summary>
	/// Helper class to show this object. Will try to see if there is MoveTweenToggleDemultiplexer, else just MoveTweenToggle
	/// </summary>
	/// <param name='toggleDuration'>
	/// -1 to use default
	/// </param>
	private void TryShowDemuxThenToggle(float toggleDuration){
		if(this.gameObject == null)
			return;

		TweenToggleDemux mtDemux = GetComponent<TweenToggleDemux>();
		TweenToggle mt = GetComponent<TweenToggle>();
		if(mtDemux != null){
			//No support for duration
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
			if(toggleDuration == 0){		//TODO-s not uniform semantic
				mtDemux.hideImmediately = true;
			}
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
	}
}
