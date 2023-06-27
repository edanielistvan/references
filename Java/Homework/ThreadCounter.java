public class ThreadCounter extends Thread {
	public void run()
	{
		int num = 1;
		
		do {
			num = Thread.currentThread().getThreadGroup().activeCount();
			System.out.println("Active threads: " + num);
			try { Thread.sleep(1000); } catch(InterruptedException ex) { }
		} while(num > 3); //Main, timer counter and this thread
	}
}