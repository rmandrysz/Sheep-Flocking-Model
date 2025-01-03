using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class SimulationController : MonoBehaviour
{
    private AgentManager agentManager;
    private readonly DataCollector dataCollector = new();
    private PlaygroundManager playgroundManager;
    private Predator predator = null;
    
    [SerializeField] private PrefabList prefabs;
    [SerializeField] private SimulationSettings simulationSettings;
    [SerializeField] private AgentSettings agentSettings;
    [SerializeField] private GameObject playgroundObject;
    [SerializeField] private GameObject agentContainerObject;

    private void Awake() 
    {
        Application.targetFrameRate = 60;
        agentManager = new(agentSettings, agentContainerObject, prefabs.agentPrefab);
        playgroundManager = new(playgroundObject, prefabs.wallSegmentPrefab, simulationSettings);
    }

    private void Start() 
    {
        playgroundManager.StartPlayground();
        agentManager.StartAgents(simulationSettings);
        if (!simulationSettings.manualPredatorSpawn)
        {
            StartPredator();
        }
    }

    private void FixedUpdate() 
    {
        Transform predatorTransform = predator ? predator.transform : null;
        Vector3 centerOfMass = agentManager.UpdateAgents(Time.fixedDeltaTime, playgroundManager.walls, predatorTransform);

        if (predator)
        {
            predator.UpdatePredator(Time.fixedDeltaTime);
            if (!simulationSettings.manualPredatorControl && predator.ReachedTargetPosition())
            {
                EndSimulation();
            }
        }

        if (simulationSettings.saveDataToFile)
        {
            dataCollector.RecordFrameData(agentManager.Agents, predatorTransform, centerOfMass);
        }
    }

    private void Update() 
    {
        if(Input.GetKeyDown(KeyCode.P))
        {
            StartCoroutine(SvgScreenshotRecorder.CaptureScreenchotBatch(1f, agentManager.Agents, predator.transform));
        }

        if(simulationSettings.manualPredatorSpawn && Input.GetKeyDown(KeyCode.S))
        {
            StartPredator();
        }
    }

    private void StartPredator()
    {
        if(!predator)
        {
            predator = Predator.Spawn(prefabs.predatorPrefab, simulationSettings);
        }
    }

    private void EndSimulation()
    {
        if (simulationSettings.saveDataToFile)
        {
            dataCollector.StoreDataInFile();
        }
        EditorApplication.isPlaying = false;
    }
}
