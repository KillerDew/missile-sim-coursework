using UnityEditor.MPE;
using UnityEngine;

[RequireComponent(typeof(Aircraft))]
public class MissileController : MonoBehaviour
{
    [SerializeField]
    public PIDController rollPID;
    public PIDController pitchPID;
    public PIDController yawPID;

    public Transform fin1;
    public Transform fin2;
    public Transform fin3;
    public Transform fin4;

    public float rollTarget = 45f;
    public float pitchTarget = -45f;
    public float yawTarget = 0f;

    float rollControl = 0f;
    float pitchControl = 0f;
    float yawControl = 0f;

    private float[] finCmds = new float[4];

    public SolidRocketMotor motor;
    Aircraft aircraftScript;

    void Awake()
    {
        aircraftScript = GetComponent<Aircraft>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Time.timeScale = 0.5f;
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        rollControl = rollPID.Update(transform.eulerAngles.z, rollTarget, Time.fixedDeltaTime);
        pitchControl = pitchPID.Update(getPitch(), pitchTarget, Time.fixedDeltaTime);
        yawControl = yawPID.Update(getYaw(), yawTarget, Time.fixedDeltaTime);
        //print(getYaw() + " | " + yawTarget + " | " + yawControl);

        finCmds[0] = Mathf.Clamp(-Mathf.Sin(Mathf.Deg2Rad * transform.eulerAngles.z) * pitchControl - Mathf.Cos(Mathf.Deg2Rad * transform.eulerAngles.z) * yawControl + rollControl, -10, 10);
        finCmds[1] = Mathf.Clamp(-Mathf.Cos(Mathf.Deg2Rad * transform.eulerAngles.z) * pitchControl - Mathf.Sin(Mathf.Deg2Rad * transform.eulerAngles.z) * yawControl + rollControl, -10, 10);
        finCmds[2] = Mathf.Clamp(Mathf.Sin(Mathf.Deg2Rad * transform.eulerAngles.z) * pitchControl + Mathf.Cos(Mathf.Deg2Rad * transform.eulerAngles.z) * yawControl + rollControl, -10, 10);
        finCmds[3] = Mathf.Clamp(Mathf.Cos(Mathf.Deg2Rad * transform.eulerAngles.z) * pitchControl + Mathf.Sin(Mathf.Deg2Rad * transform.eulerAngles.z) * yawControl + rollControl, -10, 10);

        fin1.localEulerAngles = new Vector3(fin1.localEulerAngles.x, fin1.localEulerAngles.y, 90 + finCmds[0]);
        fin2.localEulerAngles = new Vector3(fin2.localEulerAngles.x, fin2.localEulerAngles.y, 90 + finCmds[1]);
        fin3.localEulerAngles = new Vector3(fin3.localEulerAngles.x, fin3.localEulerAngles.y, 90 + finCmds[2]);
        fin4.localEulerAngles = new Vector3(fin4.localEulerAngles.x, fin4.localEulerAngles.y, 90 + finCmds[3]);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            motor.ignite();
        }
        aircraftScript.addOffsetForce(motor.getThrustForce(transform));
    }

    public float getPitch()
    {
        if (transform.eulerAngles.x > 180f)
        {
            return transform.eulerAngles.x - 360f;
        }
        else if (transform.eulerAngles.x < -180f)
        {
            return transform.eulerAngles.x + 360f;
        }
        else return transform.eulerAngles.x;
    }
    public float getYaw()
    {
        if (transform.eulerAngles.y > 180f)
        {
            return transform.eulerAngles.y - 360f;
        }
        else if (transform.eulerAngles.y < -180f)
        {
            return transform.eulerAngles.y + 360f;
        }
        else return transform.eulerAngles.y;
    }
    public float getRoll()
    {
        if (transform.eulerAngles.z > 180f)
        {
            return transform.eulerAngles.z - 360f;
        }
        else if (transform.eulerAngles.z < -180f)
        {
            return transform.eulerAngles.z + 360f;
        }
        else return transform.eulerAngles.z;
    }

    void OnDrawGizmos()
    {
        if (motor != null && motor.source != null && motor.direction != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(motor.source.position, 0.2f);
            Gizmos.color = Color.yellow;
            Gizmos.DrawRay(motor.source.position, -transform.TransformDirection(motor.direction).normalized);
        }
    }
}
