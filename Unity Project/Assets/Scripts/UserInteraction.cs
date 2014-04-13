using UnityEngine;
using System.Collections;
//using DLLTestNamespace;
using Veis.Unity.Simulation;
using Veis.Unity.Logging;

public class UserInteraction : MonoBehaviour
{
    protected UnitySimulation simulation;

    private void Start()
    {
        UnityEngine.Debug.Log("test");
        //DLLTestClass.DLLTestMethod();
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
            userLeftClicked();
        }
    }

    private void userLeftClicked()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            Debug.Log("Clicked on " + hit.collider.name);
            //Application.OpenURL("http://localhost/forms/kill_asset_work.php?user_key=9fa79ecf-40da-40d3-9b4c-cb8451efd90e&asset=f5637d0b-8904-4741-a16b-553965423b92");
            simulation.Send("help");
        }
    }
}
