package org.yawlfoundation.yawl;

import java.io.BufferedReader;
import java.io.IOException;
import java.io.InputStreamReader;
import java.net.ServerSocket;
import java.net.Socket;
import java.net.URLDecoder;
import java.util.List;

import org.yawlfoundation.yawl.engine.interfce.interfaceX.InterfaceX_ServiceSideClient;

public class YAWLListenServer implements Runnable {
	
	protected int _port;	
	protected InterfaceX_ServiceSideClient serviceSideClient;
	
	public YAWLListenServer(int port) {
		this._port = port;
	}

	public void run() {
		System.out.println("YAWLListenServer listening on port " + _port);
		ServerSocket serverSocket = null;
		boolean listening = true;

		try {
			serverSocket = new ServerSocket(_port);
			serviceSideClient = new InterfaceX_ServiceSideClient("http://localhost:8080/yawl/ix");
			serviceSideClient.addInterfaceXListener("http://localhost:" + _port);
		} catch (IOException e) {
			System.err.println("YAWLListenServer: Could not listen on port: " + _port);
			System.exit(-1);
		}

		while (listening) {
			try {
				HandleConnection(serverSocket.accept());
			} catch (IOException e) {
				e.printStackTrace();
			}
		}

		try {
			serverSocket.close();
			
		} catch (IOException e) {
			e.printStackTrace();
		}
	}
	
	protected void HandleConnection(Socket socket) {
		BufferedReader in = null;

		try {
			in = new BufferedReader(new InputStreamReader(socket.getInputStream()));
			// YAWL sends a bunch of unnecessary lines.
			// This is a simple way to ignore them and
			// only leave the wanted chars
			while (in.ready() && in.readLine().length() > 1) { }				

			String inputLine = "";
			while (in.ready()) {
				inputLine += (char)in.read();
			}			
			inputLine = URLDecoder.decode(inputLine, "UTF-8");
			System.out.println("YAWL RECV: " + inputLine);
			
			if (inputLine != null && inputLine.length() > 0) {
				List<String> replyMessages = null;
				if (replyMessages != null) {
					ClientHandler.SendToAll(replyMessages);
				}
			}
			in.close();
			socket.close();
		} catch (IOException e) {
			e.printStackTrace();
		}
	}
}
