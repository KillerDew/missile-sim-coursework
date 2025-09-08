using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class valueChangerGraphing : MonoBehaviour
{
    public AirfoilConfig changingConfig;
    public graphCoeffs graphingScript;

    [Header("UI")]
    public TMP_Text flapFractionValueText;
    public TMP_Text flapAngleValueText;
    public Transform flapAngleIndicatorSlice;

    public void onFlapFracChange(float val)
    {
        val = val / 20;
        changingConfig.flapFraction = val;
        flapFractionValueText.text = Convert.ToString(val);
    }
    public void onFlapAngleChange(float val)
    {
        graphingScript.flapAngle = Mathf.Deg2Rad * val;
        flapAngleIndicatorSlice.localEulerAngles = new Vector3(0, 0, -val);
        flapAngleValueText.text = Convert.ToString(val) + ": " + Convert.ToString(Mathf.Round(graphingScript.flapAngle/Mathf.PI*100)/100) + "π rad";
    }
    void Start()
    {
        onFlapFracChange(changingConfig.flapFraction * 20);
        onFlapAngleChange(graphingScript.flapAngle * Mathf.Rad2Deg);
    }
}
