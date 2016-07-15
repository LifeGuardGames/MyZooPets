using System.Collections;

public class LgTuple<T,U>{
	public T Item1 { get; private set; }

	public U Item2 { get; private set; }
	
	public LgTuple(T item1, U item2){
		Item1 = item1;
		Item2 = item2;
	}
	public void setItem1(T item1){
		Item1 = item1;
	}
}


public static class LgTuple{
	public static LgTuple<T, U> Create<T, U>(T item1, U item2){
		return new LgTuple<T, U>(item1, item2);
	}
}
