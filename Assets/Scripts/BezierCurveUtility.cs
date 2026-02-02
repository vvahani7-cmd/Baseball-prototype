using UnityEngine;

public class BezierCurveUtility 
{
    public static Vector3[] GenerateCubicBezier(
    Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, int resolution)
    {
        Vector3[] points = new Vector3[resolution + 1];

        for (int i = 0; i <= resolution; i++)
        {
            float t = i / (float)resolution;
            points[i] = EvaluateCubicBezier(p0, p1, p2, p3, t);
        }

        return points;
    }
    public static Vector3 EvaluateCubicBezier(
    Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
    {
        float u = 1f - t;
        return
            u * u * u * p0 +
            3f * u * u * t * p1 +
            3f * u * t * t * p2 +
            t * t * t * p3;
    }
    public static float[] BuildArcLengthTable(Vector3[] points)
    {
        float[] lengths = new float[points.Length];
        float totalLength = 0f;

        lengths[0] = 0f;

        for (int i = 1; i < points.Length; i++)
        {
            totalLength += Vector3.Distance(points[i - 1], points[i]);
            lengths[i] = totalLength;
        }

        return lengths;
    }


}
