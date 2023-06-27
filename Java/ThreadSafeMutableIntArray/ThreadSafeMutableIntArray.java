import java.util.function.IntUnaryOperator;

public class ThreadSafeMutableIntArray {
	private int[] array;
	private Object[] keys;
	
	public ThreadSafeMutableIntArray(int capacity)
	{
		array = new int[capacity];
		keys = new Object[capacity];
		
		for(int i = 0; i < capacity; i++)
		{
			keys[i] = new Object();
		}
	}
	
	public int get(int n)
	{
		synchronized(keys[n]) { return array[n]; }
	}
	
	public void set(int n, int newValue)
	{
		synchronized(keys[n]) { array[n] = newValue; }
	}
	
	public int updateAndGet(int n, IntUnaryOperator op)
	{
		synchronized(keys[n]) {
			array[n] = op.applyAsInt(array[n]);
			return array[n];
		}
	}
	
	public int getAndUpdate(int n, IntUnaryOperator op)
	{
		synchronized(keys[n]) {
			int temp = array[n];
			array[n] = op.applyAsInt(array[n]);
			return temp;
		}
	}
}
