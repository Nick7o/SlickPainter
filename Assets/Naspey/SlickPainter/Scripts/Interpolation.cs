using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Naspey.SlickPainter
{
    [System.Serializable]
    public class InterpolationSettings
    {
        public float PointsDensity = 1f;
    }

    public struct InterpolationFramedata
    {
        public Vector2 NormalizedMousePosition { get; }

        public InterpolationFramedata(Vector2 normalizedMousePosition) => NormalizedMousePosition = normalizedMousePosition;

        public Vector2[] GetInterpolatedPoints(InterpolationFramedata nextFrame, float densityFactor)
        {
            var distance = Vector2.Distance(NormalizedMousePosition, nextFrame.NormalizedMousePosition);
            int pointsCount = Mathf.CeilToInt(distance * 10f * densityFactor + Mathf.Epsilon);

            Vector2[] points = new Vector2[pointsCount];
            for (int i = 0; i < pointsCount; i++)
                points[i] = Vector2.Lerp(NormalizedMousePosition, nextFrame.NormalizedMousePosition, (i + 1) / (float)(pointsCount));
           
            return points;
        }
    }
}