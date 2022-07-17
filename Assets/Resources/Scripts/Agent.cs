using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Agent : MonoBehaviour
{   
    public Vector3 direction;

    [HideInInspector]
    public Transform predator;
    public Vector3 flockmateCollisionAvoidance = Vector3.zero;
    public Vector3 averageFlockmateVelocity = Vector3.zero;
    public Vector3 averageFlockCenter = Vector3.zero;
    public int numFlockmates = 0;

    public bool debug = false;

    private float minSpeed;
    private float maxSpeed;
    private Vector3 previousDirection;
    private List<(string name, float magnitude, Vector3 direction)> accumulator;

    [Header ("References")]
    public AgentSettings settings;

    private void Awake()
    {
        ResetAccumulators();

        float x = Random.Range(-1f, 1f);
        float y = Random.Range(-1f, 1f);
        direction = new Vector3(x, y, 0f).normalized;
        previousDirection = Vector3.zero;
    }

    public void AgentUpdate(float dt)
    {
        AdjustSpeedLimits();          
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

        if (predator && debug)
        {
            float distance = (predator.position - transform.position).magnitude;
            Debug.Log(string.Format("Distance: {0}, Flightzone Radius: {1}, SmoothStep Value: {2}", distance, settings.flightZoneRadius, PredatorSmoothStep()));
        }
    }

    private void Move(float dt)
    {
        transform.Translate(direction * dt, Space.World);

        if (direction.sqrMagnitude < 0.01f)
        {
            previousDirection = direction;
            direction = Vector3.zero;
            return;
        }

        if (direction.sqrMagnitude != 0f)
        {
            RotateInMoveDirectionSmooth();
        }

        previousDirection = direction;
        direction = Vector3.zero;
    }

    private void AvoidFlockmateCollisions()
    {
        float weight = settings.flockmateAvoidanceWeight;
        if (predator)
        {
            float diff = settings.adjustedFlockmateAvoidanceWeight - settings.flockmateAvoidanceWeight;
            weight += PredatorSmoothStep() * diff;
        }

        RequestDirection(weight * flockmateCollisionAvoidance, "Avoid Flockmates");

        if(debug)
        {
            Debug.DrawRay(transform.position, flockmateCollisionAvoidance * weight, Color.magenta);
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
            // Debug.Log(string.Format("InvSquare: {0}, OldMethod: {0}", newAvoidance, oldAvoidance));
        }
    }

    private void MatchVelocity()
    {
        if (numFlockmates == 0)
        {
            return;
        }
        
        if(debug)
        {
            Debug.DrawRay(transform.position, averageFlockmateVelocity, Color.blue);
        }

        if (!predator)
        {
            return;
        }
        float diff = settings.adjustedVelocityMatchingWeight - settings.velocityMatchingWeight;
        float weight = PredatorSmoothStep() * settings.adjustedVelocityMatchingWeight;
        averageFlockmateVelocity /= numFlockmates;
        RequestDirection(weight * averageFlockmateVelocity, "Match Velocity");
    }

    private void MoveToFlockCenter()
    {
        if (numFlockmates == 0)
        {
            return;
        }

        if(debug)
        {
            Debug.DrawRay(transform.position, averageFlockCenter - transform.position, Color.yellow);
            // Debug.Log("Average Flock Center: " + averageFlockCenter);
        }

        if (!predator)
        {
            return;
        }

        float diff = settings.adjustedFlockCenteringWeight - settings.flockCenteringWeight;
        float weight = PredatorSmoothStep() * settings.adjustedFlockCenteringWeight;
        averageFlockCenter /= numFlockmates;
        RequestDirection(weight * (averageFlockCenter - transform.position), "Move to Center");
    }

    private void AvoidWalls()
    {
        RaycastHit2D raycastHit;

        foreach (var angle in AngleCalculator.detectionAngles)
        {
            Vector3 dir = Quaternion.AngleAxis(angle, Vector3.forward) * previousDirection.normalized;
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
        var offset = transform.position - predator.position;
        var escapeDirection = Vector3.Normalize(offset) * PredatorSmoothStep() * settings.escapeWeight;
        // escapeDirection *= InvSquare(offset.magnitude, 10f);

        RequestDirection(escapeDirection, "Escape From Predator!");
    }

    private void RequestDirection(Vector3 dir, string name)
    {
        accumulator.Add((name, dir.magnitude, dir.normalized));
    }

    private void AdjustSpeedLimits()
    {
        var diff = settings.finalMaxSpeed - settings.initialMaxSpeed;
        maxSpeed = settings.initialMaxSpeed + (diff * PredatorSmoothStep());  
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

        direction = Vector3.ClampMagnitude(direction, maxSpeed);
        if (direction.sqrMagnitude < (minSpeed * minSpeed))
        {
            direction = direction.normalized * minSpeed;
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

    private void RotateInMoveDirectionSmooth()
    {
        var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
        var targetRotation = Quaternion.AngleAxis(angle, Vector3.forward);

        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.fixedDeltaTime * settings.rotationSpeed);
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

    public float Sigmoid(float softeningFactor = 1f)
    {
        if (!predator)
        {
            return 0;
        }
        var offset = transform.position - predator.position;
        return ((1 / Mathf.PI) * Mathf.Atan((settings.flightZoneRadius - offset.magnitude)) + 0.5f);
        // return 0f;
    }

    private float PredatorSmoothStep(float min = 0f, float max = 3f)
    {
        if (!predator)
        {
            return min;
        }

        float distance = (predator.position - transform.position).magnitude;
        float flipped = settings.flightZoneRadius -  distance;

        flipped = Mathf.Clamp(flipped, 0f, settings.flightZoneRadius);

        float interpolationPoint = (flipped / settings.flightZoneRadius);

        float result = Mathf.SmoothStep(min, max, interpolationPoint);

        return result;
    }
}
