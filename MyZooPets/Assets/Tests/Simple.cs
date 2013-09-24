using UnityEngine;
using System;
using LWF;
using System.Collections;

public class Simple : LWFObject {

	void Start()
	{
		setLoader();
		// SetAutoUpdate(false);
		// SetMovieEventHandler (instanceName:"testing", update:updatecallback);
		// #1 Show popup lwf/textures
		string path = "test4.swf/test4";
		string dir = "test4.swf/";
		Load(path:path, texturePrefix:dir, lwfLoadCallback:LoadCallBack);
	}

	private void LoadCallBack(LWFObject lwfObject){
		PlayMovie("sad");
		// print(lwf["sad"]);
		// print(lwf["happy"]);
		print(lwf.data.movies.Length);
		// for(int i =0; i<lwf.data.movies.Length; i++){
		// 	print(lwf.data.movies[i].GetFullName());
		// }

		print(lwf.rootMovie);
		print(lwf.rootMovie.playing);
		print(lwf.rootMovie.visible);
		print(lwf.rootMovie.attachName);

		MovieEventHandlers handlers = new MovieEventHandlers();
		handlers.Add(null, null, null, EnterFrame, UpdateFrame, null);
		lwf.rootMovie.SetHandlers(handlers);
	}

	private void Test(){
		// lwf.rootMovie.Stop();
		Pause();
		print(lwf.rootMovie.playing);
	}

	private void EnterFrame(Movie movie){
		if (movie.currentFrame == movie.totalFrames
			|| !movie.playing) {

			print(movie.GetFullName() + " is done \n");
			lwf.rootMovie.DetachMovie( movie );
			// Pause();
    	}
	}

	private void UpdateFrame(Movie m){
	}

	void setLoader()
	{
		LWFObject.SetLoader(
			lwfDataLoader:(name) => {
				TextAsset asset = Resources.Load(name) as TextAsset;
				if (asset == null) {
					return null;
				}
				return asset.bytes;
			},
			textureLoader:(name) => {
				Texture2D texture = Resources.Load(name) as Texture2D;
				if (texture == null) {
					return null;
				}
				return texture;
			}
		);
	}
}