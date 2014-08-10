package main;

import java.io.IOException;
import java.net.ServerSocket;

public class YAWLListener extends Thread {
	
	protected final int port;
	
	protected ServerSocket serverSocket;
	
	
	public YAWLListener(int port) {
		this.port = port;
		try {
			serverSocket = new ServerSocket(port);
			this.start();
			
		} catch (IOException e) {
			e.printStackTrace();
		}
	}

	public void run() {
		
	}
}
