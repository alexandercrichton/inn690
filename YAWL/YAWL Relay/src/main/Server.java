package main;

import java.io.*;
import java.net.*;
import java.util.concurrent.ExecutorService;
import java.util.concurrent.Executors;

import org.yawlfoundation.yawl.engine.interfce.SpecificationData;

public class Server implements Runnable {
	final static int PORT = 4444;
	public static ExecutorService Executor;

	public static void main(String[] args) {
		Executor = Executors.newFixedThreadPool(5);
		try {
			Executor.execute(new Server());
		} catch (Exception e) {
			System.out.println("I/O failure: " + e.getMessage());
			e.printStackTrace();
		}

	}

	public void run() {
		System.err.println("Server running");
		ServerSocket serverSocket = null;
		boolean listening = true;

		try {
			serverSocket = new ServerSocket(4444);
		} catch (IOException e) {
			System.err.println("Could not listen on port: " + PORT);
			System.exit(-1);
		}

		while (listening) {
			try {
				Executor.execute(new ClientHandler(serverSocket.accept()));
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

	public static class BusinessLogic {
		private static final int LoginUserName = 0;
		private static final int LoginPassword = 1;
		private static final int AuthenticateUser = 2;
		private static final int AuthSuccess = 3;

		private int state = LoginUserName;

		private String userName = null;
		private String userPassword = null;

		public String processInput(String clientRequest) {
			String reply = null;
			try {
				if (clientRequest != null
						&& clientRequest.equalsIgnoreCase("login")) {
					state = LoginPassword;
				}
				if (clientRequest != null
						&& clientRequest.equalsIgnoreCase("exit")) {
					return "exit";
				}

				if (state == LoginUserName) {
					reply = "Please Enter your user name: ";
					state = LoginPassword;
				} else if (state == LoginPassword) {
					userName = clientRequest;
					reply = "Please Enter your password: ";
					state = AuthenticateUser;
				} else if (state == AuthenticateUser) {
					userPassword = clientRequest;
					if (userName.equalsIgnoreCase("John")
							&& userPassword.equals("doe")) {
						reply = "Login Successful...";
						state = AuthSuccess;
					} else {
						reply = "Invalid Credentials!!! Please try again. Enter you user name: ";
						state = LoginPassword;
					}
				} else {
					reply = "Invalid Request!!!";
				}
			} catch (Exception e) {
				System.out.println("input process falied: " + e.getMessage());
				return "exit";
			}

			return reply;
		}
	}
}