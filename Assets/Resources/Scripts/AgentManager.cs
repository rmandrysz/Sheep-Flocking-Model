using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentManager
{
    private static GameObject agentContainer;

    private readonly AgentSettings settings;
    private readonly GameObject prefab;
    public List<Agent> Agents {get;} = new();

    public AgentManager(AgentSettings settings, GameObject agentContainer, GameObject prefab)
    {
        AgentManager.agentContainer = agentContainer;

        this.settings = settings;
        this.prefab = prefab;
    }

    public void StartAgents(SimulationSettings simulationSettings)
    {
        if (settings.enableRandomSpawn)
        {
            SpawnRandom();
            return;
        }
        SpawnNonRandom(simulationSettings);
    }

    public Vector3 UpdateAgents(float dt, List<Transform> obstacles, Transform predator)
    {
        Calculate(obstacles);
        return AgentUpdate(dt, predator);
    }

    private void Calculate(List<Transform> obstacles)
    {
        float detectionRadius = settings.flockmateDetectionRadius;
        float avoidanceRadius = settings.flockmateAvoidanceRadius;

        for (int i = 0; i < Agents.Count; ++i)
        {
            var agent = Agents[i];

            agent.ResetAccumulators();
            int numFlockmates = 0;
            Vector3 flockCenterSum = Vector3.zero, flockmateVelocitySum = Vector3.zero;

            foreach(var obstacle in obstacles)
            {
                Vector3 offset = agent.transform.position - obstacle.position;
                agent.AddObstacleAvoidance(offset);
            }

            for (int j = 0; j < Agents.Count; j++)
            {
                var neighbor = Agents[j];
                if (i == j)
                {
                    continue;
                }

                Vector3 offset = neighbor.transform.position - agent.transform.position;
                float sqrDist = Vector3.SqrMagnitude(offset);

                if (sqrDist <= detectionRadius * detectionRadius && Vector2.Angle(offset, agent.previousDirection) < settings.sightAngle)
                {
                    if (i == 0)
                    {
                        Debug.DrawRay(agent.transform.position, agent.previousDirection.normalized * 3, Color.blue);
                    }
                    Color color = new(0f, 255f, 0f);
                    ++numFlockmates;
                    flockmateVelocitySum += neighbor.direction;
                    flockCenterSum += neighbor.transform.position;

                    if (sqrDist <= avoidanceRadius * avoidanceRadius)
                    {
                        agent.AddFlockmateAvoidance(offset);
                        color = new Color(255f, 0f, 0f,  10f * Vector3.SqrMagnitude(offset / sqrDist));
                    }
                    if (Agents[i].debug)
                    {
                        Debug.DrawRay(agent.transform.position, offset, color);
                    }
                }
            }

            agent.numFlockmates = numFlockmates;
            if (numFlockmates != 0)
            {
                agent.averageFlockmateVelocity = flockCenterSum / numFlockmates;
                agent.averageFlockCenter = flockmateVelocitySum / numFlockmates;
            }
        }
    }

    private Vector3 AgentUpdate(float dt, Transform predator)
    {
        Vector3 centerOfMass = Vector3.zero;

        Agents.ForEach(agent => {
            agent.AgentUpdate(dt, predator);
            centerOfMass += agent.transform.position;
        });

        return centerOfMass / Agents.Count;
    }
    
    private void SpawnRandom()
    {
        for (int i = 0; i < settings.agentNumber; ++i)
        {
            float x = Random.Range(-settings.spawnRadiusForRandomSpawn, settings.spawnRadiusForRandomSpawn);
            float y = Random.Range(-settings.spawnRadiusForRandomSpawn, settings.spawnRadiusForRandomSpawn);
            Vector2 position = new(x, y);
            Quaternion rotation = Quaternion.Euler(0f, 0f, Random.Range(-180f, 180f));

            Agent agent = GameObject.Instantiate(
                    prefab, position, rotation, agentContainer.transform).GetComponent<Agent>();

            Agents.Add(agent);
        }
    }

    private void SpawnNonRandom(SimulationSettings simulationSettings)
    {
        var positions = CalculateSpawnPositions(simulationSettings);
        foreach(var pos in positions)
        {
            Agents.Add(
                GameObject.Instantiate(
                    prefab, pos, Quaternion.identity, agentContainer.transform).GetComponent<Agent>());
        }
        Debug.Log("Agents spawned: " + Agents.Count);
    }

    private List<Vector2> CalculateSpawnPositions(SimulationSettings simulationSettings)
    {
        List<Vector2> result = new();

        int agentsInOddRow = 6;
        int agentsInEvenRow = agentsInOddRow + 1;

        float xOffset = (simulationSettings.horizontalWallOffset - 30f) * 2 / agentsInOddRow;
        float yOffset = xOffset * Mathf.Sqrt(3) / 4;

        result.AddRange(GeneratePositionsFirstRow(agentsInEvenRow, xOffset));
        int row = 1;
        while (result.Count < settings.agentNumber)
        {
            float y = row * yOffset;
            if (row % 2 == 0)
            {
                result.AddRange(GeneratePositionsEvenRow(agentsInEvenRow, xOffset, y));
            }
            else
            {
                result.AddRange(GeneratePositionsOddRow(agentsInEvenRow, xOffset, y));
            }

            ++row;
        }

        Debug.Log("Number of positions calculated: " + result.Count);
        return result;
    }

    private List<Vector2> GeneratePositionsEvenRow(int agentsInRow, float xOffset, float y)
    {
        List<Vector2> result = new() { new(0, y), new(0, -y) };

        for (int i = 0; i < agentsInRow - 1; i+=2)
        {
            float x = (i/2f + 1f) * xOffset;
            result.Add(new(x, y));
            result.Add(new(-x, y));
            result.Add(new(x, -y));
            result.Add(new(-x, -y));
        }
        return result;
    }

    private List<Vector2> GeneratePositionsOddRow(int agentsInRow, float xOffset, float y)
    {
        List<Vector2> result = new();
        for (int i = 0; i < agentsInRow; i+=2)
        {
            float x = (0.5f + i/2f) * xOffset;
            result.Add(new(x, y));
            result.Add(new(-x, y));
            result.Add(new(x, -y));
            result.Add(new(-x, -y));
        }
        return result;
    }

    private List<Vector2> GeneratePositionsFirstRow(int agentsInRow, float xOffset)
    {
        float y = 0f;
        List<Vector2> result = new()
        {
            new(0f, y)
        };

        for (int i = 0; i < agentsInRow - 1; i+=2)
        {
            float x = (i/2f + 1f) * xOffset;
            result.Add(new(x, y));
            result.Add(new(-x, y));
        }
        return result;
    }
}
