using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Aircraft : MonoBehaviour
{
    public const float PREDICTION_TIMESTEP_FRACTION = 0.5f;

    Rigidbody RB;
    [SerializeField]
    List<AeroSurface> aeroSurfaces = null;

    biVector3 currentForceAndTorque;
    Vector3 centreOfMassOffset = Vector3.zero;

    float airDensity;


    void Awake()
    {
        RB = GetComponent<Rigidbody>();
    }

    void Start()
    {
        
    }

    void FixedUpdate()
    {
        Vector3 globalWind = Utils.getWindAtAltitude(transform.position.y);
        biVector3 forceAndTorqueThisFrame = calculateAerodynamicForces(
            RB.linearVelocity, RB.angularVelocity, globalWind, RB.worldCenterOfMass
        );

        Vector3 velocityPrediction = predictVelocity(forceAndTorqueThisFrame.p + Physics.gravity * RB.mass); // Velocity prediction based on current force, gravity and thrust (not implemented)
        Vector3 angularVelocityPrediction = PredictAngularVelocity(forceAndTorqueThisFrame.q); // Angular velocity prediction based on current torque
        biVector3 forceAndTorquePrediction = calculateAerodynamicForces(
            velocityPrediction, angularVelocityPrediction, globalWind, RB.worldCenterOfMass
        );

        currentForceAndTorque = 0.5f * (forceAndTorqueThisFrame + forceAndTorquePrediction); // Average of current and predicted forces and torques avoids instability

        RB.AddForce(currentForceAndTorque.p);
        RB.AddTorque(currentForceAndTorque.q);

        //RB.AddForce(transform.forward * 1000f, ForceMode.Force);
        // TODO : Thrust forces
    }


    private biVector3 calculateAerodynamicForces(Vector3 velocity, Vector3 angularVelocity, Vector3 wind, Vector3 COM)
    {
        biVector3 totalForceAndTorque = biVector3.zero;

        airDensity = Utils.getAirDensityAtAltitude(transform.position.y);

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
        Gizmos.DrawLine(transform.position, transform.position + transform.forward * 5f);
    }
}
