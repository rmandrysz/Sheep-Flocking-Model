using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class AngleCalculator
{
    public static List<float> angles;
    public static float collisionAvoidAngleIncrement = 6f;
    public static float collisionAvoidMaxAngle = 120f;

    static AngleCalculator()
    {
        angles = new List<float>();
        
        for (float angle = collisionAvoidAngleIncrement; angle < collisionAvoidMaxAngle; angle += collisionAvoidAngleIncrement)
        {
            angles.Add(angle);
            angles.Add(-angle);
        }
    }
}

