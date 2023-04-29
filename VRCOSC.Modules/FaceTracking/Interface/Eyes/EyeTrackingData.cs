﻿// Copyright (c) VolcanicArts. Licensed under the GPL-3.0 License.
// See the LICENSE file in the repository root for full license text.

using SRanipalLib;

namespace VRCOSC.Modules.FaceTracking.Interface.Eyes;

public class EyeTrackingData
{
    public Eye Left;
    public Eye Right;
    public Eye Combined;

    public float EyesDilation;
    public float EyesPupilDiameter;

    private float maxDilation;
    private float minDilation;
    private float rawDilation;

    public void Initialise()
    {
        Left.Initialise();
        Right.Initialise();
        Combined.Initialise();

        EyesDilation = 0f;
        EyesPupilDiameter = 0f;

        maxDilation = 0f;
        minDilation = float.MaxValue;
    }

    public void Update(EyeDataV2 eyeData)
    {
        rawDilation = 0f;

        updatePupil(eyeData.verbose_data.left);
        updatePupil(eyeData.verbose_data.right);

        Left.Update(eyeData.verbose_data.left, eyeData.expression_data.left);
        Right.Update(eyeData.verbose_data.right, eyeData.expression_data.right);

        Combined.Update(eyeData.verbose_data.combined.eye_data);
        Combined.Widen = (Left.Widen + Right.Widen) / 2f;
        Combined.Squeeze = (Left.Squeeze + Right.Squeeze) / 2f;

        if (rawDilation == 0) return;

        EyesDilation = (rawDilation - minDilation) / (maxDilation - minDilation);
        EyesPupilDiameter = rawDilation > 10f ? 1f : rawDilation / 10f;
    }

    private void updatePupil(SingleEyeData data)
    {
        if (!data.GetValidity(SingleEyeDataValidity.SINGLE_EYE_DATA_PUPIL_DIAMETER_VALIDITY)) return;

        rawDilation = data.pupil_diameter_mm;
        maxDilation = Math.Max(maxDilation, rawDilation);
        minDilation = Math.Min(minDilation, rawDilation);
    }
}