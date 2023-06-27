import java.util.*;

public class HaziFeladat {
	public static List<Integer> lista = new ArrayList<Integer>();
	
	public static void main(String[] args)
	{
		var start = System.nanoTime();
		UseThread(100000, false, false);
		var end = System.nanoTime();
		
		System.out.println("Time: " + (end - start) + " ns.");
		
		start = System.nanoTime();
		UseThread(100000, false, true);
		end = System.nanoTime();
		
		System.out.println("Time: " + (end - start) + " ns.");
		
		start = System.nanoTime();
		UseThread(100000, true, false);
		end = System.nanoTime();
		
		System.out.println("Time: " + (end - start) + " ns.");
		
		start = System.nanoTime();
		UseThread(100000, true, true);
		end = System.nanoTime();
		
		System.out.println("Time: " + (end - start) + " ns.");
		
		//Testing array
		ThreadSafeMutableIntArray array = new ThreadSafeMutableIntArray(2);
		
		Thread[] threads = new Thread[10];
		
		for (int i = 0; i < 10; i++)
		{
			if (i < 5) threads[i] = new Thread(() -> { while(array.get(0) < 10_000_000) { array.set(0, array.get(0) + 1); }});
			else threads[i] = new Thread(() -> { while(array.get(1) < 10_000_000) { array.set(1, array.get(1) + 1); }});
			
			threads[i].start();
		}
		
		for (int i = 0; i < 10; i++)
		{
			try { threads[i].join(); } catch(InterruptedException ex) { }
		}
		
		System.out.println("First element: " + array.get(0) + "\nSecond element: " + array.get(0));
	}
	
	public static void UseThread(int n, boolean sync, boolean order)
	{
		lista.clear();
		
		Thread[] threads = new Thread[n];
		
		for (int i = 0; i < n; i++)
		{
			threads[i] = new Thread(SegedMetodus(i, sync, order));
			
			threads[i].start();
		}
		
		for (int i = 0; i < n; i++)
		{
			try { threads[i].join(); } catch(InterruptedException ex) { }
		}
		
		System.out.println("Parameters: " + n + " | " + sync + " | " + order);
		
		System.out.println("Array length -> Expected: " + n + " | Actual: " + lista.size());
	}
	
	public static Runnable SegedMetodus(int i, boolean sync, boolean order)
	{
		return () -> { 
			if (sync)
			{
				synchronized(lista) 
				{
					if (order)
					{
						if (lista.size() == 0) lista.add(0);
						else lista.add(lista.get(lista.size() - 1) + 1);
					}
					else
					{
						lista.add(i); 
					}
				} 
			}
			else
			{
				if (order)
				{
					if (lista.size() == 0) lista.add(0);
					else lista.add(lista.get(lista.size() - 1) + 1);
				}
				else
				{
					lista.add(i); 
				}
			}	 
		};
	}
}
