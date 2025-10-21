using System;
using UnityEngine;

/*
* Realtime aerodynamic modelling of control surfaces and/or wings
* Imperical aerodynamic coefficient moddeling ased on the research
* by Waqas Khan and Meyor Nahon in 'Real-Time Modeling of Agile Fixed-Wing UAV Aerodynamics'
* https://www.researchgate.net/publication/281558306_Real-Time_Modeling_of_Agile_Fixed-Wing_UAV_Aerodynamics
* Script based on video and project by Jump Trajectory 'Realistic Aircraft Physics for Games'
* https://www.youtube.com/watch?v=p3jDJ9FtTyM
*/



public class Coefficients
{

    static float getDragAtNormal(float flapAngle)
    {
        // calculates imperically coefficient of drag based on eq. 10
        float friction = 1.98f - 0.0426f * flapAngle * flapAngle + 0.21f * flapAngle;
        return friction;
    }
    static Vector3 getCoefficientsLowAoA(float alpha, AirfoilConfig config, float flapAngle, float aspectRatio)
    {
        // cofficient calculations pre-stall based on eq. 8
        float liftCoefficient = config.liftSlope * (alpha - config.zeroLiftAoa);
        float inducedAlpha = liftCoefficient / (Mathf.PI * aspectRatio);
        float effectiveAlpha = alpha - config.zeroLiftAoa - inducedAlpha;

        float tangentialCoefficient = config.skinDrag * Mathf.Cos(effectiveAlpha);
        float normalCoefficient = (liftCoefficient + tangentialCoefficient * Mathf.Sin(effectiveAlpha)) / Mathf.Cos(effectiveAlpha);

        float dragCoefficient = normalCoefficient * Mathf.Sin(effectiveAlpha) + tangentialCoefficient * Mathf.Cos(effectiveAlpha);

        float momentCoefficient = -normalCoefficient * (0.25f - 0.175f * (Mathf.Abs(effectiveAlpha) / Mathf.PI));

        Vector3 coeffs = new(liftCoefficient, dragCoefficient, momentCoefficient);
        return coeffs;
    }

    static Vector3 getCoefficientsStall(float alpha, AirfoilConfig config, float flapAngle, float flapFraction, float aspectRatio)
    {
        // Linear interpolates induced alpha at moment of stall to 0 at +- 90deg
        float inducedAlpha;
        if (alpha > config.stallAngleHigh)
        {
            float lerpFactor = (Mathf.PI / 2 - alpha) / (Mathf.PI / 2 - config.stallAngleHigh);
            float liftPreStall = config.liftSlope * (config.stallAngleHigh - config.zeroLiftAoa);
            float inducedAlphaPreStall = liftPreStall / (Mathf.PI * aspectRatio);
            inducedAlpha = Mathf.Lerp(inducedAlphaPreStall, 0, lerpFactor);
        }
        else
        {
            float lerpFactor = (-Mathf.PI / 2 - alpha) / (-Mathf.PI / 2 - config.stallAngleLow);
            float liftPreStall = config.liftSlope * (config.stallAngleLow - config.zeroLiftAoa);
            float inducedAlphaPreStall = liftPreStall / (Mathf.PI * aspectRatio);
            inducedAlpha = Mathf.Lerp(inducedAlphaPreStall, 0, lerpFactor);
        }

        // Coefficient calculations based on eq.9, with crucial fixes found through experimentation
        // Previously, the use of sine in normal and moment coefficient calculations caused major asymetrical behaviour
        // Found issue during python experimentation, fixed using absoloute values as suggested by Jump Trajectory
        float effectiveAlpha = alpha - config.zeroLiftAoa - inducedAlpha;

        float Cd90 = getDragAtNormal(flapAngle);

        float normalCoefficient = Cd90 * Mathf.Sin(effectiveAlpha)
            * (1f / (0.56f + 0.44f * Mathf.Abs(Mathf.Sin(effectiveAlpha))) - 0.41f * (1f - Mathf.Exp(-17 / aspectRatio)));
        float tangentialCoefficient = 0.5f * config.skinDrag * Mathf.Cos(effectiveAlpha);

        float liftCoefficient = normalCoefficient * Mathf.Cos(effectiveAlpha) - tangentialCoefficient * Mathf.Sin(effectiveAlpha);
        float dragCoefficient = normalCoefficient * Mathf.Sin(effectiveAlpha) + tangentialCoefficient * Mathf.Cos(effectiveAlpha);
        float momentCoefficient = -normalCoefficient * (0.25f - 0.175f * (1f - 2f * Mathf.Abs(effectiveAlpha) / Mathf.PI));

        Vector3 coeffs = new(liftCoefficient, dragCoefficient, momentCoefficient);
        return coeffs;
    }
    static public Vector3 getCoefficients(float alpha, AirfoilConfig config, float flapAngle, float flapFraction, float aspectRatio)
    {
        // adjust constants
        AirfoilConfig adjustedConfig = AirfoilConfig.Instantiate(config);
        float theta = Mathf.Acos(2 * flapFraction - 1);
        float tau = 1 - (theta - Mathf.Sin(theta)) / MathF.PI;
        float correction = Mathf.Lerp(0.8f, 0.4f, (Mathf.Abs(flapAngle) * Mathf.Rad2Deg - 10) / 50);

        float correctedLiftSlope = config.liftSlope * (aspectRatio / (aspectRatio + 2 * (aspectRatio + 4) / (aspectRatio + 2)));
        float deltaClSlope = correctedLiftSlope * tau * correction * flapAngle;

        float liftMaxFraction = Mathf.Clamp(1 - 0.5f * (flapFraction - 0.1f) / 0.3f, 0, 1f);

        float ClMax = correctedLiftSlope * (config.stallAngleHigh - config.zeroLiftAoa) + deltaClSlope * liftMaxFraction;
        float ClMin = correctedLiftSlope * (config.stallAngleLow - config.zeroLiftAoa) + deltaClSlope * liftMaxFraction;

        float zeroLiftAoANew = config.zeroLiftAoa - deltaClSlope / correctedLiftSlope;
        float stallHighNew = zeroLiftAoANew + ClMax / correctedLiftSlope;
        float stallLowNew = zeroLiftAoANew + ClMin / correctedLiftSlope;

        adjustedConfig.zeroLiftAoa_deg = zeroLiftAoANew * Mathf.Rad2Deg;
        adjustedConfig.liftSlope = correctedLiftSlope;
        adjustedConfig.stallHigh_deg = stallHighNew * Mathf.Rad2Deg;
        adjustedConfig.stallLow_deg = stallLowNew * Mathf.Rad2Deg;

        //TODO finish corrections
        if (alpha >= adjustedConfig.stallAngleLow && alpha <= adjustedConfig.stallAngleHigh)
        {
            return getCoefficientsLowAoA(alpha, adjustedConfig, flapAngle, aspectRatio);
        }
        else
        {
            // adjust blending region for different flap angles
            // this accounts for the harsher region changes with higher angles
            float lerpFactorHigh = ((flapAngle * Mathf.Rad2Deg) + 50) / 100;
            float lerpFactorLow = ((-flapAngle * Mathf.Rad2Deg) + 50) / 100;
            float padAngleHigh = Mathf.Lerp(15, 5, lerpFactorHigh) * Mathf.Deg2Rad;
            float padAngleLow = Mathf.Lerp(15, 5, lerpFactorLow) * Mathf.Deg2Rad;
            float paddedStallHigh = adjustedConfig.stallAngleHigh + padAngleHigh;
            float paddedStallLow = adjustedConfig.stallAngleLow - padAngleLow;

            if (alpha > paddedStallHigh || alpha < paddedStallLow)
            {
                // No blending is required
                return getCoefficientsStall(alpha, adjustedConfig, flapAngle, flapFraction, aspectRatio);
            }
            else
            {
                Vector3 coeffs;
                if (alpha > adjustedConfig.stallAngleHigh)
                {
                    Vector3 preStall = getCoefficientsLowAoA(adjustedConfig.stallAngleHigh, adjustedConfig, flapAngle, aspectRatio);
                    Vector3 postStall = getCoefficientsStall(paddedStallHigh, adjustedConfig, flapAngle, flapFraction, aspectRatio);
                    float lerpFactor = (alpha - adjustedConfig.stallAngleHigh) / (paddedStallHigh - adjustedConfig.stallAngleHigh);
                    coeffs = Vector3.Lerp(preStall, postStall, lerpFactor);
                }
                else
                {
                    Vector3 preStall = getCoefficientsLowAoA(adjustedConfig.stallAngleLow, adjustedConfig, flapAngle, aspectRatio);
                    Vector3 postStall = getCoefficientsStall(paddedStallLow, adjustedConfig, flapAngle, flapFraction, aspectRatio);
                    float lerpFactor = (alpha - adjustedConfig.stallAngleLow) / (paddedStallLow - adjustedConfig.stallAngleLow);
                    coeffs = Vector3.Lerp(preStall, postStall, lerpFactor);
                }
                return coeffs;
            }
        }
    }

}
