using UnityEngine;

public class AeroSurface : MonoBehaviour
{
    [Header("Dimensions")]
    // Changed in Unity Editor
    public float width = 1.0f;
    public float height = 1.0f;
    // Euler angle offset for surface
    public Vector3 rotationOffset = Vector3.zero;

    [Header("Properties")]
    [Range(0f, 1f)] // Flap fraction for surface (0-1)
    public float flapFraction = 0.2f;
    public float flapAngle { get; private set; } = 0f; // in radians
    // Maximum flap angle in radians

    public float absMaxFlapAngle = Mathf.PI / 12f; 
    // Characteristic config for coefficient calculation
    public AirfoilConfig airfoilConfig;
    // TEST ONLY. Normally physics calculations are done seperately.
    Rigidbody RB;

    void Start()
    {
        RB = GetComponentInParent<Rigidbody>();
    }
    //! TEST SECTION. PHYSICS WILL BE HANDLED BY SEPERATE MANAGER
    void FixedUpdate()
    {
        Vector3 airVel = -RB.GetPointVelocity(transform.position);
        biVector3 forces = calculateForces(airVel, 1.225f, transform.position);
        if (forces.p.magnitude >= 1e-2) RB.AddForce(forces.p);
        if (forces.q.magnitude >= 1e-2) RB.AddTorque(forces.q);
    }
    //! -- END OF TEST SECTION

    // Calculates forces and torques based on air velocity, air density and parent position (offset for torque)
    public biVector3 calculateForces(Vector3 worldAirVelocity, float airdensity, Vector3 parentPosition)
    {   
        // Initialise force and toeque to 0
        biVector3 forceAndTorque = biVector3.zero;
        // If surface is inactive, return 0 force and torque
        if (!gameObject.activeInHierarchy) return forceAndTorque;

        // Relative position from parent to surface for torque calculations
        Vector3 relativePosition = transform.position - parentPosition;
        // Aspect ratio and area for coefficient/force calculations
        float aspectRatio = getAspectRatio();
        float area = getSurfaceArea();

        // Calculate air velocity flowing over surface in local space (Unity function)
        Vector3 airVelLocal = transform.InverseTransformDirection(worldAirVelocity);
        // Discard z component (assume 2d flow over surrface)
        airVelLocal = new Vector3(airVelLocal.x, airVelLocal.y);
        // Lift acts directly perpendicular to flow, drag acts directly against flow.
        // Use cross product to find lift direction.
        // Normalised for direction only
        Vector3 dragDir = transform.TransformDirection(airVelLocal.normalized).normalized;
        Vector3 liftDir = Vector3.Cross(dragDir, transform.forward).normalized;

        // Dynamic pressure calculation (0.5 * rho * v^2), simplifies force calcs
        float dynamicPressure = 0.5f * airdensity * airVelLocal.sqrMagnitude;
        // Angle of attack on surface
        float alpha = Mathf.Atan2(airVelLocal.y, -airVelLocal.x);

        // Calculate aerodynamic coefficients from estimation script
        Vector3 aerodynamicCoefficients = Coefficients.getCoefficients(
            alpha,
            airfoilConfig,
            flapAngle,
            flapFraction,
            aspectRatio
        ); // (lift, drag, moment)

        // Calculate lift, drag and moment forces from coefficients and dynamic pressure.
        Vector3 lift = liftDir * aerodynamicCoefficients.x * dynamicPressure * area;
        Vector3 drag = dragDir * aerodynamicCoefficients.y * dynamicPressure * area;
        // Some extra factors added to convert moment scalar to correctly directionalised torque. 
        Vector3 moment = - transform.forward * aerodynamicCoefficients.z * dynamicPressure * area * height;

        // Sum forces and torques, add to biVector3 and return.
        forceAndTorque.p += lift + drag;
        // Torque is a sum of both moment (directly from aerodynamics)
        // and also torque generated as a result of lift and drag acting at distance from COM
        // torque = r x F (cross product of relative position and force)
        forceAndTorque.q += Vector3.Cross(relativePosition, forceAndTorque.p);
        forceAndTorque.q += moment;

        return forceAndTorque;
    }

    // Private abstracted methods for calculations
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


    // Unity display of surface and flap in editor for debugging.
    void OnDrawGizmos()
    {
        float nonFlappedHeight = height * (1 - flapFraction);
        Vector3 offsetNoFlap = new(height/2 - nonFlappedHeight / 2, 0f, 0f);
        Gizmos.color = new Color(110, 186, 212, 120) / 255f;
        Gizmos.DrawCube(transform.position + offsetNoFlap, new Vector3(nonFlappedHeight, 0.1f, width));

        float flappedHeight = height * flapFraction;
        Vector3 offsetFlap = new(height / 2 - flappedHeight / 2, 0f, 0f);
        Gizmos.color = new Color(235, 129, 73, 120) / 255f;
        Gizmos.DrawCube(transform.position - offsetFlap, new Vector3(flappedHeight, 0.1f, width));
    }
}
