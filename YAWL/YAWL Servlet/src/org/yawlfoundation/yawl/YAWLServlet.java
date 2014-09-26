package org.yawlfoundation.yawl;

import java.io.IOException;

import javax.servlet.ServletContextEvent;
import javax.servlet.ServletContextListener;
import javax.servlet.ServletException;
import javax.servlet.http.HttpServlet;
import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;

public class YAWLServlet extends HttpServlet implements ServletContextListener {

	private static final long serialVersionUID = -4736351892429673657L;

	public YAWLServlet() {
		super();
	}

	@Override
	public void doGet(HttpServletRequest request,
			HttpServletResponse response) throws ServletException, IOException {
//		YAWLInterface.Instance.PrintAllTaskQueues();
		response.getWriter().write(Application.GetLog());
//		YAWLInterface.Instance.ensureConnection();
		Application.Log(YAWLInterface.Instance.handleB);
	}

	@Override
	public void contextDestroyed(ServletContextEvent arg0) {
		// TODO Auto-generated method stub
//		MAKE SURE ALL THREADS DIE PROPERLY
//		Application.Executor.shutdownNow();
	}

	@Override
	public void contextInitialized(ServletContextEvent arg0) {
		Application.Log("Context initialized");
		Application.Initialize();
//		YAWLInterface.Initialise();
	}
}
