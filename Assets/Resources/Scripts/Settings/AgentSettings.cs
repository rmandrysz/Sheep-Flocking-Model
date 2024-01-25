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

    [Header ("Obstacle Avoidance")]
    public float obstacleAvoidSoftener = 15f;

    [Header ("Weights")]
    public float flockmateAvoidanceWeight = 1f;
    public float flockCenteringWeight = 1f;
    public float velocityMatchingWeight = 1f;
    public float obstacleAvoidanceWeight = 2f;
    public float escapeWeight = 0.7f;
    public float adjustedFlockmateAvoidanceWeight = 0.2f;

    [Header ("Flockmate Avoidance")]
    public float flockmateAvoidanceRadius = 3f;
    public float flockmateAvoidanceSoftener = 1f;

    [Header ("Flocking")]
    public float flockmateDetectionRadius = 5f;
    public float sightAngle = 120f;

    [Header ("Predator Adaptation")]
    public float flightZoneRadius = 50f;

    [Header ("Visuals")]
    public float maxRotationDegrees = 1f;

    [Header ("Predator Control")]
    public bool manualPredatorControl = false;

    [Header ("Spawn Rules")]
    public bool enableRandomSpawn = false;
    public int agentNumber = 46;
    public float spawnRadius = 20f;
}
