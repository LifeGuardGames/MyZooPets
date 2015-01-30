using System.Collections;
using UnityEngine;

/// <summary>
/// Reward queue data.
/// The queue stores function pointers to be called
/// </summary>
public static class RewardQueueData{
	private static Queue q = new Queue();

	public delegate void GenericDelegate();

	public static void AddReward(GenericDelegate d){
		q.Enqueue(d);
	}
	
	public static GenericDelegate PopReward(){
		return q.Dequeue() as GenericDelegate;
	}
	
	public static bool IsEmpty(){
		return (QueueCount() > 0) ? false : true;
	}
	
	public static int QueueCount(){
		return q.Count;	
	}
}
