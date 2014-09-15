package org.yawlfoundation.yawl;

import java.io.BufferedReader;
import java.io.IOException;
import java.io.InputStreamReader;
import java.io.PrintWriter;
import java.net.Socket;
import java.util.ArrayList;
import java.util.List;

import org.yawlfoundation.yawl.resourcing.rsInterface.ResourceGatewayException;

public class ClientHandler implements Runnable {
	protected static List<ClientHandler> Clients = new ArrayList<ClientHandler>();
	
	private Socket _clientSocket = null;
	private PrintWriter _out = null;
	private BufferedReader _in = null;
	private Boolean _isListening = true;

	public ClientHandler(Socket socket) {
		_clientSocket = socket;
		addClient(this);
	}
	
	protected static synchronized void addClient(ClientHandler client) {
		Clients.add(client);
	}
	
	protected static synchronized void removeClient(ClientHandler client) {
		Clients.remove(client);
	}

	public void run() {
		Application.Log("Client connected to socket: "
				+ _clientSocket.toString());
		Application.Log(YAWLInterface.Instance.handleB);

		try {
			_out = new PrintWriter(_clientSocket.getOutputStream(), true);
			_in = new BufferedReader(new InputStreamReader(
					_clientSocket.getInputStream()));

			while (_isListening) {
				_isListening = checkConnection();
				String inputLine = _in.readLine();
				if (inputLine != null) {
					Application.Log("RECV: " + inputLine);
					List<String> replyMessages = processInput(inputLine);
					if (replyMessages != null) {
						send(replyMessages);
					}
				}
			}
			_out.close();
			_in.close();
			_clientSocket.close();
			Application.Log("Closing client connection");
			removeClient(this);
		} catch (Exception e) {
			e.printStackTrace();
		}
	}

	protected Boolean checkConnection() {
		return !_clientSocket.isInputShutdown()
				&& !_clientSocket.isOutputShutdown()
				|| !_clientSocket.isClosed() && _clientSocket.isConnected();
	}
	
	protected void closeClientConnection() {
		_isListening = false;
	}

	protected List<String> processInput(String inputLine) throws IOException, ResourceGatewayException {
		YAWLInterface.Instance.ensureConnection();
		
		List<String> replyMessages = new ArrayList<>();
		String command = inputLine.split(" ")[0];

		if (command.equalsIgnoreCase("GetActiveAgents")) {
			replyMessages = YAWLInterface.Instance.GetActiveAgents();
		} else if (command.equalsIgnoreCase("GetAllAgents")) {
			replyMessages = YAWLInterface.Instance.GetAllAgents();
		} else if (command.equalsIgnoreCase("GetTaskQueue")) {
			replyMessages = YAWLInterface.Instance.GetTaskQueue(inputLine);
		} else if (command.equalsIgnoreCase("WorkItemAction")) {
			replyMessages = YAWLInterface.Instance.WorkItemAction(inputLine);
		} else if (command.equalsIgnoreCase("LaunchCase")) {
			replyMessages = YAWLInterface.Instance.LaunchCase(inputLine);
		} else if (command.equalsIgnoreCase("GetCases")) {
			replyMessages = YAWLInterface.Instance.GetCases();
		} else if (command.equalsIgnoreCase("GetAllSpecifications")) {
			replyMessages = YAWLInterface.Instance.GetAllSpecifications();
		} else if (command.equalsIgnoreCase("GetAllWorkItems")) {
			replyMessages = YAWLInterface.Instance.GetAllWorkItems();
		} else if (command.equalsIgnoreCase("CancelCase")) {
			replyMessages = YAWLInterface.Instance.CancelCase(inputLine);
		} else if (command.equalsIgnoreCase("CancelAllCases")) {
			replyMessages = YAWLInterface.Instance.CancelAllCases(inputLine);
		} else if (command.equalsIgnoreCase("EndSession")) {
			closeClientConnection();
		}

		return replyMessages;
	}
	
	protected void send(List<String> replyMessages) {
		for (String message : replyMessages) {
			_out.println(message);
			Application.Log("SEND: " + message);
		}
	}
	
	public static synchronized void SendToAll(List<String> replyMessages) {
		for (ClientHandler client : Clients) {
			client.send(replyMessages);
		}
	}
}
