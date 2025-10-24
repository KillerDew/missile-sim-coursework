using JetBrains.Annotations;
using Unity.Mathematics;
using UnityEngine;

public class AeroSurface : MonoBehaviour
{
    [Header("Dimensions")]
    public float width = 1.0f;
    public float height = 1.0f;

    [Header("Properties")]
    [Range(0f, 1f)]
    public float flapFraction = 0.2f;
    public float flapAngle { get; private set; } = 0f;
    public float absMaxFlapAngle = Mathf.PI / 12f;

    public AirfoilConfig airfoilConfig;
    Rigidbody RB;

    public biVector3 calculateForces(Vector3 worldAirVelocity, float airdensity, Vector3 parentPosition)
    {
        biVector3 forceAndTorque = biVector3.zero;
        if (!gameObject.activeInHierarchy) return forceAndTorque;

        Vector3 relativePosition = transform.position - parentPosition;
        float aspectRatio = getAspectRatio();
        float area = getSurfaceArea();

        Vector3 airVelLocal = transform.InverseTransformDirection(worldAirVelocity);
        airVelLocal = new Vector3(airVelLocal.x, airVelLocal.y);
        Vector3 dragDir = transform.TransformDirection(airVelLocal.normalized).normalized;
        Vector3 liftDir = Vector3.Cross(dragDir, transform.forward).normalized;

        float dynamicPressure = 0.5f * airdensity * airVelLocal.sqrMagnitude;
        float alpha = Mathf.Atan2(airVelLocal.y, -airVelLocal.x);

        Vector3 aerodynamicCoefficients = Coefficients.getCoefficients(
            alpha,
            airfoilConfig,
            flapAngle,
            flapFraction,
            aspectRatio
        );
        //print(alpha * Mathf.Rad2Deg + " | " + aerodynamicCoefficients.y);

        Vector3 lift = liftDir * aerodynamicCoefficients.x * dynamicPressure * area;
        Vector3 drag = dragDir * aerodynamicCoefficients.y * dynamicPressure * area;
        Vector3 moment = - transform.forward * aerodynamicCoefficients.z * dynamicPressure * area * height;

        forceAndTorque.p += lift + drag;
        forceAndTorque.q += Vector3.Cross(relativePosition, forceAndTorque.p);
        forceAndTorque.q += moment;

        return forceAndTorque;
    }


    void setFlapAngle(float targetAngleDeg)
    {
        float targetAngleRad = targetAngleDeg * Mathf.Deg2Rad;
        flapAngle = Mathf.Clamp(targetAngleRad, -absMaxFlapAngle, absMaxFlapAngle);
    }
    float getAspectRatio()
    {
        return height / width;
    }
    float getSurfaceArea()
    {
        return height * width;
    }

    void OnDrawGizmos()
    {
        // draw using position+rotation only so sizes are in world units (global scale)
        Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);

        // sizes in world space
        float nonFlappedHeight = height * (1 - flapFraction);
        Vector3 offsetNoFlap = Vector3.right * (height / 2f - nonFlappedHeight / 2f);
        Gizmos.color = new Color(110f / 255f, 186f / 255f, 212f / 255f, 60f / 255f);
        Gizmos.DrawCube(offsetNoFlap, new Vector3(nonFlappedHeight, 0.1f, width));

        float flappedHeight = height * flapFraction;
        Vector3 offsetFlap = Vector3.right * (height / 2f - flappedHeight / 2f);
        Gizmos.color = new Color(235f / 255f, 129f / 255f, 73f / 255f, 120f / 255f);
        Gizmos.DrawCube(-offsetFlap, new Vector3(flappedHeight, 0.1f, width));
    }
}
