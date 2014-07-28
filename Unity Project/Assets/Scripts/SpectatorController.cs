using UnityEngine;
using System.Collections;

public class SpectatorController : MonoBehaviour
{
    protected float forwardSpeed = 30.0f;
    protected float upSpeed = 30.0f;
    protected Vector3 moveDirection = Vector3.zero;
    public bool active = true;

    protected void Update()
    {
        if (!active)
        {
            return;
        }

        moveDirection = transform.rotation * new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        moveDirection = moveDirection.normalized * forwardSpeed;

        if (Input.GetKey(KeyCode.Space))
        {
            moveDirection.y = upSpeed;
        }

        transform.position += moveDirection * Time.deltaTime;

        Screen.lockCursor = Input.GetMouseButton(1);
    }
}
