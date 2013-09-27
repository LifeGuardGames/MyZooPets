using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//---------------------------------------------------
// LWFAnimator
// We found this script online.  It's the only thing
// that seems to play LWF files correctly and give
// the appropriate callbacks.
//---------------------------------------------------

[System.Serializable]
public class FlashMovieClip {
	public string clipName; 	// key we create and will access clip by
	public string ASLinkage;	// the name of the linkage in the source SWF file
	public WrapMode wrapMode;	// loop, play once, etc
	public bool bCanInterrupt = true;	// can this animation be interrupted?
}

public class LWFAnimator : LWFObject {
	// the name of the instance of the movieclip in the scene by default.  We should always just leaves this as "animationg"
	public string instanceName = "animation";
	
	// the name of the LWF byte file
	public string animName;
	
	// folder path that the byte file is in.  Should end with a /
	public string folderPath;
	
	// list of movie clips that the LWF plays.  The names here are keys we create independent of the original SWF
	public List<FlashMovieClip> clips;
	
	// the clip that was last played
	protected FlashMovieClip clipCurrent;
	protected FlashMovieClip GetCurrentClip() {
		return clipCurrent;	
	}
	
	// list of playing movies
	private List<LWF.Movie> attachedMovies = new List<LWF.Movie>();
	
	//---------------------------------------------------
	// PlayClip()
	// Plays a clip with the incoming clip name key.
	//---------------------------------------------------	
	public virtual void PlayClip(string _clipName) {		
		LWF.Movie _movie = lwf.rootMovie.SearchAttachedMovie(_clipName);
		
		// if this clip is already playing, remove it
		if (_movie != null) {
			attachedMovies.Remove(_movie);
			lwf.rootMovie.DetachMovie(_movie);
			_movie = null;
		}
		
		if (lwf !=null) {
			FlashMovieClip _clip = GetClip(_clipName);
			if (_clip != null)
				_movie = lwf.rootMovie.AttachMovie(_clip.ASLinkage, _clip.clipName, enterFrame: EnterFrameCallback);
			else {
				// original code; looks like it's trying to play a movie simple based on the clip...
				//_movie = lwf.rootMovie.AttachMovie(_clipName, _clipName, enterFrame: EnterFrameCallback);
				
				// but I'd rather just show some debug message and not do anything
				Debug.Log("No such clip as " + _clipName + " on " + gameObject.name);
			}
			
			if (_movie != null && _clip != null) {
				attachedMovies.Add(_movie);		// attach the move
				clipCurrent = _clip;			// cache the clip that is currently playing
			}
		}
	}
	
	public virtual void PlayClip(string _clipName, bool _stopAllPrevMovies)
	{
		if (_stopAllPrevMovies)
			DetachAllMovies();
		
		PlayClip(_clipName);
	}
	
	public void DetachAllMovies() {
		if (attachedMovies != null && attachedMovies.Count > 0) {
			for (int i=0; i<attachedMovies.Count; i++) {
				LWF.Movie _movie = attachedMovies[i];
				lwf.rootMovie.DetachMovie(_movie);
			}
			
			attachedMovies.Clear();
		}
	}
	
	//-----------------------------------------------------------------
	// private methods
	//-----------------------------------------------------------------
	
	// Callback that called every frame.
	private void EnterFrameCallback(LWF.Movie _movie)
	{
		//Debug.Log(clipCurrent.clipName + ": " + _movie.currentFrame + " of " + _movie.totalFrames);
		
		if (_movie.currentFrame == _movie.totalFrames || !_movie.playing)
			ClipFinished();
		
		if (_movie.currentFrame == _movie.totalFrames || !_movie.playing) {			
			WrapMode _clipWrapMode = GetWrapMode(_movie.name);
			switch (_clipWrapMode) {
			case WrapMode.Default:
			case WrapMode.Clamp:
				attachedMovies.Remove(_movie);
				lwf.rootMovie.DetachMovie(_movie);
				break;
				
			case WrapMode.ClampForever:
				_movie.Stop();
				break;
				
				// for loop, no need detach not stop, as it'll loop itself anyway
			}
		}
	}
	
	protected virtual void ClipFinished() {
	}
	
	// get a clip from the clips array based on its name
	private FlashMovieClip GetClip(string _clipName)  {
		if (clips != null && clips.Count > 0) {
			for (int i=0; i<clips.Count; i++) {
				FlashMovieClip _clip = clips[i];
				if (_clip.clipName == _clipName)
					return _clip;
			}
		}
		
		return null;
	}
	
	// get a clip from the clips array based on its name
	private WrapMode GetWrapMode(string _clipName) {
		if (clips != null && clips.Count > 0) {
			for (int i=0; i<clips.Count; i++) {
				FlashMovieClip _clip = clips[i];
				if (_clip.clipName == _clipName)
					return _clip.wrapMode;
			}
		}
		
		return WrapMode.Once;
	}
	
	private void SetLoader() {
		LWFObject.SetLoader(
			lwfDataLoader:(name) => 
			{
				TextAsset asset = Resources.Load(name) as TextAsset;
				if (asset == null) 
				{
					return null;
				}
				return asset.bytes;
			},
			textureLoader:(name) => 
			{
				Texture2D texture = Resources.Load(name) as Texture2D;
				if (texture == null) 
				{
					return null;
				}
				return texture;
			}
		);
	}
	
	//-----------------------------------------------------------------
	// protected methods
	//-----------------------------------------------------------------
	
	// Use this for initialization
	protected void Start () {
		SetLoader();
		
		Load(folderPath + animName, folderPath);
		
		// the original author insists that these are necessary, but we found it messes up the z-value of the individual bitmap
		// pieces too much.  We scale separately on the game object that this script is attached to.
		//ScaleForWidth(Screen.width / 2);
		//ScaleForHeight(Screen.height / 2);
		
		StopMovie(instanceName);
		SetVisibleMovie(instanceName, false);
	}
	
}
