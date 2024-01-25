using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class PlaygroundSettings : ScriptableObject
{
    public bool manualPredatorControl = false;
    public float horizontalWallOffset;
    public float verticalWallOffset;
    public float wallSegmentSize;
}
