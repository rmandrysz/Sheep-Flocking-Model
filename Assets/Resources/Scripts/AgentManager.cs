using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentManager
{
    private static GameObject agentContainer;

    private readonly AgentSettings settings;
    private GameObject prefab;

    private List<Agent> agents = new();

    public AgentManager(AgentSettings settings, GameObject agentContainer, GameObject prefab)
    {
        AgentManager.agentContainer = agentContainer;

        this.settings = settings;
        this.prefab = prefab;
    }

    public void StartAgents()
    {
        if( settings.enableRandomSpawn )
        {
            SpawnRandom();
            return;
        }
        SpawnNonRandom();
    }

    public void UpdateAgents(float dt, List<Transform> obstacles)
    {
        Calculate(obstacles);
        AgentUpdate(Time.fixedDeltaTime);
    }

    private void Calculate(List<Transform> obstacles)
    {
        float detectionRadius = settings.flockmateDetectionRadius;
        float avoidanceRadius = settings.flockmateAvoidanceRadius;

        for (int i = 0; i < settings.agentNumber; ++i)
        {
            var agent = agents[i];

            agent.ResetAccumulators();
            agent.numFlockmates = 0;

            foreach(var obstacle in obstacles)
            {
                Vector3 offset = agent.transform.position - obstacle.position;
                agent.AddObstacleAvoidance(offset);
            }

            for (int j = 0; j < settings.agentNumber; j++)
            {
                var neighbor = agents[j];
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
                    ++agent.numFlockmates;
                    agent.averageFlockmateVelocity += neighbor.direction;
                    agent.averageFlockCenter += neighbor.transform.position;

                    if (sqrDist <= avoidanceRadius * avoidanceRadius)
                    {
                        agent.AddFlockmateAvoidance(offset);
                        color = new Color(255f, 0f, 0f,  10f * Vector3.SqrMagnitude(offset / sqrDist));
                    }
                    if (agents[i].debug)
                    {
                        Debug.DrawRay(agent.transform.position, offset, color);
                    }
                }
            }
        }
    }

    private void AgentUpdate(float dt)
    {
        agents.ForEach(agent => agent.AgentUpdate(dt));
    }

    public void AttachPredator(Predator predator)
    {
        // TODO: Make predator a reference in the agent script so you don't have to iterate
        // and put it like that manually
        // OR: pass predator as argument to update everytime
        agents.ForEach(agent => agent.predator = predator.transform);
    }

    private void SpawnRandom()
    {
        for (int i = 0; i < settings.agentNumber; ++i)
        {
            float x = Random.Range(-settings.spawnRadius, settings.spawnRadius);
            float y = Random.Range(-settings.spawnRadius, settings.spawnRadius);
            Vector2 position = new(x, y);
            Quaternion rotation = Quaternion.Euler(0f, 0f, Random.Range(-180f, 180f));

            Agent agent = GameObject.Instantiate(
                    prefab, position, rotation, agentContainer.transform).GetComponent<Agent>();

            agents.Add(agent);
        }
    }

    private void SpawnNonRandom()
    {
        var positions = CalculateSpawnPositions();
        foreach(var pos in positions)
        {
            agents.Add(
                GameObject.Instantiate(
                    prefab, pos, Quaternion.identity, agentContainer.transform).GetComponent<Agent>());
        }
    }

    private List<Vector2> CalculateSpawnPositions()
    {
        const int agentsInEvenRow = 12;
        const int agentsInOddRow = 11;
        const float maxSpawnX = 20f;
        const float maxSpawnY = 20f;
        const float spawnGap = 5f;

        List<Vector2> result = new();
        int agentsLeftToSpawn = settings.agentNumber;
        int row = 0;
        int numberOfRows = 2 * settings.agentNumber / (agentsInEvenRow + agentsInOddRow);
        float heightOffset = spawnGap * Mathf.Sqrt(3) / 4;

        while (agentsLeftToSpawn > 0)
        {
            bool even = (row % 2) == 0;
            var xAmount = even ? agentsInEvenRow : agentsInOddRow;
            float y = agentContainer.transform.position.y + maxSpawnY - (row * heightOffset);

            for( int j = 0; j < xAmount && agentsLeftToSpawn != 0; ++j, --agentsLeftToSpawn)
            {
                var x = agentContainer.transform.position.x - maxSpawnX + (j * spawnGap);
                if (!even)
                {
                    x += spawnGap / 2;
                }
                result.Add( new(x, y));
            }
            ++row;
        }
        return result;
    }
}
