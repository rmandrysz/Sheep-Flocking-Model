using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Predator : MonoBehaviour
{
    public Vector3 targetPosition = Vector3.zero;
    public float speed = 4f;
    public bool debug = false;

    public void PredatorUpdate(float dt) 
    {
        Move(dt);
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            targetPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            targetPosition.z = 0;
            if(debug)
            {
                print(targetPosition);
            }
        }
        if (Input.GetMouseButtonDown(1))
        {
            targetPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            targetPosition.z = 0;
            TeleportToTargetPosition();
        }
    }

    private void Move(float dt)
    {
        Vector3 nextPosition = Vector3.MoveTowards(transform.position, targetPosition, speed * dt);

        if (nextPosition != transform.position)
        {
            RotateInMoveDirection(nextPosition);
        }

        transform.position = nextPosition;
    }

    private void TeleportToTargetPosition()
    {
        RotateInMoveDirection(targetPosition);
        transform.position = targetPosition;
    }

    private void RotateInMoveDirection(Vector3 nextPosition)
    {
        var relativePos = nextPosition - transform.position;

        Quaternion targetRotation = Quaternion.LookRotation(Vector3.forward, relativePos.normalized);

        transform.rotation = targetRotation;
    }
}
