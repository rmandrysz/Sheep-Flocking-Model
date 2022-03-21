using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class AgentSettings : ScriptableObject
{
    public float initialMinSpeed = 0f;
    public float finalMinSpeed = 2f;
    public float initialMaxSpeed = 10f;
    public float finalMaxSpeed = 15f;

    [Header ("Collision Avoidance")]
    public float collisionAvoidDistance = 20f;
    public float circleCastRadius = 0.3f;
    public LayerMask obstacleLayer;
    public float coneOfSightAngle = 10f;

    [Header ("Weights")]
    public float flockmateAvoidanceWeight = 1f;
    public float flockCenteringWeight = 1f;
    public float velocityMatchingWeight = 1f;
    public float obstacleAvoidanceWeight = 2f;
    public float escapeWeight = 0.7f;

    [Header ("Flockmate Avoidance")]
    public float flockmateAvoidanceRadius = 3f;
    public float flockmateAvoidanceSoftener = 1f;

    [Header ("Flocking")]
    public float flockmateDetectionRadius = 5f;
    public float sightAngle = 120f;

    [Header ("Predator Adaptation")]
    public float flightZoneRadius = 50f;

    [Header ("Adjusted Weights")]
    public float adjustedFlockmateAvoidanceWeight = 0.2f;
    public float adjustedFlockCenteringWeight = 0.8f;
    public float adjustedVelocityMatchingWeight = 0.5f;
}
