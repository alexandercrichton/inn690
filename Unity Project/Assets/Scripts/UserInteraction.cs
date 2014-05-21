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
        _simulation.WorldStateUpdated += OnWorldStateUpdated;
        clickableObjects = new List<GameObject>(GameObject.FindGameObjectsWithTag("Asset"));
    }

    private void Update()
    {
        _simulation.Update();
        updateGUIText();
        handleUserInteraction();
    }

    #region User Click

    private void handleUserInteraction()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            GameObject clickedObject = getObjectUserClickedOn();

            if (clickedObject != null && clickedObject.tag == "Asset")
            {
                launchAsset(clickedObject.GetComponent<Asset>());
            }
        }
    }

    private GameObject getObjectUserClickedOn()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            return hit.collider.gameObject;
        }
        return null;
    }

    #endregion

    #region GUI

    private void updateGUIText()
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
        drawCaseInfo();
        //drawUserTextInputArea();
        drawSimulationControlButtons();
    }

    private void drawClickableObjectLabels()
    {
        GUIStyle labelStyle = new GUIStyle();
        labelStyle.alignment = TextAnchor.MiddleCenter;
        labelStyle.normal.textColor = Color.white;
        labelStyle.fontSize = 20;

        foreach (GameObject clickableObject in clickableObjects)
        {
            if (checkIsObjectInCameraView(clickableObject))
            {
                Vector3 origin = Camera.main.transform.position;
                Vector3 direction = clickableObject.transform.position - origin;
                RaycastHit hit;
                if (Physics.Raycast(origin, direction, out hit))
                {
                    if (hit.collider.name == clickableObject.name)
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
            }            
        }
    }

    private bool checkIsObjectInCameraView(GameObject gmObject)
    {
        Vector3 viewportPoint = Camera.main.WorldToViewportPoint(gmObject.transform.position);
        return (viewportPoint.x > 0 && viewportPoint.x < 1) 
            && (viewportPoint.y > 0 && viewportPoint.y < 1) 
            && (viewportPoint.z > 0);
    }

    private void drawCaseInfo()
    {
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
    }

    private void drawUserTextInputArea()
    {
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

    private void handleUserTextInput()
    {
        print(userTextInput);
        _simulation.Send(userTextInput);
    }

    private void drawSimulationControlButtons()
    {
        float buttonContainerSize = Screen.height / 6;
        Rect buttonContainerRect = new Rect(0f, Screen.height - buttonContainerSize, buttonContainerSize * 1.5f, buttonContainerSize);
        GUI.BeginGroup(buttonContainerRect);
        {
            Rect button = new Rect(0f, 0f, buttonContainerRect.width, buttonContainerRect.height / 5);
            if (GUI.Button(new Rect(button.xMin, button.height * 0, button.width, button.height), "Reset Simulation"))
            {
                resetSimulation();
            }
            if (GUI.Button(new Rect(button.xMin, button.height * 1, button.width, button.height), "Start Simulation"))
            {
                startSimulation();
            }
            if (GUI.Button(new Rect(button.xMin, button.height * 2, button.width, button.height), "Register as Participant"))
            {
                registerUser();
            }
            if (GUI.Button(new Rect(button.xMin, button.height * 3, button.width, button.height), "Launch Case"))
            {
                launchCase();
            }
            if (GUI.Button(new Rect(button.xMin, button.height * 4, button.width, button.height), "End All Cases"))
            {
                endAllCases();
            }
        }
        GUI.EndGroup();
    }

    #endregion

    #region Simulation Methods

    void Logger_LogMessage(object sender, Veis.Data. Logging.LogEventArgs e)
    {
        Debug.Log("[" + e.EventInitiator.ToString() + "]: " + e.Message);
    }

    private void resetSimulation()
    {
        _simulation.PerformSimulationAction(Veis.Simulation.SimulationActions.Start);
    }

    private void startSimulation()
    {
        _simulation.PerformSimulationAction(Veis.Simulation.SimulationActions.Reset);
    }

    private void OnWorldStateUpdated()
    {
        print("World state updated");
    }

    private void launchCase()
    {
        print("launch");
        _simulation.RequestLaunchCase(CASE_ID);
    }

    private void endAllCases()
    {
        _simulation.RequestCancelAllCases();
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
        _simulation.RegisterUser(new Veis.Simulation.UserArgs
        {
            UserName = "Janie May",
            RoleName = "Janie May",
            UserId = uuid
        });
        _userKey = _assetKey = uuid;
        _userName = "Janie May";
    }

    private void launchAsset(Asset asset)
    {
        Application.OpenURL(
            "http://localhost/forms/launch_asset.php"
            + "?user_key=" + _userKey
            + "&asset_name=" + WWW.EscapeURL(asset.AssetName)
            + "&asset_key=" + _assetKey
            + "&user_name=" + WWW.EscapeURL(asset.AssetKey)
            );
    }

    private void OnApplicationQuit()
    {
        _simulation.Send("endsession");
    }

    #endregion
}
