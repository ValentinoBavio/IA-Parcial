using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class VectorExtensions
{ 
    public static Vector3 WithNewY(this Vector3 vector, float newY)
    {
        return new Vector3(vector.x, newY, vector.z);
    }
}
