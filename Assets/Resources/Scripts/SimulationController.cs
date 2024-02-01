using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class SimulationController : MonoBehaviour
{
    private AgentManager agentManager;
    private DataCollector dataCollector;
    private PlaygroundManager playgroundManager;
    private Predator predator = null;
    
    [SerializeField] private PrefabList prefabs;
    [SerializeField] private PlaygroundSettings playgroundSettings;
    [SerializeField] private AgentSettings agentSettings;
    [SerializeField] private GameObject playgroundObject;
    [SerializeField] private GameObject agentContainerObject;

    private void Awake() 
    {
        Application.targetFrameRate = 60;
        agentManager = new(agentSettings, agentContainerObject, prefabs.agentPrefab);
        dataCollector = new();
        playgroundManager = new(playgroundObject, prefabs.wallSegmentPrefab, playgroundSettings);
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

        if (predator.transform.position == predator.targetPosition)
        {
            EndSimulation();
        }
    }

    private Predator StartPredator()
    {
        return Predator.Spawn(prefabs.predatorPrefab, playgroundSettings);
    }

    private void EndSimulation()
    {
        EditorApplication.isPlaying = false;
    }
}
