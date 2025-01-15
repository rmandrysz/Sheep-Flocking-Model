using System;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections;
using System.Collections.Generic;

public static class SvgScreenshotRecorder
{
    private static int ScreenshotsInBatchNumber = 6;
    private static int batchCounter = 0;

    public static IEnumerator CaptureScreenchotBatch(float delay, List<Agent> agents, Transform predator = null)
    {
        int screenshotCounter = 0;
        while(screenshotCounter < ScreenshotsInBatchNumber)
        {
            string path = GenerateFilePath(screenshotCounter);
            CaptureScreenshot(path, agents, predator);

            ++screenshotCounter;
            yield return new WaitForSeconds(delay);
        }
        ++batchCounter;
    }

    private static void CaptureScreenshot(string filePath, List<Agent> agents, Transform predator)
    {  
        agents.ForEach(agent => StorePositionInFile(filePath, agent.transform));

        if (predator)
        {
            StorePositionInFile(filePath, predator);
        }
    }

    private static void StorePositionInFile(string filePath, Transform transform)
    {
        Vector3 predatorPosition = transform.position;
        Vector3 angle = transform.rotation.eulerAngles;

        string dataToSave = predatorPosition.x + "\t" +
                                predatorPosition.y + "\t" +
                                angle.z + "\n";
        File.AppendAllText(filePath, dataToSave);
    }

    private static string GenerateFilePath(int screenshotNumber)
    {
        return string.Format("{0}/Resources/Data/SVGScreenshots/Coordinates/screen_{1}_{2}.txt", 
                                Application.dataPath,
                                batchCounter, 
                                screenshotNumber);
    }
}