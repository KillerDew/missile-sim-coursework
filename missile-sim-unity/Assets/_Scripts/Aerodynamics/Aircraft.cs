using System.Collections.Generic;
using UnityEngine;

// Class requires the object to have a rigidbody for physics sim.
[RequireComponent(typeof(Rigidbody))]
public class Aircraft : MonoBehaviour
{
    public const float PREDICTION_TIMESTEP_FRACTION = 0.5f;

    Rigidbody RB;
    [SerializeField]
    // A LIST of references to aero surfaces, added in the Unity Editor.
    // Lists are similar to arrays, but more memory efficient.
    List<AeroSurface> aeroSurfaces = null;

    biVector3 currentForceAndTorque;

    // Air density at the aircraft's current altitude
    // Placeholder for now, will be calculated based on altitude (weather)
    float airDensity;

    Vector3 globalWind = Vector3.zero; // Placeholder for wind implementation

    void Awake()
    {
        // Find objects rigidbody before runtime.
        RB = GetComponent<Rigidbody>();
    }

    void Start()
    {
        //! TEST ONLY: initial upward velocity to see forces in action
        RB.linearVelocity = new(0, 40, 0);
    }

    void FixedUpdate()
    {
        // Calcilate aerodynamic forces based on current state
        biVector3 forceAndTorqueThisFrame = calculateAerodynamicForces(
            RB.linearVelocity, RB.angularVelocity, globalWind, RB.worldCenterOfMass
        );
        // Predict forces for next frame based on Euler integration of current state.
        Vector3 velocityPrediction = predictVelocity(forceAndTorqueThisFrame.p + Physics.gravity * RB.mass); // Velocity prediction based on current force, gravity and thrust (not implemented)
        Vector3 angularVelocityPrediction = PredictAngularVelocity(forceAndTorqueThisFrame.q); // Angular velocity prediction based on current torque
        biVector3 forceAndTorquePrediction = calculateAerodynamicForces(
            velocityPrediction, angularVelocityPrediction, globalWind, RB.worldCenterOfMass
        );
        // Calculate an avergage of current and future forces. This is much more stable.
        currentForceAndTorque = 0.5f * (forceAndTorqueThisFrame + forceAndTorquePrediction); // Average of current and predicted forces and torques avoids instability
        // Apply forces to the rigidbody
        RB.AddForce(currentForceAndTorque.p);
        RB.AddTorque(currentForceAndTorque.q);

        // TODO : Thrust forces
    }


    private biVector3 calculateAerodynamicForces(Vector3 velocity, Vector3 angularVelocity, Vector3 wind, Vector3 COM)
    {
        biVector3 totalForceAndTorque = biVector3.zero;
        // Calculate air density, currently a placeholder. (will be part of weather)
        airDensity = Utils.getAirDensityAtAltitude(transform.position.y);

        // Loop through each aero surface and sum up force and torque.
        foreach (AeroSurface surface in aeroSurfaces)
        {
            Vector3 relativePosition = surface.transform.position - COM;
            biVector3 surfaceForceAndTorque = surface.calculateForces(
                -RB.linearVelocity + wind - Vector3.Cross(RB.angularVelocity, relativePosition),
                airDensity, COM
            );
            totalForceAndTorque += surfaceForceAndTorque;
        }

        return totalForceAndTorque;
    }
    // Euler integration for velocity and angular velocity prediction. (a=F/m)
    private Vector3 predictVelocity(Vector3 force)
    {
        return RB.linearVelocity + Time.fixedDeltaTime * PREDICTION_TIMESTEP_FRACTION * (force / RB.mass);
    }
    // Angular velocity prediction using inertia tensor based on implementation from Jump Trajectory (https://github.com/gasgiant/Aircraft-Physics/blob/master/Assets/Aircraft%20Physics/Core/Scripts/AircraftPhysics.cs#L54)
    private Vector3 PredictAngularVelocity(Vector3 torque)
    {
        Quaternion inertiaTensorWorldRotation = RB.rotation * RB.inertiaTensorRotation; // Inertia tensor rotation in world space
        Vector3 torqueInDiagonalSpace = Quaternion.Inverse(inertiaTensorWorldRotation) * torque; // Torque in diagonal space
        Vector3 angularVelocityChangeInDiagonalSpace; // Change in angular velocity in diagonal space
        angularVelocityChangeInDiagonalSpace.x = torqueInDiagonalSpace.x / RB.inertiaTensor.x; // F = I * alpha  =>  alpha = F / I
        angularVelocityChangeInDiagonalSpace.y = torqueInDiagonalSpace.y / RB.inertiaTensor.y;
        angularVelocityChangeInDiagonalSpace.z = torqueInDiagonalSpace.z / RB.inertiaTensor.z;

        return RB.angularVelocity + Time.fixedDeltaTime * PREDICTION_TIMESTEP_FRACTION // Predicted angular velocity
            * (inertiaTensorWorldRotation * angularVelocityChangeInDiagonalSpace);
    }

    // temporary: gizmos to visualize forces etc
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + currentForceAndTorque.p * 0.01f);
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.position + currentForceAndTorque.q * 0.1f);
        if (RB != null)
        {     
            Gizmos.color = Color.white;
            Gizmos.DrawLine(transform.position, transform.position + RB.linearVelocity.normalized * 2f);
        }
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, transform.position + -transform.up * 2f);
    }
}
