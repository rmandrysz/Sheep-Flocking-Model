using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataCollector
{
    private List<Data> data = new();

    public void RecordFrameData(List<Agent> agents, Transform predator)
    {
        if (!predator)
        {
            return;
        }
        
        agents.ForEach(agent => RecordAgentData(agent, predator));
    }

    public void StoreDataInFile()
    {
        string filePath = GenerateDataFilePath();

        data.ForEach(point =>
        {
            string pointData = point.predatorCenterDistance
                      + "\t" + point.agentCenterDistance
                      + "\n";
            File.AppendAllText(filePath, pointData);
        });
    }

    private void RecordAgentData(Agent agent, Transform predator)
    {
        Data dataPoint;
        dataPoint.predatorCenterDistance = (predator.transform.position - (agent.averageFlockCenter / agent.numFlockmates)).magnitude;
        dataPoint.agentCenterDistance = ((agent.averageFlockCenter / agent.numFlockmates) - agent.transform.position).magnitude;
        data.Add(dataPoint);
    }

    private string GenerateDataFilePath()
    {
        int dataNumber = 0;
        string path = Application.dataPath + "/Resources/Data/Files/Data" + dataNumber + ".txt";

        while(File.Exists(path))
        {
            ++dataNumber;
            path = Application.dataPath + "/Resources/Data/Files/Data" + dataNumber + ".txt";
        }

        return path;
    }

    private struct Data
    {
        public float predatorCenterDistance;
        public float agentCenterDistance;
    }
}
