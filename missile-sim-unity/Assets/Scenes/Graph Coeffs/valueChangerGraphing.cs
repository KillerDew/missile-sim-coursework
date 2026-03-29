using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class valueChangerGraphing : MonoBehaviour
{
    static float resolveExp(string exp)
    {
        // Remove spaces
        exp = exp.Replace(" ", "");
        // Replace pi with its value (handles multiple cases)
        exp = exp
            .Replace("pi", MathF.PI.ToString())
            .Replace("PI", MathF.PI.ToString())
            .Replace("Pi", MathF.PI.ToString())
            .Replace("π", MathF.PI.ToString());

        // Mathematically resolve string
        float result = 0.0f;
        // Uses Unity's evaluator
        bool status = ExpressionEvaluator.Evaluate(exp, out result); 
        if (!status)
        {
            // Logs error and avoids crashes
            Debug.LogError("Expression could not be evaluated."); 
        }
        // Returns result (or 0 if evaluation failed)
        return result; 
    }

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

    // Callback for changes to flap fraction slider.
    public void onFlapFracChange(float val)
    {
        // Slider now goes from 0 to 20 (int values only) to allow for more precision.
        // Conversion from 0-20 to 0-1 is done here.
        val = val / 20;
        // Change the flap fraction in the config, and update the text to show the current value
        changingConfig.flapFraction = val;
        flapFractionValueText.text = Convert.ToString(val);
    }
    // Callback for changes to flap angle slider.
    public void onFlapAngleChange(float val)
    {
        // Change the flap angle in the graphing script.
        graphingScript.flapAngle = Mathf.Deg2Rad * val;
        // Updates the the flap angle indicator and text.
        flapAngleIndicatorSlice.localEulerAngles = new Vector3(0, 0, -val);
        flapAngleValueText.text = Convert.ToString(val) + "°: " + Convert.ToString(Mathf.Round(graphingScript.flapAngle/Mathf.PI*100)/100) + "π rad";
    }
    public void onLiftSlopeChange(string str)
    {
        // Evaluates string and sets values
        float val = resolveExp(str);
        changingConfig.liftSlope = val;
        // Round to 2dp
        val = Mathf.Round(val*100)/100;
        // Displays value in field text
        liftSlopeInputField.text = Convert.ToString(val);
    }
    public void onZeroAoaChange(string str)
    {
        // Evaluates string and sets values
        float val = resolveExp(str);
        changingConfig.zeroLiftAoa_deg = val;
        // Displays value in field text
        zeroAoaInputField.text = Convert.ToString(Mathf.Round(val*100)/100);
        // Displays value in radians in seperate text display below
        float radInPi = changingConfig.zeroLiftAoa/Mathf.PI; // In terms of pi
        radInPi = Mathf.Round(radInPi*100)/100; // Rounds 2dp
        // Displays pi next to it.
        zeroAoaRadText.text = Convert.ToString(radInPi) + "π rad";
    }
    public void onSkinDragChange(float val)
    {
        changingConfig.skinDrag = val;
        skinDragValueText.text = Convert.ToString(Mathf.Round(val*100)/100);
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
