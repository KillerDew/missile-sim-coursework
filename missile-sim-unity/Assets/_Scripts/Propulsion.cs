using System;
using UnityEngine;

public class PropulsionSource
{
    public Transform source;
    public Vector3 direction;
    public float thrust;
    public virtual offsetForce getThrustForce(Transform parent)
    {
        Vector3 force = parent.InverseTransformDirection(direction.normalized) * thrust;
        return new offsetForce(force, source.position);
    }
    public PropulsionSource(Transform source, Vector3 direction, float thrust)
    {
        this.source = source;
        this.direction = direction;
        this.thrust = thrust;
    }
}
[Serializable]
public class SolidRocketMotor : PropulsionSource
{
    private new float thrust = 0;
    public AnimationCurve thrustCurve;
    public float burnTime;

    public bool ignition { get; private set; } = false;
    float ignitionTime;

    public SolidRocketMotor(Transform source, Vector3 direction, float thrust) : base(source, direction, thrust) { }


    public void ignite()
    {
        if (ignition) return;
        ignition = true;
        ignitionTime = Time.time;
    }
    public override offsetForce getThrustForce(Transform parent)
    {
        float time = Time.time - ignitionTime;
        if (time > burnTime) return new offsetForce(Vector3.zero, source.position);

        float curveThrust = thrustCurve.Evaluate(time) ;
        Vector3 force = parent.TransformDirection(direction.normalized) * curveThrust;
        return new offsetForce(force, source.position);
    }
}