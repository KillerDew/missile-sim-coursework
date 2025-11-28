using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class threeLoopAutopilot
{
    // Outer Loop (Acceleration)
    PIDController pitchAccelPID;
    PIDController yawAccelPID;

    // Middle Loop (Rate)
    PIDController pitchRatePID;
    PIDController yawRatePID;

    // Inner Loop (Angle)
    PIDController pitchAnglePID;
    PIDController yawAnglePID;


    public List<float> Update(
        float deltaTime,
        Vector3 accelCmd,
        MissileController missileController
    )
    {
        Vector3 accelCmdBody = Quaternion.Inverse(missileController.aircraftScript.transform.rotation) * accelCmd;
        Vector3 accelBody = Quaternion.Inverse(missileController.aircraftScript.transform.rotation) * missileController.aircraftScript.getAccelWorld();
        Vector3 accelError = accelCmd - accelBody;

        Vector3 currentVelBody = Quaternion.Inverse(missileController.aircraftScript.transform.rotation) * missileController.aircraftScript.getVelocityWorld();
        Vector3 currentAngularVelocityBody = Quaternion.Inverse(missileController.aircraftScript.transform.rotation) * missileController.aircraftScript.getAngularVelWorld() * Mathf.Rad2Deg;

        float currentPitchRate = currentAngularVelocityBody.z; // Unity's z-axis rotation maps to pitch rate (q)
        float currentYawRate = currentAngularVelocityBody.y;   // Unity's y-axis rotation maps to yaw rate (r)

        // Get current attitude angles from the quaternion
        Vector3 eulerAngles = missileController.aircraftScript.transform.rotation.eulerAngles;

        // Controller for acceleration cmd -> pitch cmd
        float pitchCmd = pitchAccelPID.Update(accelBody.x, deltaTime);
        float yawCmd = yawAccelPID.Update(accelBody.x, deltaTime);

        // Controller for pitch cmd -> pitch rate cmd
        float pitchRateCmd = pitchRatePID.Update(pitchCmd, deltaTime);
        float yawRateCmd = yawRatePID.Update(yawCmd, deltaTime);

        // Controller for pitch rate cmd -> fin cmd
        float pitchFinCmd = pitchAnglePID.Update(pitchRateCmd, deltaTime);
        float yawFinCmd = yawAnglePID.Update(yawRateCmd, deltaTime);

        return new List<float> {pitchFinCmd, yawFinCmd};
    }
}