using System;
using UnityEngine;

[CreateAssetMenu(fileName = "AirfoilConfig", menuName = "Scriptable Objects/AirfoilConfig")]
public class AirfoilConfig : ScriptableObject
{
    [Header("Dimensional Properties")]
    [Range(0, 1)]
    public float flapFraction;

    [Header("Aerodynamic Behaviour")]
    public float skinDrag = 0.02f;
    public float zeroLiftAoa_deg;

    [HideInInspector]
    public float zeroLiftAoa
    {
        get
        {
            return zeroLiftAoa_deg * Mathf.Deg2Rad;
        }
    }
    public float liftSlope = 2f * Mathf.PI;
    public float stallHigh_deg;
    public float stallLow_deg;

    [HideInInspector]
    public float stallAngleHigh
    {
        get
        {
            return stallHigh_deg * Mathf.Deg2Rad;
        }
    }
    [HideInInspector]
    public float stallAngleLow
    {
        get
        {
            return stallLow_deg * Mathf.Deg2Rad;
        }
    }

    public static bool operator ==(AirfoilConfig a, AirfoilConfig b)
    {
        return a.skinDrag == b.skinDrag && a.flapFraction == b.flapFraction && a.zeroLiftAoa == b.zeroLiftAoa && a.stallAngleHigh == b.stallAngleHigh && a.stallAngleLow == b.stallAngleLow && a.liftSlope == b.liftSlope;
    }
    public static bool operator !=(AirfoilConfig a, AirfoilConfig b)
    {
        return !(a == b);
    }

    public override bool Equals(object a)
    {
        return a is AirfoilConfig && (AirfoilConfig)a == this;
    }
    public override int GetHashCode()
    {
        return this.zeroLiftAoa.GetHashCode() ^ this.skinDrag.GetHashCode() ^ this.flapFraction.GetHashCode() ^ this.stallAngleHigh.GetHashCode() ^ this.stallAngleLow.GetHashCode() ^ this.liftSlope.GetHashCode();
    }
}
