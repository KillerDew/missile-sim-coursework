using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using UnityEditor;
using UnityEngine;
using XCharts.Runtime;

public class TestGraphScript : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField]
    float[,] dat1;
    [SerializeField]
    float[,] dat2;
    [SerializeField]
    float[,] dat3;

    public float freq;
    public float amplitude;
    public int datSize;

    public LineChart chart;
    Serie serie0;
    Serie serie1;
    Serie serie2;

    void Start()
    {
        serie0 = chart.GetSerie<Line>(0);
        serie1 = chart.GetSerie<Line>(1);
        serie2 = chart.GetSerie<Line>(2);
    }

    // Update is called once per frame
    void Update()
    {
        datSize = Mathf.Max(datSize, 0);
        serie0.ClearData();
        serie1.ClearData();
        serie2.ClearData();
        // dat1: sine
        for (int i = 0; i < datSize; i++)
        {
            float x = i * 360 / datSize;
            float y = Mathf.Sin(Mathf.Deg2Rad * x * freq) * amplitude;

            serie0.AddXYData(x, y);
        }
        // dat2: cos
        for (int i = 0; i < datSize; i++)
        {
            float x = i * 360 / datSize;
            float y = Mathf.Cos(Mathf.Deg2Rad * x * freq) * amplitude;
            serie1.AddXYData(x, y);
        }
        // dat3: tan
        for (int i = 0; i < datSize; i++)
        {
            float x = i * 360 / datSize;
            float y;
            if (x == 180)
            {
                y = 0;
            }
            else
            {
                y = Mathf.Tan(Mathf.Deg2Rad * x * freq) * amplitude;
            }
            ////serie2.AddXYData(x, y);
        }

    } 
}
