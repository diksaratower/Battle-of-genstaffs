using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
public static class TransformExtend
{
    public static void LookAtAxis(this Transform transform, Vector3 target, bool useX = true, bool useY = true, bool useZ = true)
    {
        var lastRotation = transform.localEulerAngles;
        transform.LookAt(target);
        var newRotation = transform.localEulerAngles;
        SetUseAxis(transform, newRotation, lastRotation, useX, useY, useZ);
    }

    public static void LookAtAxis(this Transform transform, Transform target, bool useX = true, bool useY  = true, bool useZ = true)
    {
        var lastRotation = transform.localEulerAngles;
        transform.LookAt(target);
        var newRotation = transform.localEulerAngles;
        SetUseAxis(transform, newRotation, lastRotation, useX, useY, useZ);
    }

    public static void SetUseAxis(this Transform transform, Vector3 newRotation, Vector3 lastRotation, bool useX = true, bool useY = true, bool useZ = true)
    {
        if (!useX)
        {
            newRotation.x = lastRotation.x;
        }
        if (!useY)
        {
            newRotation.y = lastRotation.y;
        }
        if (!useZ)
        {
            newRotation.z = lastRotation.z;
        }
        transform.localEulerAngles = lastRotation;
        transform.localEulerAngles = newRotation;
    }
}

public static class Vector3Extend
{
    public static Vector3 GetMiddlePoint(Vector3 a, Vector3 b)
    {
        return a + ((b - a) / 2);
    }
}