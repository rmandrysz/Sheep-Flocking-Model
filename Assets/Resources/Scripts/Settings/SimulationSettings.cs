using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class SimulationSettings : ScriptableObject
{
    public bool saveDataToFile = false;

    public bool manualPredatorControl = false;

    public float horizontalWallOffset;
    public float verticalWallOffset;
    public float wallSegmentSize;
}
