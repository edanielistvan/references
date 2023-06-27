public class SumFromToAsync implements Runnable
{
	public static long sum;
	
	private long from;
	private long to;
	
	public SumFromToAsync(long from, long to)
	{
		this.from = from;
		this.to = to;
	}
	
	public static synchronized void Add(long num)
	{
		sum += num;
	}
	
	@Override
	public void run()
	{	
		long partSum = 0;
		
		for(long i = from; i <= to; i++)
		{
			partSum += i;
		}
		
		SumFromToAsync.Add(partSum);
	}
}