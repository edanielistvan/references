import java.util.Timer;
import java.util.TimerTask;

public class TimerCounter extends Thread {
	public void run()
	{
		Timer timer = new Timer();
		
		TimerTask task = new TimerTask() {
			@Override
			public void run() {
				int num = Thread.currentThread().getThreadGroup().activeCount();
				System.out.println("[T] Active threads: " + num);
				
				if (num < 3) this.cancel();
			}
		};
		
		timer.scheduleAtFixedRate(task, 1000L, 1000L);
	}
}