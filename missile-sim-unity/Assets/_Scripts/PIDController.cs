using UnityEngine;

[System.Serializable]
public class PIDController
{
    // Tuning Parameters
    public float pGain;
    public float iGain;
    public float dGain;
    public float maxAcceptedError = 0.05f;

    // Safety Limits
    public float outputMax = 10f; // Maximum output for the controller
    public float outputMin = -10f; // Minimum output for the controller
    public float integralLimit = 10f; // Anti-windup limit for the integral term

    // State Variables
    private float integralSum = 0f;
    private float lastError = 0f; // Used for derivative calculation

    // Constructor
    public PIDController(float p, float i, float d, float outMax, float outMin, float iLimit)
    {
        pGain = p;
        iGain = i;
        dGain = d;
        outputMax = outMax;
        outputMin = outMin;
        integralLimit = iLimit;
        // Initialize state variables to zero on creation
        integralSum = 0f;
        lastError = 0f;
    }

    /// <summary>
    /// Calculates the PID control output.
    /// </summary>
    /// <param name="setpoint">The desired target value.</param>
    /// <param name="processVariable">The current measured value (e.g., position, speed).</param>
    /// <param name="deltaTime">The time elapsed since the last call (e.g., Time.fixedDeltaTime).</param>
    /// <returns>The calculated control signal (e.g., force, torque).</returns>
    public float Update(float error, float deltaTime)
    {

        // 2. Proportional Term
        float proportional = pGain * error;

        // 3. Integral Term (with Anti-Windup)
        integralSum += error * deltaTime;

        // Anti-Windup: Clamp the integral sum to prevent it from growing too large
        // when the system is saturated (e.g., max speed/force).
        integralSum = Mathf.Clamp(integralSum, -integralLimit, integralLimit);

        float integral = iGain * integralSum;

        // 4. Derivative Term (Derivative on Measurement to avoid Derivative Kick)
        // Derivative Kick: A sudden, large spike in output when the setpoint changes
        // if the derivative is calculated based on the error's change.
        // Solution: Calculate the derivative based on the *rate of change of the process variable (PV)*.
        float pvRateOfChange = (error - lastError) / deltaTime;
        float derivative = dGain * pvRateOfChange; // Negative sign because we use the PV change

        // 5. Total Output
        float output = proportional + integral + derivative;

        // 6. Output Clamping
        // Ensure the final output respects the system's limits
        output = Mathf.Clamp(output, outputMin, outputMax);

        // 7. Store current PV for next iteration's derivative calculation
        lastError = error;

        if (Mathf.Abs(error) < maxAcceptedError)
        {
            return 0;
        }else
        {
            return output;
        }
    }

    /// <summary>
    /// Resets the integral sum and last PV, useful when resetting the system.
    /// </summary>
    public void Reset()
    {
        integralSum = 0f;
        lastError = 0f;
    }
}