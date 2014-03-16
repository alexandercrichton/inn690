package main;

import java.util.Map;

import org.yawlfoundation.yawl.util.XNode;

public class Utilities {
	private static String In(int indent) {
		String retval = "";
		while(indent > 0) {
			retval = retval + "  ";
			indent--;
		}
		return retval;
	}
	
	/**
	 * Print XML by a nice format
	 * @param node
	 * @return
	 */
	public static int OutputNodeXML(XNode node) {
		return OutputNodeXML(node, 0);
	}
	
	private static int OutputNodeXML(XNode node, int indent) {
		System.out.print(In(indent) + "<" + node.getName());
		
		if(node.getAttributeCount() > 0) {
			for(Map.Entry<String, String> e : node.getAttributes().entrySet()) {
				System.out.print(" " + e.getKey() + "=\"" + e.getValue() + "\"");
			}
		}
		
		if(node.getTextLength() == 0 && node.getChildCount() == 0) System.out.print(" /");
		
		if(node.getTextLength() > 0) {
			System.out.print(">" + node.getText() + "</" + node.getName());
		}  
		
		System.out.println(">");
		

		
		if(node.getChildCount() > 0) {
			for(XNode child : node.getChildren()){
				OutputNodeXML(child, indent+1);
			}
			System.out.println(In(indent) + "</" + node.getName() + ">");
			return 1;
		}
		
				
		
		return 0;
	}
	/*
	private static int OutputNode(XNode node, int indent) {
		System.out.println(In(indent) + "NODE: " + node.getName());
		
		if(node.getAttributeCount() > 0) {
			System.out.println(In(indent+1) + "Attributes: ");
			for(Map.Entry<String, String> e : node.getAttributes().entrySet()) {
				System.out.println(In(indent+2) + e.getKey() + ": " + e.getValue());
			}
		}
		
		if(node.getTextLength() > 0) {
			System.out.println(In(indent+1)+"Text: " + node.getText());
		}
		
		if(node.getChildCount() > 0) {
			for(XNode child : node.getChildren()){
				OutputNode(child, indent+1);
			}
			return 1;
		}
		
		return 0;
	}*/
	
}
