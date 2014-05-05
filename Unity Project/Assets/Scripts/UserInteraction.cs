using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Veis.Unity.Simulation;
using Veis.Unity.Logging;

public class UserInteraction : MonoBehaviour
{
    protected UnitySimulation _simulation;
    //protected const string CASE_ID = "UID_142f2e5a-7c7c-4d2e-a684-02c230e3689d CarAccident";
    protected const string CASE_ID = "CarAccident";
    protected string _userKey = "";
    protected string _assetName = "";
    protected string _assetKey = "";
    protected string _userName = "";

    protected string _yawlInfo = "";
    protected string _simulationInfo = "";
    protected string _caseInfo = "";

    protected string userTextInput = "";

    protected List<GameObject> clickableObjects;

    private void Start()
    {
        UnityLogger.LogMessage += Logger_LogMessage;
        _simulation = new UnitySimulation();
        clickableObjects = new List<GameObject>();
        clickableObjects.AddRange(new List<GameObject>(GameObject.FindGameObjectsWithTag("Clickable")));
        clickableObjects.AddRange(new List<GameObject>(GameObject.FindGameObjectsWithTag("Asset")));
    }

    private void Update()
    {
        updateGUI();
        handleUserInteraction();
    }

    #region User Click

    private void handleUserInteraction()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            GameObject clickedObject = getObjectUserClickedOn();

            if (clickedObject.tag == "Asset")
            {
                launchAsset(clickedObject.name);
            }

            else if (clickedObject.name == "SimulationReset")
            {
                _simulation.PerformSimulationAction(Veis.Simulation.SimulationActions.Reset);
            }
            else if (clickedObject.name == "SimulationStart")
            {
                _simulation.PerformSimulationAction(Veis.Simulation.SimulationActions.Start);
            }
            else if (clickedObject.name == "LaunchCase")
            {
                launchCase();
            }
            else if (clickedObject.name == "EndAllCases")
            {
                endAllCases();
            }
            else if (clickedObject.name == "RegisterUser")
            {
                registerUser();
            }
        }
    }

    private GameObject getObjectUserClickedOn()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            //Application.OpenURL("http://localhost/forms/kill_asset_work.php?user_key=9fa79ecf-40da-40d3-9b4c-cb8451efd90e&asset=f5637d0b-8904-4741-a16b-553965423b92");
            //simulation.Send("help");
            return hit.collider.gameObject;
            //if (hit.collider.name == objectName)
            //{
            //    //Debug.Log("Clicked on " + hit.collider.name);
            //    return true;
            //}
        }
        return null;
    }

    private void launchCase()
    {
        _simulation.ProcessScriptCommand("LaunchCase|" + CASE_ID);
        //print(simulation._workflowProvider.StartedCases.ToString());
    }

    private void endAllCases()
    {
        _simulation.ProcessScriptCommand("EndAllCases");
    }

    private void registerUser()
    {
        string key = "";
        foreach (var entry in _simulation._workflowProvider.AllParticipants)
        {
            if (entry.Value.FirstName == "Janie")
            {
                key = entry.Key;
            }
        }
        string uuid = _simulation._workflowProvider.AllParticipants[key].AgentId;
        if (uuid.Length > 36)
        {
            uuid = uuid.Substring(uuid.Length - 36, 36);
        }
        _simulation.ProcessScriptCommand("RegisterUser|Janie May|" + uuid);
        _userKey = _assetKey = uuid;
        _userName = "Janie May";
    }

    private void launchAsset(string assetName)
    {
        Application.OpenURL(
            "http://localhost/forms/launch_asset.php"
            + "?user_key=" + _userKey
            + "&asset_name=" + WWW.EscapeURL(assetName)
            + "&asset_key=" + _assetKey
            + "&user_name=" + WWW.EscapeURL(_userName)
            );
        Debug.Log(
            "http://localhost/forms/launch_asset.php"
            + "?user_key=" + _userKey
            + "&asset_name=" + WWW.EscapeURL(assetName)
            + "&asset_key=" + _assetKey
            + "&user_name=" + WWW.EscapeURL(_userName)
            );
    }

    #endregion

    #region GUI

    private void updateGUI()
    {
        _caseInfo = "No case";

        foreach (var startedCase in _simulation._workflowProvider.StartedCases)
        {
            _caseInfo = "Current Case: " + startedCase.SpecificationName + " "
                + startedCase.SpecificationId;
            foreach (var human in _simulation._humans)
            {
                if (human.WorkProvider.GetGoals().Count > 0)
                {
                    foreach (var workItem in human.WorkProvider.GetGoals())
                    {
                        _caseInfo += "\nCurrent Task: " + workItem.Key.taskName;
                        foreach (var goal in workItem.Value)
                        {
                            _caseInfo += "\n" + goal.ToString();
                        }
                    }
                }
            }  
        }      
    }

    private void OnGUI()
    {
        drawClickableObjectLabels();

        Rect workItemsRect = new Rect(0f, 0f, Screen.width, Screen.height);
        GUI.BeginGroup(workItemsRect);
        {
            GUILayout.BeginVertical();
            {
                GUILayout.Label(_yawlInfo);
                GUILayout.Label(_simulationInfo);
                GUILayout.Label(_caseInfo);
            }
            GUILayout.EndVertical();
        }
        GUI.EndGroup();

        Rect userInputRect = new Rect(0f, Screen.height - 50f, Screen.width, 50f);
        if (Event.current.type == EventType.keyDown && Event.current.keyCode == KeyCode.Return)
        {
            handleUserTextInput();
        }
        else if (Event.current.keyCode != KeyCode.Return)
        {
            userTextInput = GUI.TextArea(userInputRect, userTextInput.Replace("\n", ""));
        }
    }

    private void drawClickableObjectLabels()
    {
        GUIStyle labelStyle = new GUIStyle();
        labelStyle.alignment = TextAnchor.MiddleCenter;
        labelStyle.normal.textColor = Color.white;
        labelStyle.fontSize = 20;

        foreach (GameObject clickableObject in clickableObjects)
        {
            Vector3 objectScreenPosition = Camera.main.WorldToScreenPoint(clickableObject.transform.position);
            string objectLabel = clickableObject.name;
            float objectLabelSize = 200f;
            Rect objectLabelRect = new Rect(
                objectScreenPosition.x - objectLabelSize / 2,
                Screen.height - objectScreenPosition.y - objectLabelSize / 2,
                objectLabelSize,
                objectLabelSize);
            GUI.Label(objectLabelRect, objectLabel, labelStyle);
        }
    }

    private void handleUserTextInput()
    {
        print(userTextInput);
        _simulation.Send(userTextInput);
    }

    #endregion

    void Logger_LogMessage(object sender, Veis.Data. Logging.LogEventArgs e)
    {
        Debug.Log("[" + e.EventInitiator.ToString() + "]: " + e.Message);

        //if (sender != _simulation)
        //{
        //    logMessage += ", from " + sender.ToString();
        //}
        //Debug.Log(logMessage);
    }

    private void OnApplicationQuit()
    {
        _simulation.Send("endsession");
    }
}
