using System.Collections;
using UnityEngine;
using XCharts.Runtime;

public class graphCoeffs : MonoBehaviour
{
    public AirfoilConfig config;
    public float flapAngle = 0f;
    public int sampleSize = 500;

    public LineChart Liftchart;
    public LineChart Dragchart;
    public LineChart Momentchart;
    Serie liftSerie;
    Serie dragSerie;
    Serie momentSerie;
    public float min;
    public float max;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        liftSerie = Liftchart.GetSerie(0);
        dragSerie = Dragchart.GetSerie(0);
        momentSerie = Momentchart.GetSerie(0);
        StartCoroutine(graphCoeffficients());
    }

    // Update is called once per frame
    float changeTimer;
    int prevDatSize;
    float prevMin;
    float prevMax;
    bool hasChanged;
    double prevData;
    AirfoilConfig prevAirfoilConfig;
    void Update()
    {
        if (prevDatSize != sampleSize || prevMin != min || prevMax != max || prevAirfoilConfig != config)
        {
            changeTimer = 1500;
            hasChanged = false;
        }
        else if (changeTimer > 0)
        {
            changeTimer -= Time.deltaTime * 1000;
        }
        if ((changeTimer <= 0 && !hasChanged) || Input.GetKeyDown(KeyCode.R))
        {
            Liftchart.AnimationReset();
            Dragchart.AnimationReset();
            Momentchart.AnimationReset();
            hasChanged = true;
        }

        prevDatSize = sampleSize;
        prevMin = min;
        prevMax = max;
        prevAirfoilConfig = Instantiate(config);
    }

    IEnumerator graphCoeffficients()
    {
        double x = min;
        Vector3 y = Vector3.zero;
        while (true)
        {
            x = min;
            y = Vector3.zero;
            double difference = (max-min) / sampleSize;
            for (int i = 0; i < sampleSize; i++)
            {
                x += difference;
                y = Coefficients.getCoefficients((float)x * Mathf.Deg2Rad, config, flapAngle, 4f);

                liftSerie.AddXYData(x, y.x);
                dragSerie.AddXYData(x, y.y);
                momentSerie.AddXYData(x, y.z);

            }
            yield return new WaitForSeconds(0.1f);
            liftSerie.ClearData();
            dragSerie.ClearData();
            momentSerie.ClearData();
        }
    }
}
