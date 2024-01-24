using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimulationController : MonoBehaviour
{
    private AgentManager agentManager;
    private DataCollector dataCollector;
    private PlaygroundManager playgroundManager;
    private Predator predator;
    
    [SerializeField] private PrefabList prefabs;
    [SerializeField] private PlaygroundSettings playgroundSettings;
    [SerializeField] AgentSettings agentSettings;
    [SerializeField] GameObject playgroundObject;

    private void Awake() {
        agentManager = new();
        dataCollector = new();
        playgroundManager = new(playgroundObject, prefabs.wallSegmentPrefab, playgroundSettings);
        // predator = new();
    }

    private void Start() {
        playgroundManager.StartPlayground();
        agentManager.StartAgents();
        // predator.StartPredator();
    }

    private void FixedUpdate() {
        agentManager.UpdateAgents(Time.fixedDeltaTime, playgroundManager.walls);
        // predator.UpdatePredator(Time.fixedDeltaTime);
    }
}
