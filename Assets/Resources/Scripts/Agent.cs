using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Agent : MonoBehaviour
{   
    public Vector3 direction;

    [HideInInspector]
    public GameObject predator;
    public Vector3 flockmateCollisionAvoidance = Vector3.zero;
    public Vector3 averageFlockmateVelocity = Vector3.zero;
    public Vector3 averageFlockCenter = Vector3.zero;
    public int numFlockmates = 0;

    public bool debug = false;

    private float averageSpeed;
    private List<(string name, float magnitude, Vector3 direction)> accumulator;

    [Header ("References")]
    public AgentSettings settings;

    private void Awake()
    {
        ResetAccumulators();

        averageSpeed = (settings.minSpeed + settings.maxSpeed) / 2f;

        float x = Random.Range(-1f, 1f);
        float y = Random.Range(-1f, 1f);
        direction = new Vector3(x, y, 0f).normalized;

        direction *= averageSpeed;
    }

    public void AgentUpdate(float dt)
    {                    
        AvoidWalls();
        MoveToFlockCenter();
        AvoidFlockmateCollisions();
        MatchVelocity();
        if (predator)
        {
            EscapeFromPredator();
        }

        UpdateDirection();
        Move(dt);
        RotateInMoveDirection();
    }

    private void Move(float dt)
    {
        transform.Translate(direction * dt, Space.World);

        direction = direction.normalized * averageSpeed;
    }

    private void AvoidFlockmateCollisions()
    {
        RequestDirection(settings.flockmateAvoidanceWeight * flockmateCollisionAvoidance, "Avoid Flockmates");
        if(debug)
        {
            Debug.DrawRay(transform.position, flockmateCollisionAvoidance * settings.flockmateAvoidanceWeight, Color.magenta);
        }
    }

    public void AddFlockmateAvoidance(Vector3 offset)
    {
        float distance = offset.magnitude;
        Vector3 newAvoidance = ((-offset / distance) * InvSquare(distance, settings.flockmateAvoidanceSoftener));
        Vector3 oldAvoidance = (-offset / (distance * distance));
        flockmateCollisionAvoidance += newAvoidance;
        if (debug)
        {
            Debug.Log(string.Format("InvSquare: {0}, OldMethod: {0}", newAvoidance, oldAvoidance));
        }
    }

    private void MatchVelocity()
    {
        if (numFlockmates == 0)
        {
            return;
        }
        averageFlockmateVelocity /= numFlockmates;
        RequestDirection(settings.velocityMatchingWeight * averageFlockmateVelocity, "Match Velocity");

        if(debug)
        {
            Debug.DrawRay(transform.position, averageFlockmateVelocity, Color.blue);
        }
    }

    private void MoveToFlockCenter()
    {
        if (numFlockmates == 0)
        {
            return;
        }
        averageFlockCenter /= numFlockmates;
        RequestDirection(settings.flockCenteringWeight * (averageFlockCenter - transform.position), "Move to Center");
        if(debug)
        {
            Debug.DrawRay(transform.position, averageFlockCenter - transform.position, Color.yellow);
            Debug.Log("Average Flock Center: " + averageFlockCenter);
        }
    }

    private void AvoidWalls()
    {
        RaycastHit2D raycastHit;

        foreach (var angle in AngleCalculator.detectionAngles)
        {
            Vector3 dir = Quaternion.AngleAxis(angle, Vector3.forward) * direction.normalized;
            raycastHit = Physics2D.CircleCast(transform.position, settings.circleCastRadius, dir, settings.collisionAvoidDistance, settings.obstacleLayer);

            Color debugColor = Color.white;

            if(raycastHit)
            {
                float modifier = InvSquare(raycastHit.distance, 3);
                RequestDirection(settings.obstacleAvoidanceWeight * modifier * -dir, "Reduce Velocity");
                debugColor = Color.red;
            }

            if (debug)
            {
                Debug.DrawRay(transform.position, dir.normalized * settings.collisionAvoidDistance, debugColor);
            }
        }
    }

    private void EscapeFromPredator()
    {
        var offset = transform.position - predator.transform.position;
        var escapeDirection = Vector3.Normalize(offset) * settings.escapeWeight;
        escapeDirection *= InvSquare(offset.magnitude, 10f);

        RequestDirection(escapeDirection, "Escape From Predator!");
    }

    private void RequestDirection(Vector3 dir, string name)
    {
        accumulator.Add((name, dir.magnitude, dir.normalized));
    }

    private void UpdateDirection()
    {
        foreach (var request in accumulator)
        {
            direction += request.direction * request.magnitude;
            if(debug)
            {
                Debug.Log("Taking " + request.name + " into account. Request magnitude: " + request.magnitude);
            }
        }

        direction = Vector3.ClampMagnitude(direction, settings.maxSpeed);
        if (direction.sqrMagnitude < (settings.minSpeed * settings.minSpeed))
        {
            direction = direction.normalized * settings.minSpeed;
        }
    }

    public void ResetAccumulators()
    {
        accumulator = new List<(string name, float magnitude, Vector3 direction)>();
        flockmateCollisionAvoidance = Vector3.zero;
        averageFlockCenter = Vector3.zero;
        averageFlockmateVelocity = Vector3.zero;
    }

    private void RotateInMoveDirection()
    {
        var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
        var rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        transform.rotation = rotation;
    }

    public Vector3 GetDirection()
    {
        return direction;
    }

    public static float InvSquare(float x, float softener) 
    {
        float eps = 0.000001f;
        float result = Mathf.Pow((x + eps) / softener, -2);

        return result;
    }
}
