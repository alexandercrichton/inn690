package veis.yawl;
import java.io.BufferedReader;
import java.io.IOException;
import java.io.InputStreamReader;
import java.io.PrintWriter;
import java.net.ServerSocket;
import java.net.Socket;
import java.net.URLDecoder;
import java.util.Calendar;
import java.util.HashMap;

/**
 * This class will create a thread and listen on port 1527 
 *
 */

public class YAWLSocketServerIX extends Thread {
	private ServerSocket serverSocket = null;
	
	protected static final int NOTIFY_CHECK_CASE_CONSTRAINTS = 0;
    protected static final int NOTIFY_CHECK_ITEM_CONSTRAINTS = 1;
    protected static final int NOTIFY_WORKITEM_ABORT = 2;
    protected static final int NOTIFY_TIMEOUT = 3;
    protected static final int NOTIFY_RESOURCE_UNAVAILABLE = 4;
    protected static final int NOTIFY_CONSTRAINT_VIOLATION = 5;
    protected static final int NOTIFY_CANCELLED_CASE = 6;
    
    public static final int port = 1527;
	
	public YAWLSocketServerIX() {
	    try {
	        serverSocket = new ServerSocket(port);
	        System.out.println("Started socket server on port " + port + " to listen to YAWL server.");
	        this.start();
	    } catch (IOException e) {
	        System.err.println("Could not listen on port " + port );
	        System.exit(1);
	    }
	}
    
    public void run() {
    	boolean noErrors = true;
    	
    	try {
			while(noErrors) {
		    	Socket clientSocket = null;
		    	
		        try {
		        	// Listens for a connection to be made to this socket and accepts it.
		            clientSocket = serverSocket.accept();
		            System.err.print("YAWL server just say something to me:");
		        } catch (IOException e) {
		            System.err.println("Accept failed.");
		            noErrors = false;
		            break;
		        }
		        
		        //Get input stream and output stream from the client
		        PrintWriter out = new PrintWriter(clientSocket.getOutputStream(), true);
		        BufferedReader in = new BufferedReader(new InputStreamReader(clientSocket.getInputStream()));
		        
		        //Get the current time in millisecond
		        long time = Calendar.getInstance().getTimeInMillis();
		        
		        //Wait 1 second or until client sent something
		        while(!in.ready() && Calendar.getInstance().getTimeInMillis()-1000 < time) {
		        	//System.out.println("STH"); 
		        	// Do nothing
		        }
		        
		        String inputLine = "EMPTY";
		        
		        while(inputLine.length() > 1 && in.ready()) {
		        	inputLine = in.readLine();
		        	
		        	//System.out.println("Received " + inputLine);
		        }
		        
		        
		        if( ! in.ready()) {
		        	out.print("<response><error /></response>");
		        	
		        } else {
		        	//If there is more
		        	//System.out.print("More");
		        	
		        	inputLine = "";
		        	while(in.ready()) {
		        		char bufferCharacter = (char)in.read();
		        		
		        		//if(Character.compare(bufferCharacter, '\n') != 0 || Character.compare(bufferCharacter, '\r') != 0 ) 
		        			inputLine += bufferCharacter;
		        	}
		        	inputLine = URLDecoder.decode(inputLine, "UTF-8");
		        	
		        	if(inputLine.contains("=")) {
	        			
	        			//Here are some sample string got when create a new case cube clicked on Opensim client:
	        			//This inputLine with action = 0
	        			//[YAWLListenerIX]:RECV: specVersion=0.47&caseID=112&action=0&specID=UID_142f2e5a-7c7c-4d2e-a684-02c230e3689d&specURI=CarAccident&preCheck=true
	        			//This inputLine with action = 1
	        			//[YAWLListenerIX]:RECV: workItem=<workItem><taskid>Receive_Patient</taskid><caseid>112</caseid><uniqueid>0000000000000000000000000</uniqueid><taskname>Receive Patient</taskname><documentation/><specidentifier>UID_142f2e5a-7c7c-4d2e-a684-02c230e3689d</specidentifier><specversion>0.47</specversion><specuri>CarAccident</specuri><status>Enabled</status><allowsdynamiccreation>false</allowsdynamiccreation><requiresmanualresourcing>true</requiresmanualresourcing><codelet/><enablementTime>Nov:10, 2013 11:53:05</enablementTime><enablementTimeMs>1384048385332</enablementTimeMs></workItem>&data=<?xml version="1.0" encoding="UTF-8"?><Trauma_Centre_Patient_Examination />&action=1&preCheck=true
	        			//This inputLine with action = 9
	        			//[YAWLListenerIX]:RECV: caseID=291&action=6
	        			
	        			String[] rawVars = inputLine.split("&");
	        			HashMap<String, String> vars = new HashMap<String, String>();
	        			
	        			for(String var : rawVars) {
	        				vars.put(var.split("=")[0], var.split("=")[1]);
	        			}
	        			
	        			//When something changed on YAWL, doesn't matter what it is, everything needs to be re-cache
	        			
	        			switch(Integer.parseInt(vars.get("action"))) {
	        				case NOTIFY_CHECK_CASE_CONSTRAINTS:
	        					System.out.println("[NOTIFY_CHECK_CASE_CONSTRAINTS]");
	        					YAWLComm.GetInstance().SyncAll();
	        					break;
	        				case NOTIFY_CHECK_ITEM_CONSTRAINTS:
	        					System.out.println("[NOTIFY_CHECK_ITEM_CONSTRAINTS]");
	        					YAWLComm.GetInstance().SyncAll();
	        					break;
	        				case NOTIFY_WORKITEM_ABORT:
	        					System.out.println("[NOTIFY_WORKITEM_ABORT]");
	        					YAWLComm.GetInstance().SyncAll();
	        					break;
	        				case NOTIFY_TIMEOUT:
	        					System.out.println("[NOTIFY_TIMEOUT]");
	        					YAWLComm.GetInstance().SyncAll();
	        					break;
	        				case NOTIFY_RESOURCE_UNAVAILABLE:
	        					System.out.println("[NOTIFY_RESOURCE_UNAVAILABLE]");
	        					YAWLComm.GetInstance().SyncAll();
	        					break;
	        				case NOTIFY_CONSTRAINT_VIOLATION:
	        					System.out.println("[NOTIFY_CONSTRAINT_VIOLATION]");
	        					YAWLComm.GetInstance().SyncAll();
	        					break;
	        				case NOTIFY_CANCELLED_CASE:
	        					System.out.println("[NOTIFY_CANCELLED_CASE]");
	        					YAWLComm.GetInstance().SyncAll();
	        					break;
	        			}
	        			//System.out.println("");
	        			
	        			System.out.println("[=====YAWLListenerIX BEGIN RECEIVING=====]\r\n" + inputLine + "\r\n[=====YAWLListenerIX END RECEIVING=====]");
	        			
	        			//URL Decode?
			        	out.print("<response>" + inputLine + "</response>");
	        		} else {
	        			out.println("Received a string which does not contain =, do nothing. ");
	        		}
		        } 
		        
		        Thread.yield();
	            out.close();
		        in.close();
		        clientSocket.close();
			}

	        System.err.println("[YAWLListenerIX]: Server shutting down...");
			serverSocket.close();
    	
    	} catch (Exception e) {
    		System.err.println("[YAWLListenerIX]:" + e.getMessage());
    		try { serverSocket.close(); } catch (IOException e1) { }
    	}
    	
    	System.err.println("Server closed.");
	}
}
