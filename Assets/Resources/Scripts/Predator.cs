using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Predator : MonoBehaviour
{
    public Vector3 targetPosition = Vector3.zero;
    public float speed = 4f;
    public bool debug = false;
    public bool manualControl = false;

    public static Predator Spawn(GameObject prefab, PlaygroundSettings settings)
    {
        var cam = Camera.main;
        Vector3 spawnPosition = cam.ScreenToWorldPoint(Input.mousePosition);
        spawnPosition.z = 0;
        Vector3 targetPosition = spawnPosition;
        
        if (!settings.manualPredatorControl)
        {
            spawnPosition = cam.ScreenToWorldPoint(new(0, 0));
            spawnPosition.z = 0;
            targetPosition = cam.ScreenToWorldPoint(new(cam.pixelWidth, cam.pixelHeight));
            targetPosition.z = 0;
        }

        Predator predator = Instantiate(prefab, spawnPosition, Quaternion.identity).GetComponent<Predator>();
        predator.manualControl = settings.manualPredatorControl;
        predator.targetPosition = targetPosition;

        return predator;
    }

    public void UpdatePredator(float dt) 
    {
        Move(dt);
    }

    private void Update()
    {
        if (!manualControl)
        {
            return;
        }

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
