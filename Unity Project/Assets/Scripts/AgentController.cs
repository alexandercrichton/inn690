using UnityEngine;
using System.Collections;

public class AgentController : MonoBehaviour
{
    protected float jumpSpeed = 8.0f;
    protected float gravity = 20.0f;
    protected float moveSpeed = 20.0f;
    protected float rotateSpeed = 150.0f;

    protected bool grounded = false;
    public bool isActive = false;

    protected Vector3 moveDirection = Vector3.zero;

    protected CharacterController controller;

    // Use this for initialization
    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    protected void Update()
    {
        if (!isActive)
        {
            return;
        }

        bool RMB = Input.GetMouseButton(1);
        Screen.lockCursor = RMB;

        if (RMB)
        {
            transform.rotation = Quaternion.Euler(0, Camera.main.transform.eulerAngles.y, 0);
        }
        else
        {
            transform.Rotate(0, Input.GetAxis("Horizontal") * rotateSpeed * Time.deltaTime, 0);
        }

        if (grounded)
        {
            moveDirection = new Vector3((RMB ? Input.GetAxis("Horizontal") : 0), 0, Input.GetAxis("Vertical"));
            moveDirection = moveDirection.normalized * moveSpeed;

            if (Input.GetButton("Jump"))
            {
                moveDirection.y = jumpSpeed; 
            }
        }

        moveDirection.y -= gravity * Time.deltaTime;

        CollisionFlags flags = controller.Move(transform.rotation * moveDirection * Time.deltaTime);
        grounded = ((flags & CollisionFlags.Below) != 0);
    }
}
