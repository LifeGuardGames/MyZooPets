using UnityEngine;
using System;
using Parse;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public class ServerEventArgs : EventArgs{
	public bool IsSuccessful {get; set;}
	public ParseException.ErrorCode ErrorCode {get; set;}
	public string ErrorMessage {get; set;}
}

public class ServerConnector<T> : Singleton<T> where T : MonoBehaviour {
	public bool useDummyData = false; //serve dummy data to UI instead of server data

	private bool runTimeOutTimer = false; // to run the time out timer or not
	private float timeOutTimer = 0;
	private float timeOut = 20f; //time out set to 20 seconds
	protected CancellationTokenSource timeOutRequestCancellation; //token used to cancel unfinish task/thread when timeout timer is up

	protected virtual void Awake(){}
	protected virtual void Start(){}
	protected virtual void OnDestroy(){}

	/// <summary>
	/// Keeps tracks of the timeout timer. cancel the server connection of it takes too long
	/// </summary>
	protected virtual void Update(){
		if(runTimeOutTimer){
			timeOutTimer += Time.deltaTime;
			if(timeOutTimer >= timeOut){
				timeOutTimer = 0;
				runTimeOutTimer = false;
				timeOutRequestCancellation.Cancel();
			}
		}
	}

	/// <summary>
	/// Starts the time out timer.
	/// </summary>
	protected void StartTimeOutTimer(){
		runTimeOutTimer = true;
		timeOutRequestCancellation = new CancellationTokenSource();
	}

	/// <summary>
	/// Stops the time out timer.
	/// </summary>
	protected void StopTimeOutTimer(){
		runTimeOutTimer = false;
		timeOutTimer = 0;
	}
}
