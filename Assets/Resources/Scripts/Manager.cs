using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager : MonoBehaviour
{
    public int agentNumber = 100;
    public float spawnRadius = 20f;

    List<Agent> agents;
    public GameObject predatorPrefab;
    public Transform predator;

    [SerializeField]
    private GameObject agentPrefab;
    private AgentSettings settings;

    private void Start()
    {
        agents = Spawn();
        settings = agents[0].settings;
    }

    private void FixedUpdate()
    {
        Calculate();
        AgentUpdate(Time.fixedDeltaTime);
    }
    
    private void Update() {
        if (Input.GetKeyDown(KeyCode.S) && !predator)
        {
            SpawnPredator();
        }
        if (Input.GetKeyDown(KeyCode.D) && predator)
        {
            DespawnPredator();
        }
    }

    public List<Agent> Spawn()
    {
        List<Agent> localAgents = new List<Agent>();
        for (int i = 0; i < agentNumber; ++i)
        {
            float x = Random.Range(-spawnRadius, spawnRadius);
            float y = Random.Range(-spawnRadius, spawnRadius);
            Vector2 position = new Vector3(x, y);

            localAgents.Add(GameObject.Instantiate(agentPrefab, position, Quaternion.identity, transform).GetComponent<Agent>());
            if (predator)
            {
                localAgents[i].predator = predator;
            }
        }

        return localAgents;
    }

    private void Calculate()
    {
        float detectionRadius = settings.flockmateDetectionRadius;
        float avoidanceRadius = settings.flockmateAvoidanceRadius;

        for (int i = 0; i < agentNumber; ++i)
        {
            var agent = agents[i];

            agent.ResetAccumulators();
            agent.numFlockmates = 0;

            for (int j = 0; j < agentNumber; j++)
            {
                var neighbor = agents[j];
                if (i == j)
                {
                    continue;
                }

                Vector3 offset = neighbor.transform.position - agent.transform.position;
                float sqrDist = Vector3.SqrMagnitude(offset);

                if (sqrDist <= detectionRadius * detectionRadius && Vector3.Angle(agent.GetDirection(), offset) < settings.sightAngle)
                {
                    Color color = new Color(0f, 255f, 0f);
                    ++agent.numFlockmates;
                    agent.averageFlockmateVelocity += neighbor.GetDirection();
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
        foreach (var agent in agents)
        {
            agent.AgentUpdate(dt);
        }
    }

    private void SpawnPredator()
    {
        Vector3 targetPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        targetPosition.z = 0;

        predator = GameObject.Instantiate(predatorPrefab, targetPosition, Quaternion.identity).transform;
        foreach(var agent in agents)
        {
            agent.predator = predator;
        }
    }

    private void DespawnPredator()
    {
        Destroy(predator.gameObject);
    }
}
