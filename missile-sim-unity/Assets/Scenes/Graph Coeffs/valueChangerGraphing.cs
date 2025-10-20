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

    public TMP_InputField liftSlopeInputField;

    public TMP_InputField zeroAoaInputField;
    public TMP_Text zeroAoaRadText;

    public TMP_Text skinDragValueText;

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
        flapAngleValueText.text = Convert.ToString(val) + "°: " + Convert.ToString(Mathf.Round(graphingScript.flapAngle/Mathf.PI*100)/100) + "π rad";
    }
    public void onLiftSlopeChange(string str)
    {
        float val = Utils.resolveStringValue(str);
        changingConfig.liftSlope = val;
        liftSlopeInputField.text = Convert.ToString(Mathf.Round(val/Mathf.PI*100)/100) + "π";
    }
    public void onZeroAoaChange(string str)
    {
        float val = float.Parse(str);
        changingConfig.zeroLiftAoa_deg = val;
        zeroAoaInputField.text = Convert.ToString(Mathf.Round(val*100)/100) + "°";
        zeroAoaRadText.text = Convert.ToString(Mathf.Round(val * Mathf.Deg2Rad / Mathf.PI * 100) / 100) + "π rad";
    }
    public void onSkinDragChange(float val)
    {
        changingConfig.skinDrag = val;
        skinDragValueText.text = Convert.ToString(val);
    }
    void Start()
    {
        onFlapFracChange(changingConfig.flapFraction * 20);
        onFlapAngleChange(graphingScript.flapAngle * Mathf.Rad2Deg);
        onLiftSlopeChange(Convert.ToString(changingConfig.liftSlope));
        onZeroAoaChange(Convert.ToString(changingConfig.zeroLiftAoa_deg));
        onSkinDragChange(changingConfig.skinDrag);
    }
}
