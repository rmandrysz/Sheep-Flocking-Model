using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataCollector
{
    private List<Data> data = new();

    public void RecordFrameData(List<Agent> agents, Transform predator, Vector3 centerOfMass)
    {
        if (!predator)
        {
            return;
        }

        Data frameData = CalculateFrameData(agents, predator, centerOfMass);

        data.Add(frameData);
    }

    public void StoreDataInFile()
    {
        string filePath = GenerateDataFilePath();

        data.Sort((point1, point2) => point2.predatorCenterDistance.CompareTo(point1.predatorCenterDistance));
        
        data.ForEach(point =>
        {
            string pointData = point.predatorCenterDistance
                      + "; " + point.meanAgentCenterDistance
                      + "\n";
            File.AppendAllText(filePath, pointData);
        });
    }

    private float CalculateDistanceFromCenter(Transform transform, Vector3 centerOfMass)
    {
        Vector3 offset = transform.position - centerOfMass;
        return offset.magnitude;
    }

    private Data CalculateFrameData(List<Agent> agents, Transform predator, Vector3 centerOfMass)
    {
        Data frameData = new();
        float sumOfDistances = 0f;

        agents.ForEach(agent => sumOfDistances += CalculateDistanceFromCenter(agent.transform, centerOfMass));
        float meanAgentDistance = sumOfDistances / agents.Count;
        float predatorCenterDistance = CalculateDistanceFromCenter(predator, centerOfMass);

        frameData.predatorCenterDistance = predatorCenterDistance;
        frameData.meanAgentCenterDistance = meanAgentDistance;

        return frameData;
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
        public float meanAgentCenterDistance;
    }
}
