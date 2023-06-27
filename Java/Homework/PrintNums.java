import java.io.FileNotFoundException;
import java.io.PrintWriter;

public class PrintNums extends Thread {
	public void run()
	{
		PrintWriter writer;
		
		for (int i = 0; i <= 10000; i++)
		{
			try
			{
				writer = new PrintWriter(this.getName() + ".txt");
				
				writer.write(String.valueOf(i));
				
				writer.close();
			}
			catch(FileNotFoundException ex)
			{
				System.out.println("Invalid file: " + this.getName());
			}
		}
	}
}
