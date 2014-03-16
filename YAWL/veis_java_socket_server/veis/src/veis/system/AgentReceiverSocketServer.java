package veis.system;

import java.io.IOException;
import java.net.ServerSocket;
import java.util.ArrayList;

/**
 * This class instantiate a thread with accept multiple client connections
 */
public class AgentReceiverSocketServer extends Thread {
	
	public static final int SERVER_SOCKET = 4444;
	public static ArrayList<MessageHandlerOn4444> allClients;
	
	private ServerSocket serverSocket = null;
	
	public AgentReceiverSocketServer() {
		try {
	        serverSocket = new ServerSocket(SERVER_SOCKET);
	        System.out.println("Started socket server on port " + SERVER_SOCKET + " to listen to client.");
	        allClients = new ArrayList<>();
	        this.start();
	    } catch (IOException e) {
	        System.err.println("Could not listen on port " + SERVER_SOCKET);
	    }
	}
	
	public static String getCurrentConnections(){
		return " Current connections: " + allClients.size();
	}
	
	public static void SendToAllClients(String message){
		for(int i = 0; i < allClients.size(); i++){
			allClients.get(i).Send(message);
		}
	}
	
	@Override
	public void run() {
    	boolean noErrors = true;
    	
    	try {
    		while(noErrors) {
    			try {
    				MessageHandlerOn4444 a = new MessageHandlerOn4444(serverSocket.accept());
    				allClients.add(a);
    				new Thread(a);
		        } catch (IOException e) {
		            System.err.println("Accept failed.");
		            noErrors = false;
		        }
    		}
			
	        System.err.println("[AgentReceiver]: Server shutting down...");
			serverSocket.close();
    	} catch (Exception e) {
    		System.err.println("[AgentReceiver]");
    		e.printStackTrace();
    		try { serverSocket.close(); } catch (IOException e1) { }
    	}
	}
}
