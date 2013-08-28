using System.Collections;

public static class NotificationQueueData {

	private static Queue q = new Queue();
	
	public static void AddNotification(Hashtable paramTable){
		q.Enqueue(paramTable);
	}
	
	public static Hashtable PopNotification(){
		return q.Dequeue() as Hashtable;
	}
	
	public static bool IsEmpty(){
		return (QueueCount() > 0) ? false : true;
	}
	
	public static int QueueCount(){
		return q.Count;	
	}
}
