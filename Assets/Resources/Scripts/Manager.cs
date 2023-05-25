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

    public Transform playground;
    public List<Transform> obstacles;

    [SerializeField]
    private bool saveToFile;
    private List<Vector3> data;

    public bool listenForScreenshots = false;
    private int screenshotCount = 0;

    [Header("WallGeneration")]
    public float horizontalWallOffset;
    public float verticalWallOffset;
    public float wallSegmentSize;
    public GameObject wallSegmentPrefab;

    private int skipFrame = 0;

    private void Awake()
    {
        // Application.targetFrameRate = 60;
        agents = Spawn();
    }

    private void Start()
    {
        data = new List<Vector3>();
        obstacles = new List<Transform>();
        settings = agents[0].settings;
        foreach(Transform wall in playground)
        {
            obstacles.Add(wall);
        }
        SpawnWalls();
    }

    private void FixedUpdate()
    {
        Calculate();
        AgentUpdate(Time.fixedDeltaTime);
        if(listenForScreenshots)
        {
            SaveDataForSvg();
        }
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
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            Debug.Log("Speed up to 0.5");
            Time.timeScale = 0.5f;
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            Debug.Log("Speed up to 2");
            Time.timeScale = 2f;
        }
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Debug.Log("Speed up to 1");
            Time.timeScale = 1f;
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            Debug.Log("Speed up to 3");
            Time.timeScale = 3f;
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            Debug.Log("Speed up to 4");
            Time.timeScale = 4f;
        }
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            Debug.Log("Speed up to 5");
            Time.timeScale = 5f;
        }
        if(Input.GetKeyDown(KeyCode.K))
        {
            Debug.Log("Here");
            screenshotCount = 6;
        }
    }

    public List<Agent> Spawn()
    {
        List<Agent> localAgents = new();
        for (int i = 0; i < agentNumber; ++i)
        {
            float x = Random.Range(-spawnRadius, spawnRadius);
            float y = Random.Range(-spawnRadius, spawnRadius);
            Vector2 position = new(x, y);
            Quaternion rotation = Quaternion.Euler(0f, 0f, Random.Range(-180f, 180f));

            localAgents.Add(
                GameObject.Instantiate(
                    agentPrefab, position, rotation, transform).GetComponent<Agent>());
                    
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

            foreach(var obstacle in obstacles)
            {
                Vector3 offset = agent.transform.position - obstacle.position;
                agent.AddObstacleAvoidance(offset);
            }

            for (int j = 0; j < agentNumber; j++)
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
                        // Debug.DrawRay(agent.transform.position, offset, Color.magenta);
                        Debug.DrawRay(agent.transform.position, agent.previousDirection.normalized * 3, Color.blue);
                    }
                    Color color = new(0f, 255f, 0f);
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

            if (saveToFile && predator && skipFrame != 0 && agent.averageFlockCenter != Vector3.zero)
            {
                float oldPredatorDistance = (predator.transform.position - agent.transform.position).magnitude;
                float centerDistance = ((agent.averageFlockCenter / agent.numFlockmates) - agent.transform.position).magnitude;
                float newPredatorDistance = (predator.transform.position - (agent.averageFlockCenter / agent.numFlockmates)).magnitude;
                Vector4 state = new Vector3(agent.previousDirection.magnitude, newPredatorDistance, centerDistance);
                data.Add(state);
            }
        }
        skipFrame = 1 - skipFrame;
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
        string path = Application.dataPath + "/Data/Files/Data" + dataNumber + ".txt";

        while(File.Exists(path))
        {
            ++dataNumber;
            path = Application.dataPath + "/Data/Files/Data" + dataNumber + ".txt";
        }

        foreach ( Vector3 point in data )
        {
            string pointData = point.x 
                    + "\t" + point.y 
                    + "\t" + point.z
                    + "\n"
            ;
            File.AppendAllText(path, pointData);
        } 
    }

    private void SaveDataForSvg()
    {
        if (screenshotCount <= 0 | skipFrame == 0)
        {
            return;
        }
        string path = string.Format("{0}/Data/SVGScreenshots/screen_{1}.txt", 
                            Application.dataPath, 
                            screenshotCount);
        foreach (var agent in agents)
        {
            Vector3 position = agent.transform.position;
            Vector3 angle = agent.transform.rotation.eulerAngles;

            string dataToSave = position.x + "\t" +
                                position.y + "\t" +
                                angle.z + "\n";
            File.AppendAllText(path, dataToSave);
        }
        if(predator)
        {
            Vector3 predatorPosition = predator.transform.position;
            Vector3 angle = predator.transform.rotation.eulerAngles;
            string dataToSave = predatorPosition.x + "\t" +
                                predatorPosition.y + "\t" +
                                angle.z + "\n";
            File.AppendAllText(path, dataToSave);
        }

        --screenshotCount;
    }

    private void SpawnWalls()
    {
        float halfSegmentSize = wallSegmentSize / 2;

        for( float i = -verticalWallOffset; i <= verticalWallOffset; i += wallSegmentSize )
        {
            Vector3 position1 = new(-horizontalWallOffset, i, 0f);
            Vector3 position2 = new(horizontalWallOffset, i, 0f);
            Transform segment1 = GameObject.Instantiate(wallSegmentPrefab, position1, Quaternion.identity, playground).transform;
            Transform segment2 = GameObject.Instantiate(wallSegmentPrefab, position2, Quaternion.identity, playground).transform;

            obstacles.Add(segment1);
            obstacles.Add(segment2);
        }
        for( float i = -horizontalWallOffset; i <= horizontalWallOffset; i += wallSegmentSize )
        {
            Vector3 position1 = new(i, -verticalWallOffset, 0f);
            Vector3 position2 = new(i, verticalWallOffset, 0f);
            Transform segment1 = GameObject.Instantiate(wallSegmentPrefab, position1, Quaternion.identity, playground).transform;
            Transform segment2 = GameObject.Instantiate(wallSegmentPrefab, position2, Quaternion.identity, playground).transform;

            obstacles.Add(segment1);
            obstacles.Add(segment2);
        }
    }

    private void OnDrawGizmos() {
        if(!predator)
        {
            return;
        }
        Gizmos.DrawWireSphere(predator.transform.position, settings.flightZoneRadius); 
    }

    public static float AngleInRad(Vector3 vec1, Vector3 vec2) {
        return Mathf.Atan2(vec1.y - vec2.y, vec1.x - vec2.x);
    }

    public static float AngleInDeg(Vector3 vec1, Vector3 vec2) {
        return AngleInRad(vec1, vec2) * 180 / Mathf.PI;
    }
}
