using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Agent : MonoBehaviour
{   
    public Vector3 direction;

    [HideInInspector]
    public Vector3 flockmateCollisionAvoidance = Vector3.zero;
    public Vector3 averageFlockmateVelocity = Vector3.zero;
    public Vector3 averageFlockCenter = Vector3.zero;
    public int numFlockmates = 0;

    public bool debug = false;

    private float minSpeed;
    private float maxSpeed;
    public Vector3 previousDirection;
    private List<(string name, float magnitude, Vector3 direction)> accumulator;

    [Header ("References")]
    public AgentSettings settings;

    private void Awake()
    {
        ResetAccumulators();

        direction = Vector3.zero;
        previousDirection = Vector3.zero;
    }

    public void AgentUpdate(float dt, Transform predator)
    {
        AdjustSpeedLimits(predator);          
        MoveToFlockCenter(predator);
        AvoidFlockmateCollisions(predator);
        MatchVelocity(predator);
        if (predator)
        {
            EscapeFromPredator(predator);
        }

        UpdateDirection();
        Move(dt);
    }

    private void Move(float dt)
    {
        transform.Translate(direction * dt, Space.World);

        if (direction.sqrMagnitude != 0f)
        {
            RotateInMoveDirection(dt);
        }

        previousDirection = direction;
        direction = Vector3.zero;
    }

    private void AvoidFlockmateCollisions(Transform predator = null)
    {
        float weight = settings.flockmateAvoidanceWeight;
        if (predator)
        {
            float diff = settings.adjustedFlockmateAvoidanceWeight - settings.flockmateAvoidanceWeight;
            weight += PredatorSmoothStep(predator) * diff;
        }

        RequestDirection(weight * flockmateCollisionAvoidance, "Avoid Flockmates");
    }

    public void AddFlockmateAvoidance(Vector3 offset)
    {
        float distance = offset.magnitude;
        Vector3 newAvoidance = (-offset / distance) * InvSquare(distance, settings.flockmateAvoidanceSoftener);
        flockmateCollisionAvoidance += newAvoidance;
    }

    private void MatchVelocity(Transform predator = null)
    {
        if (numFlockmates == 0)
        {
            return;
        }
        
        if (debug)
        {
            Debug.DrawRay(transform.position, averageFlockmateVelocity, Color.blue);
        }

        if (!predator)
        {
            return;
        }
        float weight = PredatorSmoothStep(predator) * settings.velocityMatchingWeight;
        RequestDirection(weight * averageFlockmateVelocity, "Match Velocity");
    }

    private void MoveToFlockCenter(Transform predator = null)
    {
        if (numFlockmates == 0 || !predator)
        {
            return;
        }

        float weight = PredatorSmoothStep(predator) * settings.flockCenteringWeight;
        RequestDirection(weight * (averageFlockCenter - transform.position), "Move to Center");
    }

    public void AddObstacleAvoidance(Vector3 offset)
    {
        float distance = offset.magnitude;
        Vector3 avoidance = offset.normalized * InvSquare(distance, 15);
        RequestDirection(avoidance * settings.obstacleAvoidanceWeight, "Avoid Obstacle");
    }

    private void EscapeFromPredator(Transform predator)
    {
        var offset = transform.position - predator.position;
        var escapeDirection = PredatorSmoothStep(predator) * settings.escapeWeight * Vector3.Normalize(offset);

        RequestDirection(escapeDirection, "Escape From Predator!");
    }

    private void RequestDirection(Vector3 dir, string name)
    {
        accumulator.Add((name, dir.magnitude, dir.normalized));
    }

    private void AdjustSpeedLimits(Transform predator)
    {
        if (!predator)
        {
            maxSpeed = settings.initialMaxSpeed;
            minSpeed = settings.initialMinSpeed;
            return;
        }
        var diffMax = settings.finalMaxSpeed - settings.initialMaxSpeed;
        maxSpeed = settings.initialMaxSpeed + (diffMax * PredatorSmoothStep(predator));
        var diffMin = settings.finalMinSpeed - settings.initialMinSpeed;
        minSpeed = settings.initialMinSpeed + (diffMin * PredatorSmoothStep(predator));
    }

    private void UpdateDirection()
    {
        foreach (var request in accumulator)
        {
            direction += request.direction * request.magnitude;
        }
        var clampedDirection =  Vector3.ClampMagnitude(direction, maxSpeed);
        if (direction != clampedDirection)
        {
            Debug.Log("Velocity clamped. Diff: " + (direction - clampedDirection));
        }

        direction = clampedDirection;
        // if (direction.sqrMagnitude < (minSpeed * minSpeed))
        // {
        //     // if (!predator)
        //     // {
        //         direction = Vector3.zero;
        //     // }
        //     // else
        //     // {
        //     //     // direction = direction.normalized * minSpeed;
        //     // }
        // }
    }

    public void ResetAccumulators()
    {
        accumulator = new List<(string name, float magnitude, Vector3 direction)>();
        flockmateCollisionAvoidance = Vector3.zero;
        averageFlockCenter = Vector3.zero;
        averageFlockmateVelocity = Vector3.zero;
    }

    private void RotateInMoveDirection(float dt)
    {
        Quaternion targetRotation = Quaternion.LookRotation(Vector3.forward, direction.normalized);
        targetRotation = Quaternion.RotateTowards(transform.rotation, targetRotation, settings.maxRotationDegrees * dt);

        transform.rotation = targetRotation;
    }

    public static float InvSquare(float x, float softener) 
    {
        const float eps = 0.000001f;
        float result = Mathf.Pow((x + eps) / softener, -2);

        return result;
    }
    
    private float PredatorSmoothStep(Transform predator, float min = 0f, float max = 3f)
    {
        if (!predator)
        {
            return min;
        }

        float distance = (predator.position - transform.position).magnitude;
        float flipped = settings.flightZoneRadius -  distance;

        flipped = Mathf.Clamp(flipped, 0f, settings.flightZoneRadius);

        float interpolationPoint = flipped / settings.flightZoneRadius;

        float result = Mathf.SmoothStep(min, max, interpolationPoint);

        return result;
    }
}
