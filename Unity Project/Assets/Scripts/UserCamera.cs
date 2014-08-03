using UnityEngine;

using System.Collections;



public class UserCamera : MonoBehaviour
{
    public Transform target;
    private SpectatorController spectatorController;
    private AgentController agentController;

    public float targetHeight = 1.7f;
    public float distance = 5.0f;
    public float maxDistance = 20;
    public float minDistance = 3f;
    public float xSpeed = 250.0f;
    public float ySpeed = 120.0f;

    public int yMinLimit = -80;
    public int yMaxLimit = 80;
    public int zoomRate = 40;

    public float rotationDampening = 3.0f;
    public float zoomDampening = 5.0f;

    private float x = 0.0f;
    private float y = 0.0f;
    private float currentDistance;
    private float desiredDistance;
    private float correctedDistance;

    void Start()
    {
        spectatorController = GetComponent<SpectatorController>();
        spectatorController.isActive = true;

        x = transform.eulerAngles.x;
        y = transform.eulerAngles.y;

        currentDistance = distance;
        desiredDistance = distance;
        correctedDistance = distance;
    }

    /**
     * Camera logic on LateUpdate to only update after all character movement logic has been handled.
     */
    void LateUpdate()
    {
        updateRotation();
        updatePosition();
    }

    private void updateRotation()
    {
        if (Input.GetMouseButton(1))
        {
            x += Input.GetAxis("Mouse X") * xSpeed * 0.02f;
            y -= Input.GetAxis("Mouse Y") * ySpeed * 0.02f;
        }
        else if (target != null)
        {
            if (Input.GetAxis("Vertical") != 0)
            {
                float targetRotationAngle = target.eulerAngles.y;
                float currentRotationAngle = transform.eulerAngles.y;
                x = Mathf.LerpAngle(currentRotationAngle, targetRotationAngle, rotationDampening * Time.deltaTime);
            }
        }
        y = ClampAngle(y, yMinLimit, yMaxLimit);
        transform.rotation = Quaternion.Euler(y, x, 0);
    }

    private void updatePosition()
    {
        if (target != null)
        {
            // calculate the desired distance
            desiredDistance -= Input.GetAxis("Mouse ScrollWheel") * Time.deltaTime * zoomRate * Mathf.Abs(desiredDistance);
            desiredDistance = Mathf.Clamp(desiredDistance, minDistance, maxDistance);
            correctedDistance = desiredDistance;

            // calculate desired camera position
            Vector3 position = target.position - (transform.rotation * Vector3.forward * desiredDistance + new Vector3(0, -targetHeight, 0));

            // check for collision using the true target's desired registration point as set by user using height
            RaycastHit collisionHit;
            Vector3 trueTargetPosition = new Vector3(target.position.x, target.position.y + targetHeight, target.position.z);

            // if there was a collision, correct the camera position and calculate the corrected distance
            bool isCorrected = false;
            if (Physics.Linecast(trueTargetPosition, position, out collisionHit))
            {
                position = collisionHit.point;
                correctedDistance = Vector3.Distance(trueTargetPosition, position);
                isCorrected = true;
            }

            // For smoothing, lerp distance only if either distance wasn't corrected, or correctedDistance is more than currentDistance
            currentDistance = !isCorrected || correctedDistance > currentDistance ? Mathf.Lerp(currentDistance, correctedDistance, Time.deltaTime * zoomDampening) : correctedDistance;

            // recalculate position based on the new currentDistance
            transform.position = target.position - (transform.rotation * Vector3.forward * currentDistance + new Vector3(0, -targetHeight, 0));
        }
    }

    private float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360)
            angle += 360;
        if (angle > 360)
            angle -= 360;
        return Mathf.Clamp(angle, min, max);
    }

    public void AssumeControlOfAgent(GameObject agent)
    {
        if (target != agent.transform)
        {
            spectatorController.isActive = false;
            if (target != null)
            {
                agentController.isActive = false;
            }
            target = agent.transform;
            agentController = agent.GetComponent<AgentController>();
            agentController.isActive = true;
        }
    }

    public void RelinquishControlOfAgent()
    {
        if (agentController != null)
        {
            agentController.isActive = false;
            agentController = null;
        }
        target = null;
        spectatorController.isActive = true;
    }
}