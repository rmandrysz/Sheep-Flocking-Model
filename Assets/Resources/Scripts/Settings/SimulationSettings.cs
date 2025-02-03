using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class SimulationSettings : ScriptableObject
{
    public bool saveDataToFile = false;

    public bool manualPredatorSpawn = false;
    public bool manualPredatorControl = false;
    public bool forceSimulationFlow = false;
    public int dataCollectionDelay = 60;

    public float horizontalWallOffset;
    public float verticalWallOffset;
    public float wallSegmentSize;
}
