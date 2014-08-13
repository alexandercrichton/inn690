package main;

import java.io.BufferedReader;
import java.io.DataInputStream;
import java.io.InputStreamReader;
import java.io.PrintStream;
import java.io.IOException;
import java.io.PrintWriter;
import java.net.Socket;
import java.net.ServerSocket;
import java.util.ArrayList;
import java.util.List;

import org.yawlfoundation.yawl.resourcing.rsInterface.ResourceGatewayException;

import main.Server.BusinessLogic;

public class ClientHandler implements Runnable {
	private Socket _clientSocket = null;
	private PrintWriter _out = null;
	private BufferedReader _in = null;
	private Boolean _isListening = true;

	public ClientHandler(Socket socket) {
		_clientSocket = socket;
	}

	public void run() {
		System.out.println("Client connected to socket: "
				+ _clientSocket.toString());

		try {
			_out = new PrintWriter(_clientSocket.getOutputStream(), true);
			_in = new BufferedReader(new InputStreamReader(
					_clientSocket.getInputStream()));

			while (_isListening) {
				_isListening = checkConnection();
				String inputLine = _in.readLine();
				if (inputLine != null) {
					System.out.println("RECV: " + inputLine);
					List<String> replyMessages = processInput(inputLine);
					if (replyMessages != null) {
						send(replyMessages);
					}
				}
			}
		} catch (IOException e) {
			e.printStackTrace();
		} catch (ResourceGatewayException e) {
			e.printStackTrace();
		} finally {
			try {
				_out.close();
				_in.close();
				_clientSocket.close();
				System.out.println("Closing client connection");
			} catch (Exception e) {
				System.out.println("Couldn't close I/O streams");
			}
		}
	}

	protected Boolean checkConnection() {
		return !_clientSocket.isInputShutdown()
				&& !_clientSocket.isOutputShutdown()
				|| !_clientSocket.isClosed() && _clientSocket.isConnected();
	}

	protected List<String> processInput(String inputLine) throws IOException, ResourceGatewayException {
		List<String> replyMessages = new ArrayList<>();
		String command = inputLine.split(" ")[0];

		if (command.equalsIgnoreCase("GetActiveAgents")) {
			replyMessages = YAWLInterface.GetActiveAgents();
		} else if (command.equalsIgnoreCase("GetAllAgents")) {
			replyMessages = YAWLInterface.GetAllAgents();
		} else if (command.equalsIgnoreCase("GetTaskQueue")) {
			replyMessages = YAWLInterface.GetTaskQueue(inputLine);
		} else if (command.equalsIgnoreCase("WorkItemAction")) {
			replyMessages = YAWLInterface.WorkItemAction(inputLine);
		} else if (command.equalsIgnoreCase("LaunchCase")) {
			replyMessages = YAWLInterface.LaunchCase(inputLine);
		} else if (command.equalsIgnoreCase("SyncAll")) {
			replyMessages = YAWLInterface.SyncAll();
		} else if (command.equalsIgnoreCase("GetCases")) {
			replyMessages = YAWLInterface.GetCases();
		} else if (command.equalsIgnoreCase("GetAllSpecifications")) {
			replyMessages = YAWLInterface.GetAllSpecifications();
		} else if (command.equalsIgnoreCase("GetAllWorkItems")) {
			replyMessages = YAWLInterface.GetAllWorkItems();
		} else if (command.equalsIgnoreCase("CancelCase")) {
			replyMessages = YAWLInterface.CancelCase(inputLine);
		} else if (command.equalsIgnoreCase("CancelAllCases")) {
			replyMessages = YAWLInterface.CancelAllCases(inputLine);
		} else if (command.equalsIgnoreCase("EndSession")) {
			closeClientConnection();
		}

		return replyMessages;
	}
	
	protected void send(List<String> replyMessages) {
		for (String message : replyMessages) {
			_out.println(message);
			System.out.println("SEND: " + message);
		}
	}
	
	protected void closeClientConnection() {
		_isListening = false;
	}
}
