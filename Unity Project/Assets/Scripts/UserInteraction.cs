using UnityEngine;
using System.Collections;

public class UserInteraction : MonoBehaviour
{
    private void Start()
    {

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
            Application.OpenURL("http://localhost");
        }
    }
}
