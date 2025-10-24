using System;
using UnityEngine;


public struct biVector3
{
    public Vector3 p;
    public Vector3 q;
    public biVector3(Vector3 force, Vector3 torque)
    {
        this.p = force;
        this.q = torque;
    }

    public static biVector3 zero = new biVector3(Vector3.zero, Vector3.zero);

    public static biVector3 operator +(biVector3 a, biVector3 b)
    {
        return new biVector3(a.p + b.p, a.q + b.q);
    }
    public static biVector3 operator *(biVector3 a, float f)
    {
        return new biVector3(a.p * f, a.q * f);
    }
    public static biVector3 operator *(float f, biVector3 a)
    {
        return new biVector3(a.p * f, a.q * f);
    }
    public static biVector3 operator /(biVector3 a, float f)
    {
        return new biVector3(a.p / f, a.q / f);
    }

}
public static class Utils
{

    public static float getAirDensityAtAltitude(float altitude)
    {
        return 1.225f; // Placeholder: returns sea level air density
    }

    /*
    public static bool isAllDigits(string s)
    {
        foreach (char c in s)
        {
            if (!char.IsDigit(c) && c != '.')
                return false;
        }
        return true;
    }
    public static float resolveStringValue(string str)
    {
        // Special values
        switch (str)
        {
            case "pi":
                return Mathf.PI;
            case "e":
                return Mathf.Exp(1);
            case "tau":
                return Mathf.PI * 2;
            case "g":
                return 9.81f;
            default:
                break;
        }
        if (isAllDigits(str))
        {
            return (float)Convert.ToDouble(str);
        }
        string[] splitString = str.Split(" ");
        if (splitString.Length > 3)
        {
            return resolveStringValue(splitString[0]);
        }
        else if (splitString.Length == 3)
        {
            float val1 = resolveStringValue(splitString[0]);
            float val2 = resolveStringValue(splitString[2]);
            switch (splitString[1])
            {
                case "+":
                    return val1 + val2;
                case "-":
                    return val1 - val2;
                case "*":
                    return val1 * val2;
                case "/":
                    return val1 / val2;
                default:
                    Debug.LogError("Invalid operator in string: " + str);
                    return 0;
            }
        }
        else if (splitString.Length == 2)
        {
            float val = resolveStringValue(splitString[1]);
            switch (splitString[0])
            {
                case "+":
                    return val;
                case "-":
                    return -val;
                default:
                    break;
            }
        }
        else if (splitString.Length == 1)
        {
            string constant = "";
            string val = "";
            char[] charArr = splitString[0].ToCharArray();
            Array.Reverse(charArr);
            foreach (char c in charArr)
            {
                if (!char.IsDigit(c) && c != '.' && c != '-') // Allow digits, decimal point, and negative sign
                {
                    constant.Insert(0, c.ToString());
                }
                else if (constant == "*") { continue; } // Ignore multiplication signs
                else if (char.IsDigit(c))
                {
                    val.Insert(0, c.ToString());
                }
            }
            return resolveStringValue(val) * resolveStringValue(constant);
        }
        else
        {
            Debug.LogError("Invalid string format: " + str);
            return 0;
        }
        return 0;

    }
    */
}
