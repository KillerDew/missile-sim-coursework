using System;
using UnityEngine;

[CreateAssetMenu(fileName = "AirfoilConfig", menuName = "Scriptable Objects/AirfoilConfig")]
public class AirfoilConfig : ScriptableObject
{
    [Header("Dimensional Properties")]
    [Range(0, 1)]
    // Flap fraction is fraction of chord that is flap.
    public float flapFraction;

    [Header("Aerodynamic Behaviour")]
    // Skin drag is the drag coefficient at 0 angle of attack.
    public float skinDrag = 0.02f;
    /**
    * Zero lift angle of attack (deg) is angle at which lift is zero.
    * e.g. 0 for symmetric airfoils.
    * Public for editing in Unity inspector. (and other scripts)
    */
    public float zeroLiftAoa_deg;

    [HideInInspector]
    /**
    * Hidden for editing in Unity inspector, but public for other scripts to read.
    * returns in radians, as used in calculations.
    */
    public float zeroLiftAoa
    {
        get
        {
            return zeroLiftAoa_deg * Mathf.Deg2Rad;
        }
    }
    // Lift per unit angle of attack in linear region. Units: rad^-1.
    public float liftSlope = 2f * Mathf.PI;
    
    /**
    * Stall angle high and low (deg) are angles at which stall occurs
    * For symmetric airfoils, stallHigh_deg = -stallLow_deg.
    * For cambered airfoils, they can be different.
    * Public for editing in Unity inspector. (and other scripts)
    */
    public float stallHigh_deg;
    public float stallLow_deg;

    // Stall angle high and low (rad). Hidden for editing.
    // Read-only for other scripts.(Radians for calculations)

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
    

    /**
    * Checks if two AirfoilConfigs are equal by comparing their properties. (and reverse for !=)
    */
    public static bool operator ==(AirfoilConfig a, AirfoilConfig b)
    {
        return a.skinDrag == b.skinDrag && a.flapFraction == b.flapFraction && a.zeroLiftAoa == b.zeroLiftAoa && a.stallAngleHigh == b.stallAngleHigh && a.stallAngleLow == b.stallAngleLow && a.liftSlope == b.liftSlope;
    }
    public static bool operator !=(AirfoilConfig a, AirfoilConfig b)
    {
        return !(a == b);
    }
    
    /**
    * Overrides Equals and GetHashCode so Unity can evaluate == and !=.
    * This is important for using AirfoilConfig in collections like HashSet or Dictionary.
    * As well as for Unity functions that rely on Equals().
    */
    public override bool Equals(object a)
    {
        return a is AirfoilConfig && (AirfoilConfig)a == this;
    }
    // Generates a unique hash code with all properties' hash codes.
    public override int GetHashCode()
    {
        return this.zeroLiftAoa.GetHashCode() ^ this.skinDrag.GetHashCode() ^ this.flapFraction.GetHashCode() ^ this.stallAngleHigh.GetHashCode() ^ this.stallAngleLow.GetHashCode() ^ this.liftSlope.GetHashCode();
    }
}
