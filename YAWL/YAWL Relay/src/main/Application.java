package main;

import java.util.concurrent.ExecutorService;
import java.util.concurrent.Executors;

public class Application {
	protected static final int CLIENT_PORT = 4444;
	protected static final int YAWL_PORT = 1527;
	public static final ExecutorService Executor = Executors.newFixedThreadPool(20);

	public static void main(String[] args) {
		try {
			Executor.execute(new ClientListenServer(CLIENT_PORT));
		} catch (Exception e) {
			e.printStackTrace();
		}

		Runtime.getRuntime().addShutdownHook(new Thread() {
			public void run() {
				System.out.println("shutdownNow()");
				Executor.shutdownNow();
			}
		});
	}
}