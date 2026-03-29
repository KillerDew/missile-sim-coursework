using System.Collections;
using UnityEngine;
using XCharts.Runtime; // Charting library used to graph the coefficients

public class graphCoeffs : MonoBehaviour
{
    public AirfoilConfig config; // Airfoil being graphed
    public float flapAngle; // Flap angle in radians, set by valueChangerGraphing script
    public int sampleSize = 500; // No. samples to plot

    // References to the charts in Unity
    public LineChart Liftchart;
    public LineChart Dragchart;
    public LineChart Momentchart;
    Serie liftSerie;
    Serie dragSerie;
    Serie momentSerie;
    // Max and min AoAs to plot (degrees).
    public float min;
    public float max;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Get references to the series in the charts to add data to. (Not exposed in Unity editor).
        liftSerie = Liftchart.GetSerie(0);
        dragSerie = Dragchart.GetSerie(0);
        momentSerie = Momentchart.GetSerie(0);
        StartCoroutine(graphCoeffficients());
    }

    
    float changeTimer; // Timer to delay graph update to improve visuals
    int prevDatSize; // Previous sample size to check for changes and update graph accordingly
    float prevMin; // Previous min AoA to check for changes and update graph accordingly
    float prevMax; // Previous max AoA to check for changes and update graph accordingly
    AirfoilConfig prevAirfoilConfig; // Previous airfoil config to check for changes and update graph accordingly
    // Update is called once per frame
    void Update()
    {
        // Check for changes in graph parameters and update graph if necessary.
        // This allows for dynamic updates when parameters are changed.
        bool hasChanged = prevDatSize != sampleSize || prevMin != min
            || prevMax != max || prevAirfoilConfig != config;
        if (hasChanged)
        {
            // Reset timer to delay graph update for improved visuals.
            changeTimer = 1500;
            hasChanged = false; // Reset change flag until next check to prevent multiple resets.
        }
        else if (changeTimer > 0)
        {
            // decrease timer if no changes.
            changeTimer -= Time.deltaTime * 1000;
        }
        if ((changeTimer <= 0 && !hasChanged) || Input.GetKeyDown(KeyCode.R))
        {
            // If change timer has finished, or the R key is pressed
            // Trigger a graph redraw using X-charts 'animation reset' procedure.
            Liftchart.AnimationReset();
            Dragchart.AnimationReset();
            Momentchart.AnimationReset();
            hasChanged = true;
        }
        
        // Update previous parameter values for next check.
        prevDatSize = sampleSize;
        prevMin = min;
        prevMax = max;
        prevAirfoilConfig = Instantiate(config); // Instantiate = copy
    }

    // Coroutine to graph coefficients.
    // IEnumerator allows program to yield control when inactive.
    const float updateTime = 0.1f; // Time between updates in seconds.
    IEnumerator graphCoeffficients()
    {
        double x; // AoA in degrees
        Vector3 y; // Coefficients (x = lift, y = drag, z = moment)
        while (gameObject.activeInHierarchy) // Run while gameobject is active in scene 
        {
            x = min; // Set AoA to min value
            y = Vector3.zero; // Set coefficients to 0
            double difference = (max-min) / sampleSize; // Calculate step size between samples
            for (int i = 0; i < sampleSize; i++) // Loop for number of samples
            {
                x += difference; // Increment AoA by step size
                // Get coefficients for AoA (converted to radians) and airfoil config.
                y = Coefficients.getCoefficients((float)x * Mathf.Deg2Rad, config, 0f, 4f);

                // Add data to series in charts, X: AoA: Y coefficients.
                liftSerie.AddXYData(x, y.x);
                dragSerie.AddXYData(x, y.y);
                momentSerie.AddXYData(x, y.z);

            }
            yield return new WaitForSeconds(updateTime); // Yield control for specified time.
            // Clear data before new update.
            liftSerie.ClearData();
            dragSerie.ClearData();
            momentSerie.ClearData();
        }
    }
}
