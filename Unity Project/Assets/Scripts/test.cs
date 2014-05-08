using UnityEngine;
using System.Collections;

public class test : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnGUI()
    {
        if (GUI.Button(new Rect(0f, 0f, 100f, 100f), "test")) { print("test"); }
        //float buttonContainerSize = Screen.height / 6;
        //Rect buttonContainerRect = new Rect(0f, Screen.height - buttonContainerSize, buttonContainerSize * 1.5f, buttonContainerSize);
        //GUI.BeginGroup(buttonContainerRect);
        //{
        //    Rect button = new Rect(0f, 0f, buttonContainerRect.width, buttonContainerRect.height / 5);
        //    if (GUI.Button(new Rect(button.xMin, button.height * 0, button.width, button.height), "Reset Simulation"))
        //    {
        //        print("test");
        //    }
        //    if (GUI.Button(new Rect(button.xMin, button.height * 1, button.width, button.height), "Start Simulation"))
        //    {
        //        //startSimulation();
        //    }
        //    if (GUI.Button(new Rect(button.xMin, button.height * 2, button.width, button.height), "Register as Participant"))
        //    {
        //        //registerUser();
        //    }
        //    if (GUI.Button(new Rect(button.xMin, button.height * 3, button.width, button.height), "Launch Case"))
        //    {
        //        //launchCase();
        //    }
        //    if (GUI.Button(new Rect(button.xMin, button.height * 4, button.width, button.height), "End All Cases"))
        //    {
        //        //endAllCases();
        //    }
        //}
        //GUI.EndGroup();
    }
}
