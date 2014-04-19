using UnityEngine;
using System.Collections;
using Veis.Unity.Simulation;
using Veis.Unity.Logging;

public class UserInteraction : MonoBehaviour
{
    protected UnitySimulation simulation;
    //protected const string CASE_ID = "UID_142f2e5a-7c7c-4d2e-a684-02c230e3689d CarAccident";
    protected const string CASE_ID = "CarAccident";

    private void Start()
    {
        Logger.LogMessage += Logger_LogMessage;
        simulation = new UnitySimulation();
    }

    void Logger_LogMessage(object sender, LogEventArgs e)
    {
        Debug.Log(e.Message);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            if (userClickedOnObject("StartCase"))
            {
                simulation.ProcessScriptCommand("LaunchCase|" + CASE_ID);
                //print(simulation._workflowProvider.StartedCases.ToString());

            }
            if (userClickedOnObject("EndAllCases"))
            {
                foreach (var startedCase in simulation._workflowProvider.StartedCases)
                {
                    print(startedCase.CaseId);
                }
                foreach (var workItem in simulation._workflowProvider.AllWorkItems)
                {
                    print(workItem.Value);
                }
                foreach (var workItem in simulation._workflowProvider.AllParticipants)
                {
                    print(workItem.Value);
                }
                foreach (var workItem in simulation._workflowProvider.AllSpecifications)
                {
                    print(workItem.Value);
                }
                simulation.ProcessScriptCommand("EndAllCases");
            }

        }
    }

    private bool userClickedOnObject(string objectName)
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            Debug.Log("Clicked on " + hit.collider.name);
            //Application.OpenURL("http://localhost/forms/kill_asset_work.php?user_key=9fa79ecf-40da-40d3-9b4c-cb8451efd90e&asset=f5637d0b-8904-4741-a16b-553965423b92");
            //simulation.Send("help");

            if (hit.collider.name == objectName)
            {
                return true;
            }
        }
        return false;
    }

    private void OnApplicationQuit()
    {
        simulation.Send("endsession");
    }
}
