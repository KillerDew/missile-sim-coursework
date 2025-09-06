using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class valueChangerGraphing : MonoBehaviour
{
    public AirfoilConfig changingConfig;

    [Header("UI")]
    public TMP_Text flapFractionValueText;
    public void onFlapFracChange(float val)
    {
        val = val / 20;
        changingConfig.flapFraction = val;
        flapFractionValueText.text = Convert.ToString(val);
    }
    void Start()
    {
        onFlapFracChange(changingConfig.flapFraction*20);
    }
}
