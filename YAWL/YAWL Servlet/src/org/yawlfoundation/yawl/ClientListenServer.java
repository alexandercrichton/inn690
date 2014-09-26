package org.yawlfoundation.yawl;

import java.io.IOException;
import java.net.ServerSocket;

public class ClientListenServer implements Runnable {
	
	protected int _port;
	
	public ClientListenServer(int port) {
		this._port = port;
	}

	public void run() {
		try {
			Application.Log("ClientListenServer listening on port " + _port);
			ServerSocket serverSocket = null;
			boolean listening = false;

			try {
				serverSocket = new ServerSocket(_port);
				listening = true;
			} catch (IOException e) {
				System.err.println("ClientListenServer: Could not listen on port: " + _port);
				System.err.println(e.getMessage());
				return;
			}

			while (listening) {
				Application.Executor.execute(new ClientHandler(serverSocket.accept()));
			}
			
			serverSocket.close();
		} catch (Exception e) {
			e.printStackTrace();
		}
		
	}
}