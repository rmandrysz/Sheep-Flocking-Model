using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimulationController : MonoBehaviour
{
    private AgentManager agentManager;
    private DataCollector dataCollector;
    private PlaygroundManager playgroundManager;
    private Predator predator;
    
    [SerializeField] private readonly PrefabList prefabs;
    [SerializeField] private readonly PlaygroundSettings playgroundSettings;
    [SerializeField] private readonly AgentSettings agentSettings;
    [SerializeField] private readonly GameObject playgroundObject;
    [SerializeField] private readonly GameObject agentContainerObject;

    private void Awake() 
    {
        agentManager = new(agentSettings, agentContainerObject, prefabs.agentPrefab);
        dataCollector = new();
        playgroundManager = new(playgroundObject, prefabs.wallSegmentPrefab, playgroundSettings);
        // predator = new();
    }

    private void Start() 
    {
        playgroundManager.StartPlayground();
        agentManager.StartAgents();
        // predator.StartPredator();
    }

    private void FixedUpdate() 
    {
        agentManager.UpdateAgents(Time.fixedDeltaTime, playgroundManager.walls);
        // predator.UpdatePredator(Time.fixedDeltaTime);
    }
}
