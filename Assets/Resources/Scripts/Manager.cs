using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class Manager : MonoBehaviour
{
    public int agentNumber = 100;
    public float spawnRadius = 20f;

    List<Agent> agents;
    public GameObject predatorPrefab;
    public Predator predator;

    [SerializeField]
    private GameObject agentPrefab;
    private AgentSettings settings;

    [SerializeField]
    private bool saveToFile;
    private List<Vector3> data;

    private void Start()
    {
        data = new List<Vector3>();
        agents = Spawn();
        settings = agents[0].settings;
    }

    private void FixedUpdate()
    {
        Calculate();
        AgentUpdate(Time.fixedDeltaTime);
        if (predator)
        {
            predator.PredatorUpdate(Time.fixedDeltaTime);
        }
    }
    
    private void Update() {
        if (Input.GetKeyDown(KeyCode.S) && !predator)
        {
            SpawnPredator();
        }
        if (Input.GetKeyDown(KeyCode.D) && predator)
        {
            if(Input.GetKeyDown(KeyCode.D))
            {
                DespawnPredator();
            }
        }
    }

    public List<Agent> Spawn()
    {
        List<Agent> localAgents = new List<Agent>();
        for (int i = 0; i < agentNumber; ++i)
        {
            float x = Random.Range(-spawnRadius, spawnRadius);
            float y = Random.Range(-spawnRadius, spawnRadius);
            Vector2 position = new Vector2(x, y);

            localAgents.Add(
                GameObject.Instantiate(
                    agentPrefab, position, Quaternion.identity, transform).GetComponent<Agent>());
                    
            if (predator)
            {
                localAgents[i].predator = predator.transform;
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

            if (predator && agent.averageFlockCenter != Vector3.zero)
            {
                float predatorDistance = (predator.transform.position - agent.transform.position).magnitude;
                float centerDistance = ((agent.averageFlockCenter / agent.numFlockmates) - agent.transform.position).magnitude;
                Vector4 state = new Vector3(agent.previousDirection.magnitude, predatorDistance, centerDistance);
                data.Add(state);
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

        predator = GameObject.Instantiate(predatorPrefab, targetPosition, Quaternion.identity).GetComponent<Predator>();
        foreach(var agent in agents)
        {
            agent.predator = predator.transform;
        }
    }

    private void DespawnPredator()
    {
        Destroy(predator.gameObject);
    }

    private void OnApplicationQuit() {
        if(saveToFile)
        {
            SaveData();
        }
    }

    private void SaveData()
    {
        int dataNumber = 0;
        string path = Application.dataPath + "/Data/Data" + dataNumber + ".txt";

        while(File.Exists(path))
        {
            ++dataNumber;
            path = Application.dataPath + "/Data/Data" + dataNumber + ".txt";
        }

        foreach ( Vector3 point in data )
        {
            string pointData = (
            point.x 
            + "\t" + point.y 
            + "\t" + point.z
            + "\n"
            );
            File.AppendAllText(path, pointData);
        } 
    }
}
