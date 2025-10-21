using JetBrains.Annotations;
using UnityEngine;

public class AeroSurface : MonoBehaviour
{
    [Header("Dimensions")]
    public float width = 1.0f;
    public float height = 1.0f;
    public Vector3 rotationOffset = Vector3.zero;

    [Header("Properties")]
    [Range(0f, 1f)]
    public float flapFraction = 0.2f;
    public float flapAngle { get; private set; } = 0f;
    public float absMaxFlapAngle = Mathf.PI / 12f;

    public AirfoilConfig airfoilConfig;
    Rigidbody RB;

    void Start()
    {
        RB = GetComponentInParent<Rigidbody>();
        //RB.AddForce(Vector3.right * 1000);
    }
    void FixedUpdate()
    {
        Vector3 airVel = -RB.GetPointVelocity(transform.position);
        biVector3 forces = calculateForces(airVel, 1.225f, transform.position);
        if (forces.p.magnitude >= 1e-2) RB.AddForce(forces.p);
        if (forces.q.magnitude >= 1e-2) RB.AddTorque(forces.q);
    }

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
        Gizmos.matrix = transform.localToWorldMatrix;


        float nonFlappedHeight = height * (1 - flapFraction);
        Vector3 offsetNoFlap = (height / 2 - nonFlappedHeight / 2) * transform.right / Vector3.Dot(transform.right, transform.lossyScale);
        Gizmos.color = new Color(110, 186, 212, 60) / 255f;
        Gizmos.DrawCube(offsetNoFlap, new Vector3(nonFlappedHeight/transform.lossyScale.x, 0.1f/transform.lossyScale.y, width/transform.lossyScale.z));

        float flappedHeight = height * flapFraction;
        Vector3 offsetFlap = (height / 2 - flappedHeight / 2) * transform.right / Vector3.Dot(transform.right, transform.lossyScale);;
        Gizmos.color = new Color(235, 129, 73, 120) / 255f;
        Gizmos.DrawCube(-offsetFlap, (new Vector3(flappedHeight/transform.lossyScale.x, 0.1f/transform.lossyScale.y, width/transform.lossyScale.z)));
    }
}
