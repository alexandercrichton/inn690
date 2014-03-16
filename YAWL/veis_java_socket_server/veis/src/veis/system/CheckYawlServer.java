package veis.system;

import java.util.Calendar;
import veis.yawl.YAWLComm;

/**
 * This class will check the connection to Yawl in every 10 seconds.
 * If it is lost, then it will try to create it
 * If the Yawl server doesn't started before hand, it also can connect to
 * @author Fortune
 *
 */
public class CheckYawlServer extends Thread{
	
	private long timestamp;
	private int checkPeriod = 10;
	private boolean isRecentlyHadAnError = false;
	
	public CheckYawlServer(){
		timestamp = Calendar.getInstance().getTimeInMillis();
		this.start();
	}
	
	public void run(){
		//This thread will run with this Application forever
		while(true){
			if((Calendar.getInstance().getTimeInMillis() - checkPeriod * 1000) > timestamp){
				timestamp = Calendar.getInstance().getTimeInMillis();
				doCheck();
			}
		}
	}
	
	private void doCheck(){
		if( ! YAWLComm.GetInstance().isConnectionAlive()){
			isRecentlyHadAnError = true;
			System.err.println("Yawl out, reconnect in the next " + checkPeriod + " seconds. ");
		} else {
			if(isRecentlyHadAnError == true){
				isRecentlyHadAnError = false;
				System.out.println("Connected to Yawl again.");
				YAWLComm.DestroyLastSession();
				try {
					YAWLComm.GetInstance().SyncAll();
				} catch (Exception e) {
					System.err.println(e.getMessage());
				}
			}
		}
	}
}
