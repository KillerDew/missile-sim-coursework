using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct WindData
{
    [Range(0, 50e3f)]
    public float referenceAltitude;
    [Range(0, 360f)]
    public float angle;
    public float speed;
}
public class Wind : MonoBehaviour
{
    public List<WindData> windLayers = new List<WindData>();

    public Vector3 getWindAtAltitude(float altitude)
    {
        if (windLayers.Count == 0) return Vector3.zero;

        WindData lowerLayer = windLayers[0];
        WindData upperLayer = windLayers[windLayers.Count - 1];

        foreach (WindData layer in windLayers)
        {
            if (layer.referenceAltitude <= altitude)
            {
                lowerLayer = layer;
            }
            if (layer.referenceAltitude >= altitude)
            {
                upperLayer = layer;
                break;
            }
        }

        if (lowerLayer.referenceAltitude == upperLayer.referenceAltitude)
        {
            return upperLayer.speed * (Quaternion.AngleAxis(upperLayer.angle, Vector3.up) * Vector3.forward).normalized;
        }
        else
        {
            float t = (altitude - lowerLayer.referenceAltitude) / (upperLayer.referenceAltitude - lowerLayer.referenceAltitude);
            float speed = Mathf.Lerp(lowerLayer.speed, upperLayer.speed, t);

            Vector3 lowerDir = Quaternion.AngleAxis(lowerLayer.angle, Vector3.up) * Vector3.forward;
            Vector3 upperDir = Quaternion.AngleAxis(upperLayer.angle, Vector3.up) * Vector3.forward;
            Vector3 direction = Vector3.Slerp(lowerDir.normalized, upperDir.normalized, t);
            return speed * direction.normalized;
        }
    }
}
