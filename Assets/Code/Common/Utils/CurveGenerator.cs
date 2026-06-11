using UnityEngine;

public static class CurveGenerator
{
    public static Vector3[] GetCurvePoints(Vector3 startPoint, Vector3 controlPoint, Vector3 endPoint, int segments)
    {
        Vector3[] points = new Vector3[segments + 1];

        for (int i = 0; i <= segments; i++)
        {
            float t = (float)i / segments;
            points[i] = CalculateQuadraticBezierPoint(t, startPoint, controlPoint, endPoint);
        }

        return points;
    }

    private static Vector3 CalculateQuadraticBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2)
    {
        Vector3 u = Vector3.Lerp(p0, p1, t);
        Vector3 v = Vector3.Lerp(p1, p2, t);
        return Vector3.Lerp(u, v, t);
    }
}