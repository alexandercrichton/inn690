package veis.yawl;

import java.io.BufferedInputStream;
import java.io.ByteArrayOutputStream;
import java.io.OutputStreamWriter;
import java.net.HttpURLConnection;
import java.net.URL;

/**
 * This class just for sending a query to IB interface
 * @author Fortune
 *
 */
public class YAWL_IB_Action {
	
	public static String execute(String IBURI, String query, String sessionHandle) throws Exception {
		
		URL url = new URL(IBURI + "?action="+query+"&sessionHandle="+sessionHandle);
		
		HttpURLConnection connection = (HttpURLConnection) url.openConnection();
        connection.setDoOutput(true);
        connection.setRequestProperty("Accept-Charset", "UTF-8");

        // required to ensure the connection is not reused. When not set, spurious
        // intermittent problems (double posts, missing posts) occur under heavy load.
        connection.setRequestProperty("Connection", "close");

        // encode data and send query
        OutputStreamWriter out = new OutputStreamWriter(connection.getOutputStream(), "UTF-8");
        out.write("");
        out.close();

        //retrieve reply

        final int BUF_SIZE = 16384;
        
        // read reply into a buffered byte stream - to preserve UTF-8
        BufferedInputStream inStream = new BufferedInputStream(connection.getInputStream());
        ByteArrayOutputStream outStream = new ByteArrayOutputStream(BUF_SIZE);
        byte[] buffer = new byte[BUF_SIZE];

        // read chunks from the input stream and write them out
        int bytesRead;
        while ((bytesRead = inStream.read(buffer, 0, BUF_SIZE)) > 0) {
            outStream.write(buffer, 0, bytesRead);
        }

        outStream.close();
        inStream.close();

        // convert the bytes to a UTF-8 string
        String result = outStream.toString("UTF-8");
        connection.disconnect();
        
		return result;
	}
	
}
