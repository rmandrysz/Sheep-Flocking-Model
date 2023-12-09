using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEditor;

public class Manager : MonoBehaviour
{
    [Header("Spawning")]
    public int agentNumber = 100;
    public float spawnRadius = 20f;

    [Header("References")]
    public GameObject predatorPrefab;
    public Predator predator;
    private List<Agent> agents;

    [SerializeField]
    private GameObject agentPrefab;
    private AgentSettings settings;

    public Transform playground;
    public List<Transform> obstacles;

    [Header("Data collecting")]
    [SerializeField]
    private bool saveToFile;
    private List<Vector3> data;

    public bool listenForScreenshots = false;
    public float capturePeriod = 1f;
    private int screenshotCount = 0;
    private int screenshotBatchCounter = 0;

    [Header("WallGeneration")]
    public float horizontalWallOffset;
    public float verticalWallOffset;
    public float wallSegmentSize;
    public GameObject wallSegmentPrefab;

    [Header("Controlled test environment")]
    public bool randomSpawnEnabled = true;
    public int agentsInEvenRow = 12;
    public int agentsInOddRow = 11;
    public float maxSpawnX = 20f;
    public float maxSpawnY = 20f;
    public float spawnGapY = 10f;
    public float spawnGapX = 5f;
    public int test = 0;

    private int skipFrame = 0;

    private void Awake()
    {
        Application.targetFrameRate = 60;
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
        screenshotBatchCounter = 0;

        if (!settings.manualPredatorControl)
        {
            SpawnPredator();
        }
    }

    private void FixedUpdate()
    {
        Calculate();
        AgentUpdate(Time.fixedDeltaTime);
        if (predator)
        {
            predator.PredatorUpdate(Time.fixedDeltaTime);
        }
        DebugPrintSumOfPositions();
    }
    
    private void Update() {
        Camera cam = Camera.main;
        if (!settings.manualPredatorControl)
        {
            Vector3 predatorFinalTarget = cam.ScreenToWorldPoint(new(cam.pixelWidth, cam.pixelHeight));
            predatorFinalTarget.z = 0;
            if (predator.transform.position == predatorFinalTarget)
            {
                Quit();
            }
        }
        if (Input.GetKeyDown(KeyCode.S) && !predator)
        {
            SpawnPredator();
        }
        if (Input.GetKeyDown(KeyCode.D) && predator && settings.manualPredatorControl)
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
        if(listenForScreenshots && Input.GetKeyDown(KeyCode.K))
        {
            StartCoroutine(CaptureSVGPeriodically());
        }
    }

    private List<Agent> SpawnRandom()
    {
        List<Agent> spawnedAgents = new();
        for (int i = 0; i < agentNumber; ++i)
        {
            float x = Random.Range(-spawnRadius, spawnRadius);
            float y = Random.Range(-spawnRadius, spawnRadius);
            Vector2 position = new(x, y);
            Quaternion rotation = Quaternion.Euler(0f, 0f, Random.Range(-180f, 180f));

            spawnedAgents.Add(
                GameObject.Instantiate(
                    agentPrefab, position, rotation, transform).GetComponent<Agent>());
                    
            if (predator)
            {
                spawnedAgents[i].predator = predator.transform;
            }
        }

        return spawnedAgents;
    }

    private List<Vector2> CalculateSpawnPositions()
    {
        List<Vector2> result = new();
        int agentsLeftToSpawn = agentNumber;
        int row = 0;
        int numberOfRows = 2 * agentNumber / (agentsInEvenRow + agentsInOddRow);

        while (agentsLeftToSpawn > 0)
        {
            bool even = (row % 2) == 0;
            var xAmount = even ? agentsInEvenRow : agentsInOddRow;
            float y = transform.position.y + maxSpawnY - (row * spawnGapY);

            for( int j = 0; j < xAmount && agentsLeftToSpawn != 0; ++j, --agentsLeftToSpawn)
            {
                var x = transform.position.x - maxSpawnX + (j * spawnGapX);
                if (!even)
                {
                    x += spawnGapX / 2;
                }
                result.Add( new(x, y));
            }
            ++row;
        }
        return result;
    } 

    private List<Agent> SpawnNonRandom()
    {
        var positions = CalculateSpawnPositions();
        List<Agent> spawnedAgents = new();
        foreach(var pos in positions)
        {
            spawnedAgents.Add(
                Instantiate(
                    agentPrefab, pos, Quaternion.identity, transform).GetComponent<Agent>());
        }
        return spawnedAgents;
    }

    public List<Agent> Spawn()
    {
        if( randomSpawnEnabled )
        {
            return SpawnRandom();
        }
        return SpawnNonRandom();
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
        var cam = Camera.main;
        Vector3 spawnPosition = cam.ScreenToWorldPoint(Input.mousePosition);
        spawnPosition.z = 0;
        Vector3 targetPosition = spawnPosition;
        
        if (!settings.manualPredatorControl)
        {
            spawnPosition = cam.ScreenToWorldPoint(new(0, 0));
            spawnPosition.z = 0;
            targetPosition = cam.ScreenToWorldPoint(new(cam.pixelWidth, cam.pixelHeight));
            targetPosition.z = 0;
        }

        predator = Instantiate(predatorPrefab, spawnPosition, Quaternion.identity).GetComponent<Predator>();
        predator.manualControl = settings.manualPredatorControl;
        predator.targetPosition = targetPosition;

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
        string path = Application.dataPath + "/Resources/Data/Files/Data" + dataNumber + ".txt";

        while(File.Exists(path))
        {
            ++dataNumber;
            path = Application.dataPath + "/Resources/Data/Files/Data" + dataNumber + ".txt";
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

    private void SaveDataForSvg(int nameNumber)
    {
        string path = string.Format("{0}/Resources/Data/SVGScreenshots/Coordinates/screen_{1}_{2}.txt", 
                            Application.dataPath,
                            screenshotBatchCounter, 
                            nameNumber);
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
    }

    private void SpawnWalls()
    {
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

    private IEnumerator CaptureSVGPeriodically()
    {
        screenshotCount = 6;
        while(screenshotCount > 0){
            --screenshotCount;
            Debug.Log("Waited " + screenshotCount);
            SaveDataForSvg(screenshotCount);
            yield return new WaitForSeconds(capturePeriod);
        }
        ++screenshotBatchCounter;
    }

    private void Quit()
    {
        EditorApplication.isPlaying = false;
    }

    private void DebugPrintSumOfPositions()
    {
        test += 1;
        if (test == 1000)
        {
            float xSum = 0f;
            foreach (var agent in agents)
            {
                xSum += agent.transform.position.x;
            }
            Debug.Log("Sum of X: " + xSum + "\n");
            test = 0;
        }
    }
}
