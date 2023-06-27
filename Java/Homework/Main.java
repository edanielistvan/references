import java.io.File;
import java.io.FileNotFoundException;
import java.util.Scanner;

public class Main 
{
	public static void main(String[] args)
	{
		HF3();
		
		HF5();
	}
	
	private static void HF5()
	{
		Thread[] threads = new Thread[10];
		Thread counter = new ThreadCounter();
		Thread timer = new TimerCounter();
		
		counter.start();
		timer.start();
		
		for (int i = 0; i < 10; i++)
		{
			threads[i] = new PrintNums();
			threads[i].start();
		}
		
		try { Thread.sleep(1000); } catch(InterruptedException ex) { }
		
		for (int i = 0; i < 10; i++) 
		{
			try { threads[i].join(); } catch(InterruptedException ex) { }
		}
		
		Scanner reader;
		
		for (int i = 0; i < 10; i++)
		{
			try
			{
				reader = new Scanner(new File(threads[i].getName() + ".txt"));
				
				System.out.println(threads[i].getName() + ": " + reader.nextLine());
				
				reader.close();
			}
			catch(FileNotFoundException ex)
			{	
			}
		}
	}
	
	private static void HF3()
	{
		long start = 1L;
		long end = 1000000000L;
		
		long sum = 0L;
		
		long startTime = System.nanoTime();
		
		for (long i = start; i <= end; i++)
		{
			sum += i;
		}
		
		long endTime = System.nanoTime();
		
		System.out.println("Sum: " + sum + ", time: " + (endTime - startTime) + " ns");
		
		long step = end / 10;
		
		Thread[] threads = new Thread[10];
		
		long thStartTime = System.nanoTime();
		
		for(int i = 1; i <= 10; i++)
		{
			threads[i-1] = new Thread(new SumFromToAsync((i - 1) * step + 1, i * step));
			threads[i-1].start();
		}
		
		for(int i = 0; i < 10; i++)
		{
			try
			{
				threads[i].join();
			}
			catch(InterruptedException ex)
			{
				System.out.println("Error at: " + i);
			}
		}
		
		long thEndTime = System.nanoTime();
		
		System.out.println("Sum: " + SumFromToAsync.sum + ", time: " + (thEndTime - thStartTime) + " ns");
		
		System.out.println("Saved time: " + ((endTime - startTime) - (thEndTime - thStartTime)));
	}
}
