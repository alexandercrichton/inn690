package main;

import java.io.FileWriter;
import java.io.IOException;
import java.net.ConnectException;

import veis.system.AgentReceiverSocketServer;
import veis.system.CheckYawlServer;
import veis.yawl.YAWLComm;
import veis.yawl.YAWLSocketServerIX;

/**
 * This is the class contains the main execution point
 */
public class Application {

	public static String YAWLAdmin = "admin";
	public static String YAWLPasswd = "YAWL";
	
	public static void main(String [] args)  {
		
		String path = Application.class.getResource("").getPath();
		if( ! path.equals("main/")){
			System.out.println("Working directory: file://" + path );	
		}
		
		System.out.println("Log file: " + logFileName);
		
		new AgentReceiverSocketServer();
		new YAWLSocketServerIX();
		new CheckYawlServer();
		
		try
		{
			YAWLComm.GetInstance().SyncAll();
			//System.out.println(SystemStorage.GetInstance());
		}
		catch(ConnectException ex){
			System.err.println(" - It seems like YAWL Engine has not started yet.");
			System.err.println(" - Get it from http://www.yawlfoundation.org, and start by \"Start Engine\" shortcut.");
			//System.exit(1);
		}
		catch (Exception ex)
		{
			ex.printStackTrace();
		}
	}
	
	private static String logFileName = "ListennedLog.txt";
	public static void Log(String s){
		try
    	{
    	    //the true will append the new data
    	    FileWriter fw = new FileWriter(logFileName,true); 
    	    //appends the string to the file
    	    fw.write(s + "\r\n");
    	    fw.close();
    	    System.out.println(s);
    	}
    	catch(IOException ioe)
    	{
    	    System.err.println("IOException: " + ioe.getMessage());
    	}
	}
}
