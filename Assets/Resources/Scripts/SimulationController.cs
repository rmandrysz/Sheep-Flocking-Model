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
        agentManager.StartAgents();
        predator = StartPredator();
    }

    private void FixedUpdate() 
    {
        agentManager.UpdateAgents(Time.fixedDeltaTime, playgroundManager.walls, predator.transform);
        predator.UpdatePredator(Time.fixedDeltaTime);

        if (!simulationSettings.manualPredatorControl && predator.ReachedTargetPosition())
        {
            EndSimulation();
        }

        if (simulationSettings.saveDataToFile)
        {
            dataCollector.RecordFrameData(agentManager.Agents, predator.transform);
        }
    }

    private void Update() 
    {
        if(Input.GetKeyDown(KeyCode.P))
        {
            StartCoroutine(SvgScreenshotRecorder.CaptureScreenchotBatch(1f, agentManager.Agents, predator.transform));
        }
    }

    private Predator StartPredator()
    {
        return Predator.Spawn(prefabs.predatorPrefab, simulationSettings.manualPredatorControl);
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
