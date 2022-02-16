using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class AngleCalculator
{
    public static List<float> angles;
    public static List<float> detectionAngles;
    public static float collisionAvoidAngleIncrement = 6f;
    public static float collisionAvoidMaxAngle = 140f;
    public static float collisionDetectionAngleIncrement = 30f;

    static AngleCalculator()
    {
        angles = new List<float>();
        detectionAngles = new List<float>();
        
        for (float angle = 15f; angle < collisionAvoidMaxAngle; angle += collisionAvoidAngleIncrement)
        {
            angles.Add(angle);
            angles.Add(-angle);
        }

        for (float angle = collisionDetectionAngleIncrement; angle <= 90f; angle += collisionDetectionAngleIncrement)
        {
            detectionAngles.Add(angle);
            detectionAngles.Add(-angle);
        }
    }
}

