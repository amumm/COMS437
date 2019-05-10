using UnityEngine;
using System.Collections;

public class Movement : MonoBehaviour
{
    public float moveSpeed = 4.0F;
    public float rotationSpeed = 5.0F;
    public float jumpSpeed = 8.0F;
    public float gravity = 20.0F;
    private Vector3 moveDirection = Vector3.zero;

    public CharacterController controller;


    void Start()
    {
    }

    void Update()
    {
        var forward = Input.GetKey("w");
        var backward = Input.GetKey("s");
        var turnRight = Input.GetKey("a");
        var turnLeft = Input.GetKey("d");

        if (controller.isGrounded)
        {
            moveDirection.x *= 0.8f;
            moveDirection.z *= 0.8f;

            if (forward)
            {
                moveDirection = transform.forward * moveSpeed;
            }
            if (backward)
            {
                moveDirection = transform.forward * -moveSpeed;
            }

            if (Input.GetKey("space"))
            {
                moveDirection.y = jumpSpeed;
            }

        }
        else {
            moveDirection.x *= 0.995f;
            moveDirection.z *= 0.995f;
        }

        if (turnRight)
        {
            transform.Rotate(new Vector3(0, transform.rotation.y - rotationSpeed, 0), 100.0f * Time.deltaTime);
        }

        if (turnLeft)
        {
            transform.Rotate(new Vector3(0, transform.rotation.y + rotationSpeed, 0), 100.0f * Time.deltaTime);
        }


        moveDirection.y -= gravity * Time.deltaTime;

        controller.Move(moveDirection * Time.deltaTime);
    }

}