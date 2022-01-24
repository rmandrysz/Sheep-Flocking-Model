using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class AgentSettings : ScriptableObject
{
    public float minSpeed = 5f;
    public float maxSpeed = 10f;

    [Header ("Collision Avoidance")]
    public float collisionAvoidDistance = 20f;
    public float circleCastRadius = 0.3f;
    public LayerMask obstacleLayer;

    [Header ("Weights")]
    public float flockmateAvoidanceWeight = 1f;
    public float flockCenteringWeight = 1f;
    public float velocityMatchingWeight = 1f;
    public float obstacleAvoidanceWeight = 2f;
    public float maxInfluence = 5f;

    [Header ("Flockmate Avoidance")]
    public float flockmateAvoidanceRadius = 3f;

    [Header ("Flocking")]
    public float flockmateDetectionRadius = 5f;
    public float sightAngle = 120f;
}
