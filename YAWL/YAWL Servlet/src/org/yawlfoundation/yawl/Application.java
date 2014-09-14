package org.yawlfoundation.yawl;

import java.util.concurrent.ExecutorService;
import java.util.concurrent.Executors;

public class Application {
	protected static final int CLIENT_PORT = 4444;
	protected static final int YAWL_PORT = 1527;
	protected static String Log = "";
	
	public static final ExecutorService Executor = Executors.newFixedThreadPool(5);

	public static void main(String[] args) {
		try {
			Log("Application main");
//			Executor.execute(new ClientListenServer(CLIENT_PORT));
//			Executor.execute(new YAWLListenServer(YAWL_PORT));
		} catch (Exception e) {
			e.printStackTrace();
		}
	}
	
	public static void Initialize() {
		try {
			Log("Application initialized");
			Executor.execute(new ClientListenServer(CLIENT_PORT));
//			Executor.execute(new YAWLListenServer(YAWL_PORT));
		} catch (Exception e) {
			e.printStackTrace();
		}
	}
	
	public static synchronized void Log(String s) {
		Log += s + "\n";
	}

	public static synchronized String GetLog() {
		return Log;
	}
	
	public static synchronized void ResetLog() {
		Log = "";
	}
}